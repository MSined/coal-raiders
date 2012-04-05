using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    class Waypoint
    {
        public struct Edge { public float length; public Waypoint connectedTo; }

        //int temp = 0;
        static int IDCtr = 0;
        public int ID;
        public Vector3 position;
        public List<Edge> connectedEdges = new List<Edge>();

        Matrix world;
        Model model;

        public Waypoint(Game game, Model model, Vector3 position)
        {
            this.ID = IDCtr++;
            this.position = position;

            this.world = this.world = Matrix.CreateTranslation(position);
            this.model = model;
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
