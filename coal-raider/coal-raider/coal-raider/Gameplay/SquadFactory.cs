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
        Flank,
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

                case SquadType.Flank:
                    return createFlank(game, unitList);
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
        public static Vector3[] getPentagramFormationOffset()
        {

            Vector3[] formationOffset = {
                                    new Vector3(-1,0,0),
                                    new Vector3(1,0,0),
                                    new Vector3(0,0,1),
                                    new Vector3(-0.5f,0,-1),
                                    new Vector3(0.5f,0,-1)
                                    };

            return formationOffset;
        }
        public static SquadSlotType[] getPentagramFormationSlotTypes()
        {
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return formationSlotTypes;
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
        public static Vector3[] getPyramidFormationOffset()
        {
            Vector3[] formationOffset = {
                                    new Vector3(0,0,1),
                                    new Vector3(0.5f,0,0),
                                    new Vector3(-0.5f,0,0),
                                    new Vector3(1,0,-1),
                                    new Vector3(0,0,-1),
                                    new Vector3(-1,0,-1)
                                    };

            return formationOffset;
        }
        public static SquadSlotType[] getPyramidFormationSlotTypes()
        {
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Ranged,
                                    };

            return formationSlotTypes;
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
        public static Vector3[] getSquareFormationOffset()
        {
            Vector3[] formationOffset = {
                                    new Vector3(0.5f,0,0.5f),
                                    new Vector3(-0.5f,0,0.5f),
                                    new Vector3(0.5f,0,-0.5f),
                                    new Vector3(-0.5f,0,-0.5f)
                                    };

            return formationOffset;
        }
        public static SquadSlotType[] getSquareFormationSlotTypes()
        {
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return formationSlotTypes;
        }

        private static Squad createFlank(Game game, Unit[] unitList)
        {
            int numUnitsInFormation = 6;
            Vector3[] formationOffset = {
                                    new Vector3(1,0,1),
                                    new Vector3(-1,0,1),
                                    new Vector3(-0.5f,0,0),
                                    new Vector3(0.5f,0,0),
                                    new Vector3(1,0,0),
                                    new Vector3(-1,0,0)
                                    };
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes);
        }
        public static Vector3[] getFlankFormationOffset()
        {
            Vector3[] formationOffset = {
                                    new Vector3(1,0,1),
                                    new Vector3(-1,0,1),
                                    new Vector3(-0.5f,0,0),
                                    new Vector3(0.5f,0,0),
                                    new Vector3(1,0,0),
                                    new Vector3(-1,0,0)
                                    };

            return formationOffset;
        }
        public static SquadSlotType[] getFlankFormationSlotTypes()
        {
            SquadSlotType[] formationSlotTypes = {
                                    SquadSlotType.Melee,
                                    SquadSlotType.Melee,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Magic,
                                    SquadSlotType.Ranged,
                                    SquadSlotType.Ranged,
                                    };

            return formationSlotTypes;
        }

        public static int getFormationUnitCount(SquadType s)
        {
            switch (s)
            {
                case SquadType.Flank:
                    return 6;

                case SquadType.Pentagram:
                    return 5;

                case SquadType.Pyramid:
                    return 6;

                case SquadType.Square:
                    return 4;

                default:
                    return 0;
            }
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
