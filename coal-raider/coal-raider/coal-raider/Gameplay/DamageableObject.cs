using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class DamageableObject : Object
    {

        public int topHP { get; protected set; }
        public int hp { get; protected set; }
        public int topSP { get; protected set; }
        public int sp { get; protected set; }

        public int meleeDefense { get; protected set; }
        public int rangeDefense { get; protected set; }
        public int magicDefense { get; protected set; }

        public int team { get; protected set; }

        public DamageableObject(Game game, Model[] modelComponents, Vector3 position,
            int topHP, int topSP, int meleeDefence, int rangeDefence, int magicDefence, bool isAlive, int team)
            : base(game, modelComponents, position, isAlive, true)
        {
            this.team = team;

            this.topHP = topHP;
            this.hp = topHP;

            this.topSP = topSP;
            this.sp = topSP;

            this.meleeDefense = meleeDefense;
            this.rangeDefense = rangeDefense;
            this.magicDefense = magicDefense;
        }

        public void receiveDamage(int meleeDmg, int rangeDmg, int magicDmg)
        {
            int totalDmg = 0;

            totalDmg += Math.Max(0, meleeDmg - meleeDefense);
            totalDmg += Math.Max(0, rangeDmg - rangeDefense);
            totalDmg += Math.Max(0, magicDmg - magicDefense);

            hp -= totalDmg;
        }

        public void drawHealth(Camera camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D healthTexture)
        {
            if (this.isAlive)
            {
                int healthBarWidth = 20;
                int healthBarHeight = 5;
                Rectangle srcRect, destRect;

                Vector3 screenPos = graphicsDevice.Viewport.Project(this.position + new Vector3(0, 1.7f, 0), camera.projection, camera.view, Matrix.Identity);

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, healthBarWidth, healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None, 0.81f);
                
                float healthPercentage = (float)hp / (float)topHP;

                Color healthColor = new Color(new Vector3(1 - healthPercentage, healthPercentage, 0));

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, (int)(healthPercentage * healthBarWidth), healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, healthColor, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            }
        }

        public void checkIfDead()
        {
            if (hp <= 0)
            {
                isAlive = false;
            }
        }

    }
}
