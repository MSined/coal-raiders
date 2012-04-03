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
        private int numUnitsInFormation = 5;

        public Object target { get; protected set; }
        public float speed;

        Unit[] unitList;

        Vector3[] formationOffset;
        SquadSlotType[] formationSlotTypes;

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
            //Will need to palce units in array according to table with values for each unit type;
            return uList;
        }

        public override void Update(GameTime gameTime, List<Object> colliders, List<Waypoint> waypointList)
        {
            updatePosition();

            if (target != null)
            {
                targetPosition = target.position;
                moveToTargetPosition(waypointList);
            }

            Matrix rotation = getRotationMatrix();
            Vector3 anchor = position + speed * velocity;

            for (int i=0; i < unitList.Length; ++i )
            {
                Vector3 newPos = anchor + Vector3.Transform(formationOffset[i], rotation);
                unitList[i].setTarget(newPos, velocity);
                unitList[i].Update(gameTime, colliders, waypointList);
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
