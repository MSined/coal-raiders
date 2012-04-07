using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    class StaticObject : Object
    {
        public Model model { get; protected set; }

        public StaticObject(Game game, Model[] modelComponents, Vector3 position, bool canCollide)
            : base(game, modelComponents, position, true, canCollide)
        {
            model = modelComponents[0];
            world = /*Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * */ Matrix.CreateTranslation(position);
            this.position = position;
        }

        public override void Draw(Camera camera)
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

            DebugShapeRenderer.AddBoundingBox(bounds, Color.Green);
        }
    }
}