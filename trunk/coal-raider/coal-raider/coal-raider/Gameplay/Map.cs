using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace coal_raider
{
    class Map : Microsoft.Xna.Framework.GameComponent
    {
        private Game game;
        private Model model;
        private Model waypointModel;
        protected Matrix world = Matrix.Identity;
        public Vector2 size { get; protected set; }
        public List<StaticObject> staticObjects { get; protected set; }

        public List<Waypoint> waypointList = new List<Waypoint>();

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

            waypointModel = modelComponents[3];

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
            bool[,] mapBool = new bool[0, 0];
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
                        y = -1;

                    if ((line = sr.ReadLine()) != null)
                    {
                        words = line.Split(delimiterChars);
                        size = new Vector2(int.Parse(words[0]), int.Parse(words[1]));
                    }

                    mapBool = new bool[(int)size.X, (int)size.Y];

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

                            staticObjects.Add(new StaticObject(game, modelArrays[int.Parse(i)], new Vector3((x++ * 2) - size.X, 0, (2 * y) - size.Y)));
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
            createTile(mapBool);
        }

        public void createTile(bool[,] mapBool)
        {
            //mapbool, true = accessible , false = inaccessible
            int xLen = mapBool.GetLength(0);
            int yLen = mapBool.GetLength(1);

            Waypoint[,] mapTemp = new Waypoint[mapBool.GetLength(0), mapBool.GetLength(1)];//link helper

            for (int x = 0; x < xLen; ++x)
            {
                for (int y = 0; y < yLen; ++y)
                {
                    if (mapBool[x, y])
                    {
                        mapTemp[x, y] = new Waypoint(game, waypointModel, new Vector3((x * 2) - size.X, 0, (y * 2) - size.Y));//fixed
                    }
                    else
                    {
                        mapTemp[x, y] = null;
                    }
                }
            }

            Waypoint.Edge edge;
            for (int x = 0; x < xLen; ++x)
            {
                for (int y = 0; y < yLen; ++y)
                {
                    if (mapTemp[x, y] != null)
                    {
                        for (int i = -1; i <= 1; ++i)
                        {
                            for (int j = -1; j <= 1; ++j)
                            {
                                if (!(i == 0 && j == 0) && !(x + i < 0 || y + j < 0) && !(x + i >= xLen || y + j >= yLen))
                                {
                                    if (mapTemp[x + i, y + j] == null)
                                        continue;
                                    edge = new Waypoint.Edge();
                                    edge.connectedTo = mapTemp[x + i, y + j];
                                    edge.length = (mapTemp[x, y].position - edge.connectedTo.position).LengthSquared();
                                    mapTemp[x, y].connectedEdges.Add(edge);
                                }
                            }
                        }

                        waypointList.Add(mapTemp[x, y]);//add waypoint to the list
                    }
                }
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
                if (camera.inCamera(so.position))
                {
                    so.Draw(camera);
                }
            }

            foreach (Waypoint w in waypointList)
            {
                w.Draw(camera);
            }
        }
    }
}
