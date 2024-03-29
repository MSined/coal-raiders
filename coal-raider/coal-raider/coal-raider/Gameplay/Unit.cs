﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class Unit : DamageableObject
    {
        public UnitType type { get; protected set; }

        public Vector3 lookDirection = new Vector3(1, 0, 0);
        public Vector3 velocity = new Vector3(0, 0, 0);
        public Vector3? targetPosition { get; protected set; }

        public int meleeAttack { get; protected set; }
        public int rangeAttack { get; protected set; }
        public int magicAttack { get; protected set; }
        
        public float speed { get; protected set; } 
        public float attackRange { get; protected set; }

        public DamageableObject attackTarget { get; protected set; }

        private float spRecoverTimer = 0f;
        private float spRecoverInterval = 1000f;
        private int spRecoverRate = 5;

        private float armRotation = 0, leftLegRotation = 0, rightLegRotation = 0, attackRate = 0, attackTimer = 0, armUpAngle = 0, armDownAngle = 0, armRotationSpeed = 0;
        private bool armMoveUp = true, leftLegMoveForward = true, moving = false;
        private Matrix armWorld = Matrix.Identity, leftLegWorld = Matrix.Identity, rightLegWorld = Matrix.Identity;
        private Vector3 unitDir = Vector3.Zero;
        private Matrix meshWorld;
        public bool attacking = false;

        private SoundEffect attackSound;

        private Camera camera;

        protected int[] attributes { get; private set; }

        //public Boolean poisoned = false, checkBoxCollision = false;
        //public int burningStacks = 0;
        //public bool wasUpdated = false;

        // Characters initial position is defined by the spawnpoint ther are associated with
        public Unit(Game game, Model[] modelComponents, Vector3 position,
            UnitType type, int topHP, int meleeAttack, int rangeAttack, int magicAttack, int meleeDefence, int rangeDefence, int magicDefence, float speed, bool isAlive, int team,
            float armUpAngle, float armDownAngle, float armRotationSpeed, float attackRange, float attackRate, SoundEffect attackSound, Camera camera)
            : base(game, modelComponents, position, topHP, meleeDefence, rangeDefence, magicDefence, isAlive, team)
        { 
            this.type = type;
            this.speed = speed;

            this.meleeAttack = meleeAttack;
            this.rangeAttack = rangeAttack;
            this.magicAttack = magicAttack;

            this.attackTarget = null;

            this.armUpAngle = armUpAngle;
            this.armDownAngle = armDownAngle;
            this.armRotationSpeed = armRotationSpeed;
            this.attackRange = attackRange;
            this.attackRate = attackRate;
            this.attackSound = attackSound;

            this.camera = camera;
        }

        public override void Update(GameTime gameTime, SpatialHashGrid grid, List<Waypoint> waypointList)
        {
            if (targetPosition != null)
            {
                //moveToTargetPosition(waypointList);
                velocity = (Vector3)targetPosition - position;
            }

            CheckCollisions(grid.getPotentialColliders(this));

            DebugShapeRenderer.AddLine(new Vector3(this.position.X, this.position.Y + 1, this.position.Z),
                                       new Vector3(this.position.X + this.velocity.X * 2, this.position.Y + 1 + this.velocity.Y * 2, this.position.Z + this.velocity.Z * 2),
                                       Color.Black);

            moving = true;

            if (attackTarget != null)
            {
                if (!attackTarget.isAlive)
                {
                    attackTarget = null;
                    attacking = false;
                }
                else
                {
                    Vector3 newVel = attackTarget.position - this.position;
                    // Stop unit when it is close enough to attack
                    if (newVel.Length() < attackRange)
                    {
                        moving = false;
                        attacking = true;

                        newVel.Normalize();
                        lookDirection = newVel;
                        attackTarget.receiveDamage(meleeAttack, rangeAttack, magicAttack);
                    }
                    else
                    {
                        velocity = newVel;
                        velocity.Normalize();
                        lookDirection = velocity;
                    }
                }
            }

            updateAnimation(gameTime, camera);

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
            //CheckCollisions(grid.getPotentialColliders(this));

            checkIfDead();

            /*
            spRecoverTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (sp < topSP && spRecoverTimer > spRecoverInterval)
            {
                sp += spRecoverRate;
                spRecoverTimer = 0;
            }
             */

            //base.Update(gameTime, colliders, cameraTarget, waypointList);
        }

        public bool selectAttack(List<Object> objList)
        {
            DamageableObject mostAggro = null;
            float aggroLevel = 0;

            foreach (Object o in objList)
            {
                if (!(o is DamageableObject /*|| o is AttackableBuilding*/) || !o.isAlive)
                    continue;

                DamageableObject u = (DamageableObject)o;

                if (u.team == team)
                    continue;

                //Calculate which one I want to attack using aggro level
                float thisAggroLevel = 0;

                //Add aggro depending on the type of the unit
                //TODO
                
                //Add aggro depending on distance (to separate same class units)
                thisAggroLevel += 1 / (u.position - position).Length();

                if (thisAggroLevel > aggroLevel)
                {
                    aggroLevel = thisAggroLevel;
                    mostAggro = u;
                }
            }

            if (mostAggro != null)
            {
                attackTarget = mostAggro;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setTarget(Vector3 pos, Vector3 orientation)
        {
            targetPosition = pos;
            lookDirection = orientation;
        }

        private void updateAnimation(GameTime gameTime, Camera cam)
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
                        {
                            float dist = (cam.cameraTarget - position).LengthSquared();
                            if (float.IsNaN(dist)) dist = 0;
                            float vol = dist / 300;
                            float scaledVol = (vol >= 1 ? 0 : (1 - vol));
                            attackSound.Play(scaledVol, 0.0f, 0.0f);
                            armMoveUp = false;
                        }
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
            if (this.isAlive && camera.inView(this))
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
                        be.SpecularColor = blackVector;
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = meshWorld * mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }

                DebugShapeRenderer.AddBoundingBox(bounds, Color.White);

                //BoundingSphere bs = new BoundingSphere(position + new Vector3(0,1,0), attackRange);
                //DebugShapeRenderer.AddBoundingSphere(bs, Color.Red);

            }
        }

        public void CheckCollisions(List<Object> colliders)
        {
            foreach (Object o in colliders)
            {
                if (o.collideable && o.isAlive)// && bounds.Intersects(o.bounds))
                {
                    // If we have already collided
                    if (bounds.Intersects(o.bounds))// && (o is StaticObject || o is Unit))
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
                    // If we are not colliding, then avoid
                    else
                    {
                        if (o is Unit && !attacking)
                            continue;

                        Vector3 lookahead = this.velocity;
                        lookahead.Normalize();
                        // Get ray directions
                        Vector3 rayDirLeft = Vector3.Transform(lookahead, Matrix.CreateRotationY(MathHelper.ToRadians(30)));
                        Vector3 rayDirRight = Vector3.Transform(lookahead, Matrix.CreateRotationY(MathHelper.ToRadians(-30)));
                        // Create rays from unit
                        Ray leftRay = new Ray(this.position, rayDirLeft);
                        Ray rightRay = new Ray(this.position, rayDirRight);

                        float leftSide = 0, rightSide = 0;

                        // If left ray hits something, get its value
                        if (leftRay.Intersects(o.bounds).HasValue)
                            leftSide = (float)leftRay.Intersects(o.bounds).Value;

                        // get right ray value
                        if (rightRay.Intersects(o.bounds).HasValue)
                            rightSide = (float)rightRay.Intersects(o.bounds).Value;

                        // Check if left side collision, turn right
                        if (leftSide < rightSide)
                            velocity += -rayDirRight;
                        // Otherwise turn to the left
                        else if (leftSide > rightSide)
                            velocity += -rayDirLeft;
                    }
                }
            }
        }
    }
}