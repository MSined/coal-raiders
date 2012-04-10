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

            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 20), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 165), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 310), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
            unitUIBoxList.Add(new unitUIBox(new Vector2(1115, 455), squadCreate.Width, squadCreate.Height, altPlus.Width, altPlus.Height, altCreate.Width, altCreate.Height));
        }
    }
}
