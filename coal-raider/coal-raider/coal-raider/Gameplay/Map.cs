using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    class Map : Microsoft.Xna.Framework.GameComponent
    {

        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        public float leftXPos { get; protected set; }
        public float bottomYPos { get; protected set; }

        RasterizerState originalState, transparentState;

        DepthStencilState first, second, original;

        GraphicsDevice graphics;

        public Map(Game game, Model[] modelComponents, float leftX, float bottomY, GraphicsDevice graphics)
            : base(game)
        {
            // Set model attribute
            model = modelComponents[0];

            leftXPos = leftX;
            bottomYPos = bottomY;

            this.graphics = graphics;

            originalState = new RasterizerState();
            originalState.CullMode = CullMode.CullCounterClockwiseFace;

            transparentState = new RasterizerState();
            transparentState.CullMode = CullMode.None;

            first = new DepthStencilState();
            first.DepthBufferEnable = true;
            first.DepthBufferWriteEnable = true;

            second = new DepthStencilState();
            second.DepthBufferEnable = true;
            second.DepthBufferWriteEnable = false;

            original = graphics.DepthStencilState;
        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.SpecularPower = 10f;
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = world * mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }

        }
    }
}
