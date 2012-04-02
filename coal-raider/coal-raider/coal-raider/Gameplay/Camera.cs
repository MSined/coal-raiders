﻿using System;
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
    }
}