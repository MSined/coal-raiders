using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class Camera : Microsoft.Xna.Framework.GameComponent
    {
        // View and projection matrices for camera
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        // Camera attributes for constructor
        public Vector3 cameraPosition { get; protected set; }
        public Vector3 cameraDistFromPlayer;
        Vector3 cameraDirection;
        Vector3 cameraUp;

        public Vector3 cameraTarget;

        // Current scroll wheel value. It stores the cumulative scroll value since start of game
        // Also used to verify against new scroll values to determine if zoom in or out
        float scrollWheelValue = 0;

        private Game game;

        private Vector3 origPos, origTarget, origUp, origDir, origDistFromPlayer;

        private Vector2 screenSizeOver2; // Should be Size technically, but we want floats.
        private Rectangle noActionRectangle;
        private const int MouseCameraBorder = 5; // Really, this should be 1.

        public Camera(Game game, Vector3 pos /* in respect to player*/, Vector3 target, Vector3 up)
            : base(game)
        {
            this.game = game;
            // Set values and create required matrices
            cameraPosition = pos;
            cameraDistFromPlayer = pos;
            cameraDirection = target - pos;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                             (float)Game.Window.ClientBounds.Width /
                                                             (float)Game.Window.ClientBounds.Height,
                                                             1, 1000);
            origPos = pos;
            origTarget = target;
            origUp = up;
            origDir = cameraDirection;
            origDistFromPlayer = cameraDistFromPlayer;

            var vp = game.GraphicsDevice.Viewport;
            noActionRectangle = new Rectangle(MouseCameraBorder, MouseCameraBorder, vp.Width - MouseCameraBorder * 2, vp.Height - MouseCameraBorder * 2);
            screenSizeOver2 = new Vector2(vp.Width / 2, vp.Height / 2);

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var ms = Mouse.GetState();
            var msV = new Vector3((ms.X - screenSizeOver2.X) / screenSizeOver2.X, 0, (ms.Y - screenSizeOver2.Y) / screenSizeOver2.Y);
            var msPt = new Point(ms.X, ms.Y);
            if (!noActionRectangle.Contains(msPt))
            {
                cameraTarget += msV;
            }
            cameraPosition = cameraTarget + (Vector3.UnitZ * 2.2f) + cameraDistFromPlayer;

            // Check for scroll wheel zooming
            // Camera moves along its direction matrix (where it is looking)
            if (Mouse.GetState().ScrollWheelValue < scrollWheelValue)
            {
                cameraDistFromPlayer -= cameraDirection * 1f;
                scrollWheelValue = Mouse.GetState().ScrollWheelValue;
            }
            else if (Mouse.GetState().ScrollWheelValue > scrollWheelValue)
            {
                cameraDistFromPlayer += cameraDirection * 1f;
                scrollWheelValue = Mouse.GetState().ScrollWheelValue;
            }

            // Recreate the lookat matrix to update camera
            CreateLookAt();

            base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        public bool inCamera(Vector3 target)
        {
            if (target.X > cameraPosition.X - 25 &&
                target.X < cameraPosition.X + 25 &&
                target.Z > cameraPosition.Z - 25 &&
                target.Z < cameraPosition.Z)
            {
                return true;
            }
            return false;
        }

        public BoundingFrustum CreateFromRectangle(Rectangle rectangle, Game game)
        {
            var vp = game.GraphicsDevice.Viewport;

            float inverseWidth = 1.0f / (float)rectangle.Width;
            float inverseHeight = 1.0f / (float)rectangle.Height;

            Matrix mat = Matrix.Identity;

            mat.M11 = vp.Width * inverseWidth;
            mat.M22 = vp.Height * inverseHeight;

            mat.M41 = ((float)vp.Width - 2 * (float)rectangle.Center.X) * inverseWidth;
            mat.M42 = -((float)vp.Height - 2 * (float)rectangle.Center.Y) * inverseHeight;

            return new BoundingFrustum((view * projection * mat));
        }

        public BoundingFrustum UnprojectRectangle(Rectangle source, Game game)
        {
            //http://forums.create.msdn.com/forums/p/6690/35401.aspx , by "The Friggm"
            // Many many thanks to him...

            Viewport viewport = game.GraphicsDevice.Viewport;


            // Point in screen space of the center of the region selected
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);

            // Generate the projection matrix for the screen region
            Matrix regionProjMatrix = projection;

            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);

            // Calculate the region center in the projection matrix. M31 is horizonatal center.
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);

            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.
            regionProjMatrix.M33 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);

            return new BoundingFrustum(view * regionProjMatrix);
        }

        public List<Unit> RectangleSelect(List<Unit> objectsList, Rectangle selectionRect, Game game)
        {
            Viewport viewport = game.GraphicsDevice.Viewport;
            // Create a new list to return it
            List<Unit> selectedObj = new List<Unit>();
            foreach (Unit o in objectsList)
            {
                // Getting the 2D position of the object
                Vector3 screenPos = viewport.Project(o.position, projection, view, Matrix.Identity);
                // screenPos is window relative, we change it to be viewport relative
                screenPos.X -= viewport.X;
                screenPos.Y -= viewport.Y;
                if (selectionRect.Contains((int)screenPos.X, (int)screenPos.Y))
                {
                    // Add object to selected objects list
                    selectedObj.Add(o);
                }
            }
            return selectedObj;
        }
    }
}
