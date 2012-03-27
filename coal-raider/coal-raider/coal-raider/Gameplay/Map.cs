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
        private Game game;
        private Model model;
        protected Matrix world = Matrix.Identity;
        public List<StaticObject> staticObjects { get; protected set; }

        /*
        RasterizerState originalState, transparentState;
        DepthStencilState first, second, original;
        */

        GraphicsDevice graphics;

        string mapFileName = ".\\map.txt";

        public Map(Game game, Model[] modelComponents, GraphicsDevice graphics)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;

            // Set models
            model = modelComponents[0];

            Model[] building = new Model[1];
            building[0] = modelComponents[1];

            Model[] tree = new Model[1];
            tree[0] = modelComponents[2];

            Model[][] modelArrays = {
                                        building,
                                        tree
                                    };

            staticObjects = new List<StaticObject>();

            buildMapFromFile(mapFileName, modelArrays);

            /*
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
            */
        }

        private void buildMapFromFile(string fileName, Model[][] modelArrays)
        {
            StreamReader sr = new StreamReader(fileName);
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (sr)
                {
                    char[] delimiterChars = { ' ', ',', '\n', ')', '(', '/' };
                    string[] words = new string[10];
                    String line;
                    int x = 0,
                        y = 0;

                    Vector2 mapSize = new Vector2();
                    if ((line = sr.ReadLine()) != null)
                    {
                        words = line.Split(delimiterChars);
                        mapSize = new Vector2(int.Parse(words[0]), int.Parse(words[1]));
                    }


                    // Read and display lines from the file until the end of
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        x = 0;
                        ++y;

                        words = line.Split(delimiterChars);
                        foreach (string i in words)
                        {
                            if (i.Equals("-"))
                            {
                                ++x;
                                continue;
                            }
                            staticObjects.Add(new StaticObject(game, modelArrays[int.Parse(i)], new Vector3((x++ * 2) - mapSize.X, 0, (2 * y) - mapSize.Y)));
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The map file could not be read:");
                Console.WriteLine(e.Message);
            }
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

            foreach (StaticObject so in staticObjects)
            {
                so.Draw(camera);
            }
        }
    }
}
