using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
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
        public static Unit createUnit(Game game, Model[] modelComponents, Vector3 position, UnitType type, int team)
        {
            switch(type)
            {
                case UnitType.Mage:
                    return createMage(game, modelComponents, position, team);
                case UnitType.Ranger:
                    return createRanger(game, modelComponents, position, team);
                case UnitType.Warrior:
                    return createWarrior(game, modelComponents, position, team);
            }
            return null;
        }

        private static Unit createMage(Game game, Model[] modelComponents, Vector3 position, int team)
        {
            int topHP = 2000;
            float speed = 0.05f;

            float attackRange = 3f;
            float attackRate = 2000f; //in miliseconds

            int meleeAttack = 0;
            int rangeAttack = 0;
            int magicAttack = 10;

            int meleeDefense = 0;
            int rangeDefense = 2;
            int magicDefense = 5;

            float armUpAngle = -80f;
            float armDownAngle = 15f;
            float armRotationSpeed = 10f;

            SoundEffect attackSound = game.Content.Load<SoundEffect>(@"Sounds/staff");
            
            return new Unit(game, modelComponents, position, UnitType.Mage,
                            topHP, meleeAttack, rangeAttack, magicAttack, meleeDefense, rangeDefense, magicDefense, speed, true, team,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate, attackSound);
        }

        private static Unit createRanger(Game game, Model[] modelComponents, Vector3 position, int team)
        {
            int topHP = 2000;
            float speed = 0.07f;

            float attackRange = 4f;
            float attackRate = 1000f; //in miliseconds

            int meleeAttack = 0;
            int rangeAttack = 7;
            int magicAttack = 3;

            int meleeDefense = 3;
            int rangeDefense = 3;
            int magicDefense = 3;

            float armUpAngle = -30f;
            float armDownAngle = 0f;
            float armRotationSpeed = 10f;

            SoundEffect attackSound = game.Content.Load<SoundEffect>(@"Sounds/gun");

            return new Unit(game, modelComponents, position, UnitType.Ranger,
                            topHP, meleeAttack, rangeAttack, magicAttack, meleeDefense, rangeDefense, magicDefense, speed, true, team,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate, attackSound);
        }

        private static Unit createWarrior(Game game, Model[] modelComponents, Vector3 position, int team)
        {
            int topHP = 4000;
            float speed = 0.06f;

            float attackRange = 1f;
            float attackRate = 300f; //interval in miliseconds

            int meleeAttack = 7;
            int rangeAttack = 0;
            int magicAttack = 0;

            int meleeDefense = 5;
            int rangeDefense = 5;
            int magicDefense = 0;

            float armUpAngle = -80f;
            float armDownAngle = 15f;
            float armRotationSpeed = 10f;

            SoundEffect attackSound = game.Content.Load<SoundEffect>(@"Sounds/sword");

            return new Unit(game, modelComponents, position, UnitType.Warrior,
                            topHP, meleeAttack, rangeAttack, magicAttack, meleeDefense, rangeDefense, magicDefense, speed, true, team,
                            armUpAngle, armDownAngle, armRotationSpeed, attackRange, attackRate, attackSound);
        }

    }
}
