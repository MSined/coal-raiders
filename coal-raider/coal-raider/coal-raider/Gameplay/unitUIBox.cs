using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    public class unitUIBox
    {
        public Rectangle mainRect;
        public Vector2 position;
        public Rectangle warriorPlusRect;
        public Rectangle rangerPlusRect;
        public Rectangle magePlusRect;
        public Rectangle warriorMinusRect;
        public Rectangle rangerMinusRect;
        public Rectangle mageMinusRect;
        public Rectangle createRect;

        public unitUIBox(Vector2 position, int squadCreateWidth, int squadCreateHeight, int altPlusMinusWidth, int altPlusMinusHeight, int altCreateWidth, int altCreateHeight)
        {
            this.position = position;
            mainRect = new Rectangle((int)position.X, (int)position.Y, squadCreateWidth, squadCreateHeight);
            warriorPlusRect = new Rectangle((int)position.X, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            rangerPlusRect = new Rectangle((int)position.X + 50, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            magePlusRect = new Rectangle((int)position.X + 100, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            warriorMinusRect = new Rectangle((int)position.X + 25, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            rangerMinusRect = new Rectangle((int)position.X + 75, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            mageMinusRect = new Rectangle((int)position.X + 125, (int)position.Y + 75, altPlusMinusWidth, altPlusMinusHeight);
            createRect = new Rectangle((int)position.X, (int)position.Y + 100, altCreateWidth, altCreateHeight);

            warriorPlus = true;
            rangerPlus = true;
            magePlus = true;
            warriorMinus = true;
            rangerMinus = true;
            mageMinus = true;
            create = true;
            warriorNum = 0;
            rangerNum = 0;
            mageNum = 0;
        }

        public bool warriorPlus;
        public bool rangerPlus;
        public bool magePlus;
        public bool warriorMinus;
        public bool rangerMinus;
        public bool mageMinus;
        public bool create;
        public int warriorNum;
        public int rangerNum;
        public int mageNum;
    }
}
