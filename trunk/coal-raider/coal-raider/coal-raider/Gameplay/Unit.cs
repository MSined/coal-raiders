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

        private float spRecoverTimer = 0f;
        private float spRecoverInterval = 1000f;
        private int spRecoverRate = 5;

        protected int[] attributes { get; private set; }

        //public Boolean poisoned = false, checkBoxCollision = false;
        //public int burningStacks = 0;
        //public bool wasUpdated = false;

        // Characters initial position is defined by the spawnpoint ther are associated with
        public Unit(Game game, Model[] modelComponents, Vector3 position,
            UnitType type, int topHP, int topSP, float speed, bool isAlive)
            : base(game, modelComponents, position, isAlive)
        {
            this.type = type;

            this.topHP = topHP;
            this.hp = topHP;

            this.topSP = topSP;
            this.sp = topSP;

            this.speed = speed;

        }

        public override void Update(GameTime gameTime, List<Object> colliders, List<Waypoint> waypointList)
        {
            if (targetPosition != null)
            {
                //moveToTargetPosition(waypointList);
                velocity = (Vector3)targetPosition - position;
                
            }

            if (!(velocity.X == 0 && velocity.Y == 0 && velocity.Z == 0))
            {
                velocity.Normalize();
                position += speed * velocity;

            }
            
            float angle = (float)Math.Asin(lookDirection.X) + MathHelper.ToRadians(180);
            if (lookDirection.Z > 0)
            {
                angle = MathHelper.ToRadians(180) - angle;
            }
            world = Matrix.CreateRotationY(-angle) * Matrix.CreateTranslation(position);

            //check collisions after moved
            CheckCollisions(colliders);

            checkIfDead();

            /*
            if (this is Enemy)
                checkBox.Update(gameTime, colliders, cameraTarget);
            */

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

        public override void Draw(Camera camera)
        {
            // Required to stop drawing players that are dead and should not be drawn
            if (this.isAlive)
            {
                Matrix[] transforms = new Matrix[modelComponents[0].Bones.Count];
                modelComponents[0].CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in modelComponents[0].Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.SpecularPower = 10f;
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = world * mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }
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
                if (bounds.Intersects(o.bounds))
                {
                    if (o is StaticObject || o is Unit)
                    {
                        //neutralize the Z movement if going in a collision by moving up/down
                        if (position.X > o.bounds.Min.X && position.X < o.bounds.Max.X)
                        {
                            position -= speed * new Vector3(0, 0, velocity.Z);
                        }

                        //neutralize the X movement if going in a collision by moving left/right
                        if (position.Z > o.bounds.Min.Z && position.Z < o.bounds.Max.Z)
                        {
                            position -= speed * new Vector3(velocity.X, 0, 0);
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