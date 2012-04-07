using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class Unit : NavigatingObject
    {
        public UnitType type { get; protected set; }
        public int topHP;
        public int hp { get; protected set; }
        public int topSP;
        public int sp { get; protected set; }
        public float speed;
        public float attackRange;

        private float spRecoverTimer = 0f;
        private float spRecoverInterval = 1000f;
        private int spRecoverRate = 5;

        private float armRotation = 0, leftLegRotation = 0, rightLegRotation = 0, attackRate = 0, attackTimer = 0, armUpAngle = 0, armDownAngle = 0, armRotationSpeed = 0;
        private bool armMoveUp = true, leftLegMoveForward = true, attacking = false, moving = false;
        private Matrix armWorld = Matrix.Identity, leftLegWorld = Matrix.Identity, rightLegWorld = Matrix.Identity;
        private Vector3 unitDir = Vector3.Zero;
        private Matrix meshWorld;

        protected int[] attributes { get; private set; }

        //public Boolean poisoned = false, checkBoxCollision = false;
        //public int burningStacks = 0;
        //public bool wasUpdated = false;

        // Characters initial position is defined by the spawnpoint ther are associated with
        public Unit(Game game, Model[] modelComponents, Vector3 position,
            UnitType type, int topHP, int topSP, float speed, bool isAlive,
            float armUpAngle, float armDownAngle, float armRotationSpeed, float attackRange, float attackRate)
            : base(game, modelComponents, position, isAlive, false)
        {
            this.type = type;

            this.topHP = topHP;
            this.hp = topHP;

            this.topSP = topSP;
            this.sp = topSP;

            this.speed = speed;

            this.armUpAngle = armUpAngle;
            this.armDownAngle = armDownAngle;
            this.armRotationSpeed = armRotationSpeed;
            this.attackRange = attackRange;
            this.attackRate = attackRate;
        }

        public override void Update(GameTime gameTime, SpatialHashGrid grid, List<Waypoint> waypointList)
        {

            if (targetPosition != null)
            {
                //moveToTargetPosition(waypointList);
                velocity = (Vector3)targetPosition - position;
            }
            moving = true;
            /*
            if (targetPosition != null && ((Vector3)targetPosition - this.position).LengthSquared() < attackRange)
            {
                // Stop unit when it is close enough to attack
                moving = false;
            }*/

            updateAnimation(gameTime);

            if (moving && !(velocity.X == 0 && velocity.Y == 0 && velocity.Z == 0))
            {
                if (velocity.Length() > speed)
                {
                    velocity.Normalize();
                    position += speed * velocity;
                }
                else
                {
                    position += velocity;
                }
            }

            float angle = (float)Math.Asin(lookDirection.X) + MathHelper.ToRadians(180);
            if (lookDirection.Z > 0)
            {
                angle = MathHelper.ToRadians(180) - angle;
            }
            world = Matrix.CreateRotationY(-angle) * Matrix.CreateTranslation(position);

            //check collisions after moved
            CheckCollisions(grid.getPotentialColliders(this));

            checkIfDead();

            spRecoverTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (sp < topSP && spRecoverTimer > spRecoverInterval)
            {
                sp += spRecoverRate;
                spRecoverTimer = 0;
            }

            //base.Update(gameTime, colliders, cameraTarget, waypointList);
        }

        public void checkIfDead()
        {
            if (hp <= 0)
            {
                isAlive = false;
            }
        }

        public void setTarget(Vector3 pos, Vector3 orientation)
        {
            targetPosition = pos;
            lookDirection = orientation;
            newTargetPosition = true;
        }

        private void updateAnimation(GameTime gameTime)
        {
            Quaternion q;
            Vector3 s, t;

            // Get world components for arm position calculations
            world.Decompose(out s, out q, out t);

            // Check if close enough to attack
            if (attacking)
            {
                attackTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (attackTimer >= attackRate)
                {
                    if (armMoveUp)
                    {
                        armRotation -= armRotationSpeed;
                        if (armRotation < armUpAngle)
                            armMoveUp = false;
                    }
                    else
                    {
                        armRotation += armRotationSpeed;
                        if (armRotation > armDownAngle)
                        {
                            armMoveUp = true;
                            attackTimer = 0;
                        }
                    }
                }
            }
            // Set arm position and orientation
            Vector3 armPos = t;
            // Model position offset
            armPos.Y += 1.0520f;
            armWorld = Matrix.CreateRotationX(MathHelper.ToRadians(armRotation)) * Matrix.CreateFromQuaternion(q) * Matrix.CreateTranslation(armPos);

            // Set leg position and orientation
            if (moving)
            {
                if (leftLegMoveForward)
                {
                    leftLegRotation -= 5f;
                    rightLegRotation += 5f;
                    if (leftLegRotation < -30)
                        leftLegMoveForward = false;
                }
                else
                {
                    leftLegRotation += 5f;
                    rightLegRotation -= 5f;
                    if (leftLegRotation > 30)
                        leftLegMoveForward = true;
                }
            }
            Vector3 legPos = t;
            // Model position offset
            legPos.Y += 0.7153f;
            leftLegWorld = Matrix.CreateRotationX(MathHelper.ToRadians(leftLegRotation)) * Matrix.CreateFromQuaternion(q) * Matrix.CreateTranslation(legPos);
            rightLegWorld = Matrix.CreateRotationX(MathHelper.ToRadians(rightLegRotation)) * Matrix.CreateFromQuaternion(q) * Matrix.CreateTranslation(legPos);
        }

        public override void Draw(Camera camera)
        {
            // Required to stop drawing players that are dead and should not be drawn
            if (this.isAlive)
            {
                Matrix[] transforms = new Matrix[modelComponents[0].Bones.Count];
                modelComponents[0].CopyAbsoluteBoneTransformsTo(transforms);

                int meshNum = 0;
                foreach (ModelMesh mesh in modelComponents[0].Meshes)
                {
                    if (meshNum == 3)
                        meshWorld = armWorld;
                    else if (meshNum == 0)
                        meshWorld = leftLegWorld;
                    else if (meshNum == 2)
                        meshWorld = rightLegWorld;
                    else
                        meshWorld = world;

                    ++meshNum;
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.SpecularPower = 10f;
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = meshWorld * mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }

                DebugShapeRenderer.AddBoundingBox(bounds, Color.White);

            }
        }

        public void drawHealth(Camera camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D healthTexture)
        {
            if (this.isAlive)
            {
                int healthBarWidth = 20;
                int healthBarHeight = 5;
                Rectangle srcRect, destRect;

                Vector3 screenPos = graphicsDevice.Viewport.Project(this.position + new Vector3(0, 0.8f, 0), camera.projection, camera.view, Matrix.Identity);

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, healthBarWidth, healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None, 0.81f);

                float healthPercentage = (float)hp / (float)topHP;

                Color healthColor = new Color(new Vector3(1 - healthPercentage, healthPercentage, 0));

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, (int)(healthPercentage * healthBarWidth), healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, healthColor, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            }
        }

        //collision vs buildings/ all bullet collisions are in Bullet
        public void CheckCollisions(List<Object> colliders)
        {
            foreach (Object o in colliders)
            {

                if (o.collideable && bounds.Intersects(o.bounds))
                {
                    if (o is StaticObject || o is Unit)
                    {
                        //neutralize the Z movement if going in a collision by moving up/down
                        if ((bounds.Max.X > o.bounds.Min.X && bounds.Max.X < o.bounds.Max.X) ||
                            (bounds.Min.X > o.bounds.Min.X && bounds.Min.X < o.bounds.Max.X))
                        {
                            Vector3 dir = o.position - position;
                            dir.Normalize();
                            position -= speed * new Vector3(0, 0, dir.Z);
                        }

                        //neutralize the X movement if going in a collision by moving left/right
                        if ((bounds.Max.Z > o.bounds.Min.Z && bounds.Max.Z < o.bounds.Max.Z) ||
                            (bounds.Min.Z > o.bounds.Min.Z && bounds.Min.Z < o.bounds.Max.Z))
                        {
                            Vector3 dir = o.position - position;
                            dir.Normalize();
                            position -= speed * new Vector3(dir.X, 0, 0);
                        }

                        /*update bounds again to make sure Character does not get stuck
                        if (bounds.FloatIntersects(o.bounds))
                        {//push against the building
                            Vector3 moveBack = position - o.position;
                            moveBack.Normalize();
                            position += moveBack * speed;
                        }*/
                    }
                }
            }
        }
    }
}