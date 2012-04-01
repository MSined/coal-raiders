using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    class Waypoint : Object
    {
        public struct Edge { public float length; public Waypoint connectedTo; }

        //int temp = 0;
        static int IDCtr = 0;
        public int ID;
        //public Vector3 position;
        public List<Edge> connectedEdges = new List<Edge>();

        public Waypoint(Game game, Model[] modelComponents, Vector3 position)
            : base(game, modelComponents, position, false)
        {
            this.ID = IDCtr++;
            this.position = position;
        }

        public override void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[modelComponents[0].Bones.Count];
            modelComponents[0].CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in modelComponents[0].Meshes)
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
