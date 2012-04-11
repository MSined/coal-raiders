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
        //Flank, //6
        Lane, //2
        Pentagram, //5
        Pyramid, //6
        Solo, //1
        Square, //4
        Triangle, //3
    }

    public enum SquadSlotType
    {
        Magic,
        Melee,
        Ranged
    }

    static class SquadFactory
    {
        public static Squad createSquad(Game game, Unit[] unitList, int team)
        {
            SquadType? type = getFormationFromCount(unitList.Length);

            switch(type)
            {
                    /*
                case SquadType.Flank:
                    return createFlank(game, unitList, team);
                     * */

                case SquadType.Lane:
                    return createLane(game, unitList, team, AveragePosition(unitList));

                case SquadType.Pentagram:
                    return createPentagram(game, unitList, team, AveragePosition(unitList));

                case SquadType.Pyramid:
                    return createPyramid(game, unitList, team, AveragePosition(unitList));

                case SquadType.Solo:
                    return createSolo(game, unitList, team, AveragePosition(unitList));

                case SquadType.Square:
                    return createSquare(game, unitList, team, AveragePosition(unitList));

                case SquadType.Triangle:
                    return createTriangle(game, unitList, team, AveragePosition(unitList));
            }
            return null;
        }
        /*
        private static Squad createFlank(Game game, Unit[] unitList, int team)
        {
            int numUnitsInFormation = 6;
            Vector3[] formationOffset = getFormationOffset(SquadType.Flank);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Flank);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team);
        }*/

        private static Squad createLane(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 2;
            Vector3[] formationOffset = getFormationOffset(SquadType.Lane);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Lane);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        private static Squad createPentagram(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 5;
            Vector3[] formationOffset = getFormationOffset(SquadType.Pentagram);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Pentagram);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        private static Squad createPyramid(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 6;
            Vector3[] formationOffset = getFormationOffset(SquadType.Pyramid);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Pyramid);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        private static Squad createSolo(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 1;
            Vector3[] formationOffset = getFormationOffset(SquadType.Solo);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Solo);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        private static Squad createSquare(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 4;
            Vector3[] formationOffset = getFormationOffset(SquadType.Square);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Square);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        private static Squad createTriangle(Game game, Unit[] unitList, int team, Vector3 startPos)
        {
            int numUnitsInFormation = 3;
            Vector3[] formationOffset = getFormationOffset(SquadType.Triangle);
            SquadSlotType[] formationSlotTypes = getFormationSlotTypes(SquadType.Triangle);

            return new Squad(game, unitList, numUnitsInFormation, formationOffset, formationSlotTypes, team, startPos);
        }

        public static SquadType? getFormationFromCount(int i){
            foreach (SquadType st in Enum.GetValues(typeof(SquadType)))
            {
                if (i == SquadFactory.getFormationUnitCount(st))
                {
                    return st;
                }
            }
            return null;
        }

        public static int getFormationUnitCount(SquadType s)
        {
            switch (s)
            {
                    /*
                case SquadType.Flank:
                    return 6;*/

                case SquadType.Lane:
                    return 2;

                case SquadType.Pentagram:
                    return 5;

                case SquadType.Pyramid:
                    return 6;

                case SquadType.Solo:
                    return 1;

                case SquadType.Square:
                    return 4;

                case SquadType.Triangle:
                    return 3;

                default:
                    return 0;
            }
        }

        public static Vector3[] getFormationOffset(SquadType s)
        {
            switch (s)
            {
                    /*
                case SquadType.Flank:
                    return new Vector3[] {
                                            new Vector3(1,0,1),
                                            new Vector3(-1,0,1),
                                            new Vector3(-0.5f,0,0),
                                            new Vector3(0.5f,0,0),
                                            new Vector3(1,0,0),
                                            new Vector3(-1,0,0)
                                            };*/

                case SquadType.Lane:
                    return new Vector3[] {
                                            new Vector3(0.7f,0,0),
                                            new Vector3(-0.7f,0,0)
                                            };

                case SquadType.Pentagram:
                    return new Vector3[] {
                                            new Vector3(-1.2f,0,0),
                                            new Vector3(1.2f,0,0),
                                            new Vector3(0,0,1.2f),
                                            new Vector3(-0.7f,0,-1.2f),
                                            new Vector3(0.7f,0,-1.2f)
                                            };

                case SquadType.Pyramid:
                    return new Vector3[] {
                                            new Vector3(0,0,1.2f),
                                            new Vector3(0.7f,0,0),
                                            new Vector3(-0.7f,0,0),
                                            new Vector3(1,0,-1.2f),
                                            new Vector3(0,0,-1.2f),
                                            new Vector3(-1.2f,0,-1.2f)
                                            };

                case SquadType.Solo:
                    return new Vector3[] {
                                            new Vector3(0.01f,0,0)
                                            };

                case SquadType.Square:
                    return new Vector3[] {
                                            new Vector3(0.7f,0,0.7f),
                                            new Vector3(-0.7f,0,0.7f),
                                            new Vector3(0.7f,0,-0.7f),
                                            new Vector3(-0.7f,0,-0.7f)
                                            };

                case SquadType.Triangle:
                    return new Vector3[] {
                                            new Vector3(0,0,1.2f),
                                            new Vector3(0.7f,0,0),
                                            new Vector3(-0.7f,0,0),
                                            };

                default:
                    return null;
            }
        }

        public static SquadSlotType[] getFormationSlotTypes(SquadType s)
        {
            switch (s)
            {
                    /*
                case SquadType.Flank:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Magic,
                                                SquadSlotType.Magic,
                                                SquadSlotType.Ranged,
                                                SquadSlotType.Ranged,
                                                };*/

                case SquadType.Lane:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Magic
                                                };

                case SquadType.Pentagram:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Ranged,
                                                SquadSlotType.Ranged,
                                                };

                case SquadType.Pyramid:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Ranged,
                                                SquadSlotType.Magic,
                                                SquadSlotType.Ranged,
                                                };

                case SquadType.Solo:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee
                                                };

                case SquadType.Square:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Melee,
                                                SquadSlotType.Ranged,
                                                SquadSlotType.Ranged,
                                                };

                case SquadType.Triangle:
                    return new SquadSlotType[] {
                                                SquadSlotType.Melee,
                                                SquadSlotType.Ranged,
                                                SquadSlotType.Ranged,
                                                };

                default:
                    return null;
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

        public static Vector3 AveragePosition(Unit[] list)
        {
            Vector3 totalPosition = new Vector3();
            for (int i = 0; i < list.Length; ++i)
            {
                totalPosition += list[i].position;// -Vector3.Transform(formationOffset[i], rotation);
            }

            return totalPosition / list.Length;
        }
    }
}
