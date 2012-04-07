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
                    return createMage(game, modelComponents, position, -80f, 15f, 10f, 25f, 2000f);
                case UnitType.Ranger:
                    return createRanger(game, modelComponents, position, -30f, 0f, 10f, 40f, 1000f);
                case UnitType.Warrior:
                    return createWarrior(game, modelComponents, position, -80f, 15f, 10f, 1.5f, 300f);
            }
            return null;
        }

        private static Unit createMage(Game game, Model[] modelComponents, Vector3 position,
                                       float armUpAngle, float armDownAngle, float armRotationSpeed, float attackRange, float attackRate)
        {
            int topHP = 100;
            int topSP = 100;
            float speed = 0.05f;
            return new Unit(game, modelComponents, position, UnitType.Mage, topHP, topSP, speed, true, 
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

        private static Unit createRanger(Game game, Model[] modelComponents, Vector3 position,
                                         float armUpAngle, float armDownAngle, float armRotationSpeed, float attackRange, float attackRate)
        {
            int topHP = 100;
            int topSP = 50;
            float speed = 0.07f;
            return new Unit(game, modelComponents, position, UnitType.Ranger, topHP, topSP, speed, true,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

        private static Unit createWarrior(Game game, Model[] modelComponents, Vector3 position, 
                                          float armUpAngle, float armDownAngle, float armRotationSpeed, float attackRange, float attackRate)
        {
            int topHP = 200;
            int topSP = 20;
            float speed = 0.06f;
            return new Unit(game, modelComponents, position, UnitType.Warrior, topHP, topSP, speed, true,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate);
        }

    }
}
