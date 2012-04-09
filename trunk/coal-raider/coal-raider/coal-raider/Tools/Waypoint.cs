using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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

        // REMOVE THESE VARIABLES
        public Texture2D texture1 = GameplayScreen.wpltex1, texture2 = GameplayScreen.wpltex2, texture3 = GameplayScreen.wpltex3;
        public bool closed = false, open = false;

        public Waypoint(Game game, Model model, Vector3 position)
        {
            this.ID = IDCtr++;
            this.position = position;

            this.world = this.world = Matrix.CreateTranslation(position);
            this.model = model;
        }

        [Conditional("DEBUG")]
        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    // REMOVE TEXTURE ENABLED AND RELATED CODE
                    be.TextureEnabled = true;
                    be.Texture = texture1;
                    if (closed)
                        be.Texture = texture2;
                    else if (open)
                        be.Texture = texture3;
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
