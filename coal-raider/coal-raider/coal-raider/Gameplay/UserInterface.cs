using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class UserInterface
    {
        public List<unitUIBox> unitUIBoxList = new List<unitUIBox>();
        public Texture2D userInterface, blankTexture, squadCreate, altCreate, altMinus, altPlus, resources, squadsTexture;
        public UserInterface(Game game)
        {
            userInterface = game.Content.Load<Texture2D>(@"UI\UI");
            squadCreate = game.Content.Load<Texture2D>(@"UI\squadCreate");
            altCreate = game.Content.Load<Texture2D>(@"UI\altCreate");
            altMinus = game.Content.Load<Texture2D>(@"UI\altMinus");
            altPlus = game.Content.Load<Texture2D>(@"UI\altPlus");
            resources = game.Content.Load<Texture2D>(@"UI\resources");
            squadsTexture = game.Content.Load<Texture2D>(@"UI\squads");
            blankTexture = game.Content.Load <Texture2D>("blank");

            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 20), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 165), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 310), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 455), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
        }

        public void drawCooldowns(double totalTime, double elapsedTime, int unitTypeNum, ScreenManager sm)
        {
            //1115x635
            //UISprites.Draw(texture, new Rectangle(((int)Math.Floor((float)(203) / (float)1000 * width)) + ((int)(((float)44 / (float)1000) * width) * abilityNum) + ((int)(((float)7 / (float)1000) * width) * abilityNum), (height - (int)(width / 1000f * 200f) + (int)Math.Floor((103 / 200f) * (width / 1000f * 200f))), (int)(((float)44 / (float)1000) * width), (int)(MathHelper.Clamp((float)(elapsedTime / totalTime), 0f, 1f) * (int)(((float)44 / (float)1000) * width))), Color.Black * 0.5f);
            sm.SpriteBatch.Draw(blankTexture, new Rectangle(1115 + unitTypeNum * 50, 635, 50, (int)(MathHelper.Clamp((float)((totalTime - elapsedTime) / totalTime) * 50, 0, 50))), null, Color.Black * 0.5f, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }
    }
}
