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
        public float avgSpeed;
        public float bigestRange;

        public Unit[] unitList;

        Vector3[] formationOffset;
        SquadSlotType[] formationSlotTypes;

        // Moved this out of Update function
        Random rand = new Random();

        public bool wasAttacking = false, verifySquad = false;
        int unitsBeforeAttack;

        public Squad(Game game, Unit[] unitList, int numUnitsInFormation, Vector3[] formationOffset, SquadSlotType[] formationSlotTypes)
            : base(game, null, new Vector3(0,0,0), true, false)
        {
            if (unitList.Length != numUnitsInFormation)
                throw new NotImplementedException();

            this.formationOffset = formationOffset;
            this.formationSlotTypes = formationSlotTypes;
            this.numUnitsInFormation = numUnitsInFormation;
            this.unitsBeforeAttack = unitList.Length;
            this.unitList = placeUnits(unitList);

            bigestRange = float.MinValue;
            float totalSpeed = 0;
            foreach (Unit u in unitList)
            {
                totalSpeed += u.speed;
                if (u.attackRange > bigestRange) bigestRange = u.attackRange;
            }
            avgSpeed = totalSpeed / numUnitsInFormation;
                        
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

        private float totalAssignmentEase(Unit[] uList, SquadSlotType[] sst)
        {
            float total = 0;

            foreach (Unit u in uList)
            {
                float ease = 0;
                for (int i = 0; i < sst.Length; ++i)
                {
                    ease += 1 / (1 + SquadFactory.getSlotCost(u.type, sst[i]));
                }

                total += ease;
            }

            return total;
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
            Vector3 anchor = position + avgSpeed * velocity;
            
            // Calculate distance to squad target
            float range = (target.position - this.position).LengthSquared();

            for (int i=0; i < unitList.Length; ++i )
            {
                Vector3 newPos = anchor + Vector3.Transform(formationOffset[i], rotation);

                // Check if the squad is in range
                if (range < bigestRange)
                {
                    // If so, set the unit target to the squad target
                    unitList[i].setTarget(target.position, velocity);
                    wasAttacking = true;
                    //unitList[i].attacking = true;
                }
                // Otherwise
                else
                {
                    // If we were attacking and are no longer, check if squad formation needs
                    // to be changed
                    if (wasAttacking)
                        checkSquad();

                    // Set the unit target to the squad position
                    unitList[i].setTarget(newPos, velocity);
                    //unitList[i].attacking = false;
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

        private void checkSquad()
        {
            int currentUnitCount = unitList.Length;
            // If we lost units, get a new formation
            if (unitsBeforeAttack != currentUnitCount)
            {
                // Set default values for squad type
                SquadType bestType = SquadType.Square;

                #region Get Best Formation

                float minCost = 999999;
                // Find most suitable formation by current unit count (dont choose formation with less units
                // then we currently have)
                if (currentUnitCount < SquadFactory.getFormationUnitCount(SquadType.Pentagram))
                {
                    // Get slot type information
                    SquadSlotType[] pSlots = SquadFactory.getPentagramFormationSlotTypes();

                    float cost = 0;
                    // For each unit in the current unit list
                    foreach (Unit u in unitList)
                    {
                        // Get the total assignment ease from using this formation
                        cost = totalAssignmentEase(unitList, pSlots);
                    }

                    if (cost < minCost)
                    {
                        minCost = cost;
                        bestType = SquadType.Pentagram;
                    }
                }

                if (currentUnitCount < SquadFactory.getFormationUnitCount(SquadType.Flank))
                {
                    // Get slot type information
                    SquadSlotType[] pSlots = SquadFactory.getFlankFormationSlotTypes();

                    float cost = 0;
                    // For each unit in the current unit list
                    foreach (Unit u in unitList)
                    {
                        // Get the total assignment ease from using this formation
                        cost = totalAssignmentEase(unitList, pSlots);
                    }

                    if (cost < minCost)
                    {
                        minCost = cost;
                        bestType = SquadType.Flank;
                    }
                }

                if (currentUnitCount < SquadFactory.getFormationUnitCount(SquadType.Pyramid))
                {
                    // Get slot type information
                    SquadSlotType[] pSlots = SquadFactory.getPyramidFormationSlotTypes();

                    float cost = 0;
                    // For each unit in the current unit list
                    foreach (Unit u in unitList)
                    {
                        // Get the total assignment ease from using this formation
                        cost = totalAssignmentEase(unitList, pSlots);
                    }

                    if (cost < minCost)
                    {
                        minCost = cost;
                        bestType = SquadType.Pyramid;
                    }
                }

                if (currentUnitCount < SquadFactory.getFormationUnitCount(SquadType.Square))
                {
                    // Get slot type information
                    SquadSlotType[] pSlots = SquadFactory.getSquareFormationSlotTypes();

                    float cost = 0;
                    // For each unit in the current unit list
                    foreach (Unit u in unitList)
                    {
                        // Get the total assignment ease from using this formation
                        cost = totalAssignmentEase(unitList, pSlots);
                    }

                    if (cost < minCost)
                    {
                        minCost = cost;
                        bestType = SquadType.Square;
                    }
                }
                #endregion

                // Get the new formation information and activate new formation
                switch(bestType)
                {
                    case SquadType.Flank:
                        this.formationOffset = SquadFactory.getFlankFormationOffset();
                        this.formationSlotTypes = SquadFactory.getFlankFormationSlotTypes();
                        this.unitList = placeUnits(unitList);
                        break;

                    case SquadType.Pentagram:
                        this.formationOffset = SquadFactory.getPentagramFormationOffset();
                        this.formationSlotTypes = SquadFactory.getPentagramFormationSlotTypes();
                        this.unitList = placeUnits(unitList);
                        break;

                    case SquadType.Pyramid:
                        this.formationOffset = SquadFactory.getPyramidFormationOffset();
                        this.formationSlotTypes = SquadFactory.getPyramidFormationSlotTypes();
                        this.unitList = placeUnits(unitList);
                        break;

                    case SquadType.Square:
                        this.formationOffset = SquadFactory.getSquareFormationOffset();
                        this.formationSlotTypes = SquadFactory.getSquareFormationSlotTypes();
                        this.unitList = placeUnits(unitList);
                        break;

                    default:
                        // This is an error state that should never be reached
                        // In any case, leave squad formation as is
                        break;
                }

                // Set other necessary information for formation operation
                bigestRange = float.MinValue;
                float totalSpeed = 0;
                foreach (Unit u in unitList)
                {
                    totalSpeed += u.speed;
                    if (u.attackRange > bigestRange) bigestRange = u.attackRange;
                }
                avgSpeed = totalSpeed / numUnitsInFormation;

                target = null;
            }
        }
    }
}
