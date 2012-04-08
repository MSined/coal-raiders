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
        private Vector3 blackVector = new Vector3(0, 0, 0);
        Random rand;

        public StaticObject(Game game, Model[] modelComponents, Vector3 position, bool canCollide)
            : base(game, modelComponents, position, true, canCollide)
        {
            model = modelComponents[0];
            //world = /*Matrix.CreateRotationY(MathHelper.ToRadians(angle)) * */ Matrix.CreateTranslation(position);
            this.position = position;
            rand = new Random(this.GetHashCode() * System.Environment.TickCount);
            int randomizer = rand.Next(1, 4);
            switch (randomizer)
            {
                case 1:
                    world = Matrix.CreateWorld(position, Vector3.Left, Vector3.Up);
                    break;
                case 2:
                    world = Matrix.CreateWorld(position, Vector3.Right, Vector3.Up);
                    break;
                case 3:
                    world = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
                    break;
                case 4:
                    world = Matrix.CreateWorld(position, Vector3.Backward, Vector3.Up);
                    break;
            }
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
                    be.SpecularColor = blackVector;
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = world * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }

            //DebugShapeRenderer.AddBoundingBox(bounds, Color.Green);
        }
    }
}