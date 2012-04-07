using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    public enum UnitType
    {
        Mage,
        Ranger,
        Warrior
    }

    static class UnitFactory
    {
        public static Unit createUnit(Game game, Model[] modelComponents, Vector3 position, UnitType type)
        {
            switch(type)
            {
                case UnitType.Mage:
                    return createMage(game, modelComponents, position);
                case UnitType.Ranger:
                    return createRanger(game, modelComponents, position);
                case UnitType.Warrior:
                    return createWarrior(game, modelComponents, position);
            }
            return null;
        }

        private static Unit createMage(Game game, Model[] modelComponents, Vector3 position)
        {
            int topHP = 100;
            int topSP = 100;
            float speed = 0.05f;

            float attackRange = 25f;
            float attackRate = 2000f; //in miliseconds

            float armUpAngle = -80f;
            float armDownAngle = 15f;
            float armRotationSpeed = 10f;
            
            return new Unit(game, modelComponents, position, UnitType.Mage, topHP, topSP, speed, true, 
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

        private static Unit createRanger(Game game, Model[] modelComponents, Vector3 position)
        {
            int topHP = 100;
            int topSP = 50;
            float speed = 0.07f;

            float attackRange = 40f;
            float attackRate = 1000f; //in miliseconds

            float armUpAngle = -30f;
            float armDownAngle = 0f;
            float armRotationSpeed = 10f;

            return new Unit(game, modelComponents, position, UnitType.Ranger, topHP, topSP, speed, true,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

        private static Unit createWarrior(Game game, Model[] modelComponents, Vector3 position)
        {
            int topHP = 200;
            int topSP = 20;
            float speed = 0.06f;

            float attackRange = 1.5f;
            float attackRate = 300f; //interval in miliseconds

            float armUpAngle = -80f;
            float armDownAngle = 15f;
            float armRotationSpeed = 10f;

            return new Unit(game, modelComponents, position, UnitType.Warrior, topHP, topSP, speed, true,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

    }
}
