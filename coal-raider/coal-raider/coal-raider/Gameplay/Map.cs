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

        public List<waypoint> waypointList = new List<waypoint>();

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

            createTile(buildMapFromFile(mapFileName, modelArrays));

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

        private bool[,] buildMapFromFile(string fileName, Model[][] modelArrays)
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

                    bool[,] mapBool = new bool[(int)mapSize.X, (int)mapSize.Y];//return bool[,]

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
                                mapBool[x, y] = true;
                                ++x;
                                continue;
                            }
                            mapBool[x, y] = false;
                            staticObjects.Add(new StaticObject(game, modelArrays[int.Parse(i)], new Vector3((x++ * 2) - mapSize.X, 0, (2 * y) - mapSize.Y)));
                        }
                        
                    }
                    return mapBool;
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The map file could not be read:");
                Console.WriteLine(e.Message);

                return new bool[0, 0];
            }
        }

        public void createTile(bool[,] mapBool)
        {
            //mapbool, true = accessible , false = inaccessible

            waypoint[,] mapTemp = new waypoint[mapBool.GetLength(0), mapBool.GetLength(1)];//link helper

            for (int i = 0; i < mapBool.GetLength(0); ++i)
            {
                for (int j = 0; j < mapBool.GetLength(1); ++j)
                {
                    if (mapBool[i, j])
                    {
                        mapTemp[i, j] = new waypoint(new Vector3(j - mapBool.GetLength(1) / 2, 0, i - mapBool.GetLength(0) / 2));//fixed
                        waypointList.Add(mapTemp[i, j]);//add waypoint to the list
                        //set links
                        if (i != 0)//if have up
                        {
                            if (j != 0 && mapTemp[i - 1, j - 1] != null)//up left
                            {
                                //mapTemp[i, j].upLeft = mapTemp[i - 1, j - 1];//set upLeft link
                                //mapTemp[i - 1, j - 1].downRight = mapTemp[i, j];
                                mapTemp[i, j].neighbors.Add(mapTemp[i - 1, j - 1]);//set upLeft link
                                mapTemp[i - 1, j - 1].neighbors.Add(mapTemp[i, j]);
                            }
                            if (mapTemp[i - 1, j] != null)//up
                            {
                                //mapTemp[i, j].up = mapTemp[i - 1, j];
                                //mapTemp[i - 1, j].down = mapTemp[i, j];
                                mapTemp[i, j].neighbors.Add(mapTemp[i - 1, j]);
                                mapTemp[i - 1, j].neighbors.Add(mapTemp[i, j]);
                            }
                            if (j != mapTemp.GetLength(0) - 1 && j != mapTemp.GetLength(0) && mapTemp[i - 1, j + 1] != null)//up right
                            {
                                //mapTemp[i, j].upRight = mapTemp[i - 1, j + 1];
                                //mapTemp[i - 1, j + 1].downLeft = mapTemp[i, j];
                                mapTemp[i, j].neighbors.Add(mapTemp[i - 1, j + 1]);
                                mapTemp[i - 1, j + 1].neighbors.Add(mapTemp[i, j]);
                            }
                        }
                        if (j != 0 && mapTemp[i, j - 1] != null)//if have left
                        {
                            //mapTemp[i, j].right = mapTemp[i, j - 1];
                            //mapTemp[i, j - 1].left = mapTemp[i, j];
                            mapTemp[i, j].neighbors.Add(mapTemp[i, j - 1]);
                            mapTemp[i, j - 1].neighbors.Add(mapTemp[i, j]);
                        }
                    }
                }
            }//end loop
        }//end createTile


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
    //Joe
    class waypoint
    {
        public Vector3 position { get; private set; }
        public List<waypoint> neighbors = new List<waypoint>();

        public List<waypoint> path = new List<waypoint>();
        public float g;//cost
        public float h;//heuristic
        public float f;

        public waypoint(Vector3 position)
        {
            this.position = position;
            this.g = 0;
            this.h = 0;
            this.f = 0;
        }
    }
}
