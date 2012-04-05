using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    abstract class Object : GameComponent
    {
        public Game game { get; private set; }
        protected Matrix world = Matrix.Identity;
        protected Model[] modelComponents { get; private set; }
        public BoundingBox bounds { get; protected set; }
        public bool isAlive;

        private Vector3 _position;
        public Vector3 position
        {
            get
            {
                return _position;
            }
            
            protected set
            {
                _position = value;
                Vector3 center = (bounds.Max + bounds.Min)/2.0f;
                Vector3 offset = position - center;
                bounds = new BoundingBox(bounds.Min + offset, bounds.Max + offset);
            }
        }
        static int objectIDCounter = 0;
        public int objectID = objectIDCounter++;
        public int[] cellIDs = { -1, -1, -1, -1 };

        public Object(Game game, Model[] modelComponents, Vector3 position, bool isAlive)
            : base(game)
        {
            this.game = game;
            this.modelComponents = modelComponents;
            if (modelComponents != null)
            {
                this.bounds = CreateBoundingBox(modelComponents[0]);
            }
            else
            {
                this.bounds = new BoundingBox();
            }
                
            this.world = Matrix.CreateTranslation(position);
            this.position = position;
            this.isAlive = isAlive;
        }

        public virtual void Draw(Camera camera) { }

        public virtual void Update(GameTime gameTime, SpatialHashGrid grid, List<Waypoint> waypointList) { }

        private static BoundingBox CreateBoundingBox(Model model)
        {

            Matrix[] boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            BoundingBox result = new BoundingBox();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    BoundingBox? meshPartBoundingBox = GetBoundingBox(meshPart, boneTransforms[mesh.ParentBone.Index]);
                    if (meshPartBoundingBox != null)
                        result = BoundingBox.CreateMerged(result, meshPartBoundingBox.Value);
                }
            }

            result = new BoundingBox(result.Min, result.Max);
            return result;
        }

        private static BoundingBox? GetBoundingBox(ModelMeshPart meshPart, Matrix transform)
        {
            if (meshPart.VertexBuffer == null)
                return null;

            Vector3[] positions = VertexElementExtractor.GetVertexElement(meshPart, VertexElementUsage.Position);
            if (positions == null)
                return null;

            Vector3[] transformedPositions = new Vector3[positions.Length];
            Vector3.Transform(positions, ref transform, transformedPositions);

            /*
            for (int i = 0; i < transformedPositions.Length; i++)
            {
                Console.WriteLine(" " + transformedPositions[i]);
            }
            */
            return BoundingBox.CreateFromPoints(transformedPositions);
        }
    }
}
