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
        protected BoundingBox bounds { get; set; }
        public bool isAlive;

        private Vector3 _position;
        protected Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                Vector3 center = bounds.Max - bounds.Min;
                Vector3 offset = position - center;
                bounds = new BoundingBox(bounds.Max + offset, bounds.Min + offset);
            }
        }
        //static int objectIDCounter = 0;
        //public int objectID = objectIDCounter++;
        //public int[] cellIDs = { -1, -1, -1, -1 };

        public Object(Game game, Model[] modelComponents, Vector3 position, bool isAlive)
            : base(game)
        {
            this.game = game;
            this.modelComponents = modelComponents;
            this.world = Matrix.CreateTranslation(position);
            this.bounds = bounds;
            this.position = position;
            this.isAlive = isAlive;
        }

        public abstract void Draw(Camera camera);

        public virtual void Update(GameTime gameTime, List<Object> colliders, Vector3 cameraTarget){}

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

            for (int i = 0; i < transformedPositions.Length; i++)
            {
                Console.WriteLine(" " + transformedPositions[i]);
            }
            return BoundingBox.CreateFromPoints(transformedPositions);
        }
    }
}
