using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class Squad : NavigatingObject
    {
        public int numUnitsInFormation;

        public Object target { get; protected set; }
        private float avgSpeed;
        private float biggestRange;
        private float minRange = 3;
        private float formationMovementSpeed = 0.06f;

        public Unit[] unitList { get; protected set; }

        private Vector3[] formationOffset; 
        private SquadSlotType[] formationSlotTypes;

        // Moved this out of Update function
        private Random rand = new Random();
         
        //public bool wasAttacking = false, verifySquad = false;
        private int unitsBeforeAttack;
        public bool attacking = false;

        public int team { get; protected set; }

        public Squad(Game game, Unit[] unitList, int numUnitsInFormation, Vector3[] formationOffset, SquadSlotType[] formationSlotTypes, int team, Vector3 startPos)
            : base(game, null, startPos, true, false)
        {
            if (unitList.Length != numUnitsInFormation)
                throw new NotImplementedException();

            this.formationOffset = formationOffset;
            this.formationSlotTypes = formationSlotTypes;
            this.numUnitsInFormation = numUnitsInFormation;
            this.unitsBeforeAttack = unitList.Length;
            this.unitList = placeUnits(unitList);
            this.team = team;

            biggestRange = getBiggestRange();
            avgSpeed = getSpeed();
            updatePosition();

            target = null;
        }

        private Unit[] placeUnits(Unit[] uList)
        {
            Unit[] newList = new Unit[uList.Length];
            for (int i = 0; i < newList.Length; ++i)
            {
                newList[i] = null;
            }

            uList = sortByAssignmentEase(uList);

            foreach (Unit u in uList)
            {
                int index = 0;
                int smallestCost = int.MaxValue;
                for (int i = 0; i < newList.Length; ++i)
                {
                    if (newList[i] == null)
                    {
                        int newCost = SquadFactory.getSlotCost(u.type, formationSlotTypes[i]);
                        if (newCost < smallestCost)
                        {
                            smallestCost = newCost;
                            index = i;
                        }

                        if (newCost == 0)
                            break;
                    }
                }

                newList[index] = u;
            }

            return newList;
        }

        private Unit[] sortByAssignmentEase(Unit[] uList)
        {
            MultiMap<float, Unit> d = new MultiMap<float, Unit>();

            foreach (Unit u in uList)
            {
                float ease = 0;
                for (int i = 0; i < formationSlotTypes.Length; ++i)
                {
                    ease += 1 / (1 + SquadFactory.getSlotCost(u.type, formationSlotTypes[i]));
                }

                d.Add(ease, u);
            }
            
            return Enumerable.Reverse(d.Values).ToArray();
        }

        public override void Update(GameTime gameTime, SpatialHashGrid grid, List<Waypoint> waypointList)
        {
            //updatePosition();
            if(!attacking)
                this.position += this.velocity * this.formationMovementSpeed;

            if (target != null)
            {
                targetPosition = target.position;
                moveToTargetPosition(waypointList);
            }

            Matrix rotation = getRotationMatrix();
            //Vector3 anchor = position + avgSpeed * velocity;

            //Debug Stuff
            BoundingSphere bs = new BoundingSphere(this.position, biggestRange);
            DebugShapeRenderer.AddBoundingSphere(bs, Color.Red);
            //End Debug stuff

            BoundingBox bb = new BoundingBox(bs.Center - new Vector3(biggestRange, biggestRange, biggestRange), bs.Center + new Vector3(biggestRange, biggestRange, biggestRange));

            checkSquad();

            //Get possible attackers
            List<Object> possibleAttack =  grid.getAttackBoxColliders(bb);
            List<Object> temp = new List<Object>();
            // This process can be done in "getAttackBoxColliders" so as to avoid
            // doing it again over here
            foreach (Object o in possibleAttack)
            {
                if (o.bounds.Intersects(bs) && !(o is StaticObject))
                    temp.Add(o);
            }
            possibleAttack = temp;
            
            // Moved these outside of loop for efficiency
            attacking = false;
            Vector3 newPos = Vector3.Zero;

            for (int i=0; i < unitList.Length; ++i )
            {
                newPos = this.position + Vector3.Transform(formationOffset[i], rotation);
                //bool attacking = false;

                // Check if the squad is in range
                if (possibleAttack.Count > 0)
                {
                    // If so, tell the unit to select one of the other units and attack
                    attacking = unitList[i].selectAttack(possibleAttack);
                }
                
                if (!attacking)
                {
                    // Set the unit target to the squad position
                    unitList[i].setTarget(newPos, velocity);
                }
                unitList[i].Update(gameTime, grid, waypointList);

                if (unitList[i].attacking)
                    attacking = true;
            }
        }

        private Matrix getRotationMatrix()
        {
            if (!(velocity.X == 0 && velocity.Y == 0 && velocity.Z == 0))
            {
                velocity.Normalize();
                float angle = (float)Math.Asin(velocity.X);
                if (velocity.Z < 0) angle = - (angle - MathHelper.ToRadians(180));
                return Matrix.CreateRotationY(angle);
            }
            return Matrix.Identity;
        }

        private void updatePosition()
        {
            Matrix rotation = getRotationMatrix();
            Vector3 totalPosition = new Vector3();
            for (int i = 0; i < unitList.Length; ++i)
            {
                totalPosition += unitList[i].position - Vector3.Transform(formationOffset[i], rotation);
            }

            position = totalPosition / numUnitsInFormation;
        }

        public override void Draw(Camera camera)
        {
            foreach (Unit u in unitList)
            {
                u.Draw(camera);
            }
        }

        public void drawHealth(Camera camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D healthTexture)
        {
            foreach (Unit u in unitList)
            {
                u.drawHealth(camera, spriteBatch, graphicsDevice, healthTexture);
            }
        }

        public void setTarget(Object t)
        {
            target = t;
            targetPosition = t.position;
            newTargetPosition = true;
        }

        public bool Intersects(BoundingFrustum bf)
        {
            foreach (Unit u in unitList)
            {
                if (bf.Contains(u.bounds) != ContainmentType.Disjoint)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Intersects(Ray r)
        {
            foreach (Unit u in unitList)
            {
                if (r.Intersects(u.bounds).HasValue)
                {
                    return true;
                }
            }
            return false;
        }

        private float getBiggestRange()
        {
            float br = 0;
            foreach (Unit u in unitList)
            {
                if (u.attackRange > br) br = u.attackRange + 0.5f;
            }

            if (br < minRange) br = minRange;

            return br;
        }

        private float getSpeed()
        {
            float totalSpeed = 0;
            foreach (Unit u in unitList)
            {
                totalSpeed += u.speed;
            }
            // Make the avg speed a slightly smaller value than the real speed of the units
            return totalSpeed / (numUnitsInFormation * 1.5f);
        }

        private void checkSquad()
        {
            foreach (Unit u in unitList)
            {
                if (!u.isAlive)
                {
                    updateSquad();
                    return;
                }
            }
        }

        private void updateSquad()
        {
            List<Unit> uList = new List<Unit>();
            foreach(Unit u in unitList)
            {
                if (u.isAlive)
                {
                    uList.Add(u);
                }                    
            }
            unitList = uList.ToArray();

            int currentUnitCount = unitList.Length;

            if (currentUnitCount == 0)
            {
                this.isAlive = false;
                return;
            }

            // If we lost units, get a new formation
            if (unitsBeforeAttack != currentUnitCount)
            {
                // Set default values for squad type
                SquadType bestType = (SquadType)SquadFactory.getFormationFromCount(currentUnitCount);

                this.formationOffset = SquadFactory.getFormationOffset(bestType);
                this.formationSlotTypes = SquadFactory.getFormationSlotTypes(bestType);
                numUnitsInFormation = SquadFactory.getFormationUnitCount(bestType);
                this.unitList = placeUnits(unitList);

                // Set other necessary information for formation operation
                biggestRange = getBiggestRange();
                avgSpeed = getSpeed();

                //newTargetPosition = false;
                //target = null;
            }
        }
    }
}
