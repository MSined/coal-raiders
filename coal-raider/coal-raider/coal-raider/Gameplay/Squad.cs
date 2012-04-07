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
        private int numUnitsInFormation;

        public Object target { get; protected set; }
        public float speed;

        public Unit[] unitList;

        Vector3[] formationOffset;
        SquadSlotType[] formationSlotTypes;

        // Moved this out of Update function
        Random rand = new Random();

        public Squad(Game game, Unit[] unitList, int numUnitsInFormation, Vector3[] formationOffset, SquadSlotType[] formationSlotTypes)
            : base(game, null, new Vector3(0,0,0), true)
        {
            if (unitList.Length != numUnitsInFormation)
                throw new NotImplementedException();

            this.formationOffset = formationOffset;
            this.formationSlotTypes = formationSlotTypes;
            this.numUnitsInFormation = numUnitsInFormation;
            this.unitList = placeUnits(unitList);
            
            float totalSpeed = 0;
            foreach (Unit u in unitList)
            {
                totalSpeed += u.speed;
            }
            speed = totalSpeed / numUnitsInFormation;
                        
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
            updatePosition();

            if (target != null)
            {
                targetPosition = target.position;
                moveToTargetPosition(waypointList);
            }

            Matrix rotation = getRotationMatrix();
            Vector3 anchor = position + speed * velocity;
            
            // Calculate distance to squad target
            float distSqrd = (target.position - this.position).LengthSquared();

            for (int i=0; i < unitList.Length; ++i )
            {
                Vector3 newPos = anchor + Vector3.Transform(formationOffset[i], rotation);

                // Check if the squad is in range
                if (distSqrd < 10)
                {
                    // If so, set the unit target to the squad target
                    unitList[i].setTarget(target.position, velocity);
                    unitList[i].attacking = true;
                }
                // Otherwise
                else
                {
                    // Set the unit target to the squad position
                    unitList[i].setTarget(newPos, velocity);
                }

                unitList[i].Update(gameTime, grid, waypointList);
            }

        }

        private Matrix getRotationMatrix()
        {
            if (!(velocity.X == 0 && velocity.Y == 0 && velocity.Z == 0))
            {
                velocity.Normalize();
                float angle = (float)Math.Asin(velocity.X);
                if (velocity.Z > 0) angle = -angle;
                return Matrix.CreateRotationY(-angle);
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

        public void setTarget(Object t)
        {
            target = t;
            targetPosition = t.position;
            newTargetPosition = true;
        }
    }
}
