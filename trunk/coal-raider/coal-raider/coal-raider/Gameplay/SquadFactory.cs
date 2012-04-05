using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    public enum SquadType
    {
        Pentagram,
        Pyramid,
        Square,
    }

    public enum SquadSlotType
    {
        Magic,
        Melee,
        Ranged
    }

    static class SquadFactory
    {
        public static Squad createSquad(Game game, Unit[] unitList, SquadType type)
        {
            switch(type)
            {
                case SquadType.Pentagram:
                    return createPentagram(game, unitList);
                case SquadType.Pyramid:
                    return createPyramid(game, unitList);
                case SquadType.Square:
                    return createSquare(game, unitList);
            }
            return null;
        }

        private static Squad createPentagram(Game game, Unit[] unitList)
        {
            int numUnitsInFormation = 5;
            Vector3[] formationOffset = {
                                    new Vector3(-1,0,0),
                                    new Vector3(1,0,0),
                                    new Vector3(0,0,1),
                                    new Vector3(-0.5f,0,-1),
                                    new Vector3(0.5f,0,-1)
                                    };
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes);
        }

        private static Squad createPyramid(Game game, Unit[] unitList)
        {
            int numUnitsInFormation = 6;
            Vector3[] formationOffset = {
                                    new Vector3(0,0,1),
                                    new Vector3(0.5f,0,0),
                                    new Vector3(-0.5f,0,0),
                                    new Vector3(1,0,-1),
                                    new Vector3(0,0,-1),
                                    new Vector3(-1,0,-1)
                                    };
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Ranged,
                                    };

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes);
        }

        private static Squad createSquare(Game game, Unit[] unitList)
        {
            int numUnitsInFormation = 4;
            Vector3[] formationOffset = {
                                    new Vector3(0.5f,0,0.5f),
                                    new Vector3(-0.5f,0,0.5f),
                                    new Vector3(0.5f,0,-0.5f),
                                    new Vector3(-0.5f,0,-0.5f)
                                    };
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes);
        }

        public static int getSlotCost(UnitType uType, SquadSlotType sType)
        {
            switch (uType)
            {
                case UnitType.Mage:
                    switch (sType)
                    {
                        case SquadSlotType.Magic:
                            return 0;
                        case SquadSlotType.Melee:
                            return 2000;
                        case SquadSlotType.Ranged:
                            return 500;
                    }
                    break;

                case UnitType.Ranger:
                    switch (sType)
                    {
                        case SquadSlotType.Magic:
                            return 1000;
                        case SquadSlotType.Melee:
                            return 1500;
                        case SquadSlotType.Ranged:
                            return 0;
                    }
                    break;

                case UnitType.Warrior:
                    switch (sType)
                    {
                        case SquadSlotType.Magic:
                            return 2000;
                        case SquadSlotType.Melee:
                            return 0;
                        case SquadSlotType.Ranged:
                            return 1000;
                    }
                    break;

            }
            throw new NotSupportedException();
        }
    }
}
