#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
#endregion

namespace coal_raider
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        /*---- Original GameplayScreen Fields -----*/
        ContentManager content;
        SpriteFont gameFont;
        GameComponentCollection components = new GameComponentCollection();
        SpatialHashGrid grid;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;
        /*---- End Original GameplayScreen Fields -----*/

        Camera camera;
        Map map;

        BoundingFrustumRenderer bfRenderer;

        Squad squad1, squad2, squad3;

        Model mountainModel, treeModel, buildingModel, unitModelWarrior, unitModelRanger, unitModelMage, groundTileModel, selectionRingModel;

        Texture2D mDottedLine, userInterface, blankTexture, squadCreate, altCreate, altMinus, altPlus;
        Rectangle mSelectionBox, squadCreateRec1, squadCreateRec2, squadCreateRec3, squadCreateRec4;

        List<Rectangle> altCreateRec = new List<Rectangle>();
        List<Rectangle> altPlusRec = new List<Rectangle>();
        List<Rectangle> altMinusRec = new List<Rectangle>();

        List<Unit> testUnitList = new List<Unit>();
        List<Squad> selectedSquads = new List<Squad>();
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            /*---- Original GameplayScreen Initialization -----*/
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
            /*---- End Original GameplayScreen Initialization -----*/
            
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            // Create camera and add to components list
            camera = new Camera(ScreenManager.Game, new Vector3(0, 20, 10), Vector3.Zero, -Vector3.UnitZ);
            components.Add(camera);

            // Initialize Models
            groundTileModel = ScreenManager.Game.Content.Load<Model>(@"Models\ground");
            mountainModel = ScreenManager.Game.Content.Load<Model>(@"Models\mountain");
            treeModel = ScreenManager.Game.Content.Load<Model>(@"Models\trees");
            buildingModel = ScreenManager.Game.Content.Load<Model>(@"Models\Building");

            unitModelWarrior = ScreenManager.Game.Content.Load<Model>(@"Models\warrior");
            unitModelRanger = ScreenManager.Game.Content.Load<Model>(@"Models\ranger");
            unitModelMage = ScreenManager.Game.Content.Load<Model>(@"Models\mage");
            selectionRingModel = ScreenManager.Game.Content.Load<Model>(@"Models\SelectionRing");

            Model waypointModel = ScreenManager.Game.Content.Load<Model>(@"Models\waypointModel");

            mDottedLine = ScreenManager.Game.Content.Load<Texture2D>("DottedLine");
            userInterface = ScreenManager.Game.Content.Load<Texture2D>(@"UI\UI");
            squadCreate = ScreenManager.Game.Content.Load<Texture2D>(@"UI\squadCreate");
            altCreate = ScreenManager.Game.Content.Load<Texture2D>(@"UI\altCreate");
            altMinus = ScreenManager.Game.Content.Load<Texture2D>(@"UI\altMinus");
            altPlus = ScreenManager.Game.Content.Load<Texture2D>(@"UI\altPlus");
            blankTexture = ScreenManager.Game.Content.Load<Texture2D>("blank");

            squadCreateRec1 = new Rectangle(1115, 20, squadCreate.Width, squadCreate.Height);
            squadCreateRec2 = new Rectangle(1115, 165, squadCreate.Width, squadCreate.Height);
            squadCreateRec3 = new Rectangle(1115, 310, squadCreate.Width, squadCreate.Height);
            squadCreateRec4 = new Rectangle(1115, 455, squadCreate.Width, squadCreate.Height);

            for (int i = 0; i < 4; i++)
            {
                altCreateRec.Add(new Rectangle(1115, 120 + (i * 145), altCreate.Width, altCreate.Height));
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    altPlusRec.Add(new Rectangle(1115 + (j * 50), 95 + (i * 145), altPlus.Width, altPlus.Height));
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    altMinusRec.Add(new Rectangle(1140 + (j * 50), 95 + (i * 145), altMinus.Width, altMinus.Height));
                }
            }

            Model[] a = new Model[5];
            a[0] = mountainModel;
            a[1] = treeModel;
            a[2] = waypointModel;
            a[3] = groundTileModel;
            a[4] = buildingModel;

            map = new Map(ScreenManager.Game, a, ScreenManager.GraphicsDevice);
            components.Add(map);

            Model[] w = new Model[1];
            w[0] = unitModelWarrior;

            Model[] r = new Model[1];
            r[0] = unitModelRanger;

            Model[] m = new Model[1];
            m[0] = unitModelMage;

            /*
            unit1 = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(-10, 0, -10), UnitType.Warrior, 1);
            unit2 = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(10, 0, 10), UnitType.Warrior, 1);
            components.Add(unit1);
            components.Add(unit2);
             * */

            #region TESTUNITREGION

            //for (int i = 0; i < 15; i++)
            //{
            //    for (int j = 0; j < 15; j++)
            //    {
            //        testUnitList.Add(UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(i, 0, j), UnitType.Warrior));
            //    }
            //}

            //foreach (Unit u in testUnitList)
            //{
            //    components.Add(u);
            //}

            #endregion

            Unit[] unitList1 = new Unit[6];
            unitList1[0] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(-30, 0, 45), UnitType.Warrior, 0);
            unitList1[1] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(-30, 0, 45), UnitType.Ranger, 0);
            unitList1[2] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(-30, 0, 45), UnitType.Ranger, 0);
            unitList1[3] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(-30, 0, 45), UnitType.Warrior, 0);
            unitList1[4] = UnitFactory.createUnit(ScreenManager.Game, m, new Vector3(-30, 0, 45), UnitType.Mage, 0);
            unitList1[5] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(-30, 0, 45), UnitType.Warrior, 0);

            squad1 = SquadFactory.createSquad(ScreenManager.Game, unitList1, SquadType.Pyramid);
            components.Add(squad1);

            Unit[] unitList2 = new Unit[5];
            unitList2[0] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(30, 0, -45), UnitType.Warrior, 1);
            unitList2[1] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(30, 0, -45), UnitType.Ranger, 1);
            unitList2[2] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(30, 0, -45), UnitType.Ranger, 1);
            unitList2[3] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(30, 0, -45), UnitType.Warrior, 1);
            unitList2[4] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(30, 0, -45), UnitType.Warrior, 1);

            squad2 = SquadFactory.createSquad(ScreenManager.Game, unitList2, SquadType.Pentagram);
            components.Add(squad2);

            Unit[] unitList3 = new Unit[3];
            unitList3[0] = UnitFactory.createUnit(ScreenManager.Game, m, new Vector3(-30, 0, -40), UnitType.Mage, 1);
            unitList3[1] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(-30, 0, -40), UnitType.Ranger, 1);
            unitList3[2] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(-30, 0, -40), UnitType.Ranger, 1);

            squad3 = SquadFactory.createSquad(ScreenManager.Game, unitList3, SquadType.Triangle);
            components.Add(squad3);

            //squad1.setTarget(unitList2[0]);
            //squad2.setTarget(unitList1[0]);
            
            float cellSize = 2.0f;
            grid = new SpatialHashGrid(map.size.X * cellSize, map.size.Y * cellSize, cellSize, -map.size.X / 2, map.size.Y / 2);
            for (int i = 0; i < map.staticObjects.Count; ++i)
                grid.insertStaticObject(map.staticObjects[i]);
            
            for (int i = 0; i < unitList1.Length; ++i)//for collisions
                grid.insertDynamicObject(unitList1[i]);

            for (int i = 0; i < unitList2.Length; ++i)//for collisions
                grid.insertDynamicObject(unitList2[i]);

            //grid.insertDynamicObject(unit1);
            //grid.insertDynamicObject(unit2);

            // Initialize our renderer
            DebugShapeRenderer.Initialize(ScreenManager.Game.GraphicsDevice);

            bfRenderer = new BoundingFrustumRenderer(new BoundingFrustum(camera.view * camera.projection), ScreenManager.Game);

            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                /*----- GAME UPDATE GOES HERE -----*/
                camera.Update(gameTime);

                #region Updates
                List<Object> colliders = new List<Object>();
                GameComponent[] gcc = new GameComponent[components.Count];
                components.CopyTo(gcc, 0);
                foreach (GameComponent gc in gcc)
                {
                    if (!(gc is Object))
                    {
                        gc.Update(gameTime);
                    }
                    else
                    {
                        Object o = (Object)gc;
                        // Only update if the object is alive
                        if (o.isAlive)
                        {
                            //colliders = grid.getPotentialColliders(o);
                            o.Update(gameTime, grid, map.waypointList);
                            //colliders.Clear();
                        }
                        else
                        {
                            components.Remove(o);
                            grid.removeDynamicObject(o);
                        }
                    }
                }
                #endregion
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            MouseState mouseState = input.CurrentMouseState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                /*----- INPUT HANDLING GOES HERE -----*/

                //If the user has just clicked the Left mouse button, then set the start location for the Selection box
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed & input.LastMouseState.LeftButton == ButtonState.Released)
                {
                    //Set the starting location for the selectiong box to the current location
                    //where the Left button was initially clicked.
                    mSelectionBox = new Rectangle(input.CurrentMouseState.X, input.CurrentMouseState.Y, 0, 0);
                }

                //If the user is still holding the Left button down, then continue to re-size the 
                //selection square based on where the mouse has currently been moved to.
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    //The starting location for the selection box remains the same, but increase (or decrease)
                    //the size of the Width and Height but taking the current location of the mouse minus the
                    //original starting location.
                    mSelectionBox = new Rectangle(mSelectionBox.X, mSelectionBox.Y, input.CurrentMouseState.X - mSelectionBox.X, input.CurrentMouseState.Y - mSelectionBox.Y);
                }

                //If the user has released the left mouse button, then reset the selection square
                if (input.CurrentMouseState.LeftButton == ButtonState.Released && input.LastMouseState.LeftButton == ButtonState.Pressed)
                {
                    selectedSquads = new List<Squad>();

                    if (mSelectionBox.Width == 0)
                    {
                        mSelectionBox.Width = 1;
                    }

                    if (mSelectionBox.Height == 0)
                    {
                        mSelectionBox.Height = 1;
                    }

                    BoundingFrustum bFrustrum = camera.UnprojectRectangle(mSelectionBox, ScreenManager.Game);

                    bfRenderer.Frustum = bFrustrum;
                    bfRenderer.Update();

                    foreach (GameComponent gc in components)
                    {
                        if (gc is Squad)
                        {
                            Squad s = (Squad)gc;
                            if (s.Intersects(bFrustrum))
                                selectedSquads.Add(s);
                        }
                    }

                    //Reset the selection square to no position with no height and width
                    mSelectionBox = new Rectangle(0, 0, 0, 0);
                     
                }

                if (input.CurrentMouseState.RightButton == ButtonState.Released && input.LastMouseState.RightButton == ButtonState.Pressed)
                {
                    Ray singleClick = camera.GetMouseRay(new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y), ScreenManager.Game);

                    foreach (GameComponent gc in components)
                    {
                        //Will need to remove this so that players cannot direct squads to other squads, they ahve to attack buildings
                        if (gc is Squad)
                        {
                            Squad sq = (Squad)gc;
                            if (sq.Intersects(singleClick))
                            {
                                foreach (Squad s in selectedSquads)
                                {
                                    s.setTarget(sq);
                                }
                            }
                        }
                        //End

                        if (gc is Object)
                        {
                            Object o = (Object)gc;
                            if (singleClick.Intersects(o.bounds).HasValue)
                            {
                                foreach (Squad s in selectedSquads)
                                {
                                    s.setTarget(o);
                                }
                            }
                        }
                    }
                }

            }
        }

        private void DrawSelectionBox(Rectangle box)
        {
            int spacing = 10;
            Vector2 dashSize = new Vector2(5, 2); //Length, thickness

            //Draw the horizontal portions of the selection box 
            DrawHorizontalLine(mSelectionBox.X, mSelectionBox.Y, box.Width, spacing, dashSize);
            DrawHorizontalLine(mSelectionBox.X, mSelectionBox.Y + mSelectionBox.Height, box.Width, spacing, dashSize);

            //Draw the verticla portions of the selection box 
            DrawVerticalLine(mSelectionBox.X, mSelectionBox.Y, box.Height, spacing, dashSize);
            DrawVerticalLine(mSelectionBox.X + mSelectionBox.Width, mSelectionBox.Y, box.Height, spacing, dashSize);
        }

        private void DrawHorizontalLine(int xCoord, int yCoord, int length, int spacing, Vector2 dashSize)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //When the width is greater than 0, the user is selecting an area to the right of the starting point
            if (length > 0)
            {
                //Draw the line starting at the startring location and moving to the right
                for (int aCounter = 0; aCounter <= length - (int)dashSize.X; aCounter += spacing)
                {
                    if (length - aCounter >= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, yCoord, (int)dashSize.X, (int)dashSize.Y), Color.White);
                    }
                }
            }
            //When the width is less than 0, the user is selecting an area to the left of the starting point
            else if (length < 0)
            {
                //Draw the line starting at the starting location and moving to the left
                for (int aCounter = -(int)dashSize.X; aCounter >= length; aCounter -= spacing)
                {
                    if (length - aCounter <= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(mSelectionBox.X + aCounter, yCoord, (int)dashSize.X, (int)dashSize.Y), Color.White);
                    }
                }
            }

        }

        private void DrawVerticalLine(int xCoord, int yCoord, int length, int spacing, Vector2 dashSize)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //When the height is greater than 0, the user is selecting an area below the starting point
            if (length > 0)
            {
                //Draw the line starting at the starting loctino and moving down
                for (int aCounter = 0; aCounter <= length; aCounter += spacing)
                {
                    if (length - aCounter >= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(xCoord, yCoord + aCounter, (int)dashSize.X, (int)dashSize.Y), new Rectangle(0, 0, mDottedLine.Width, mDottedLine.Height), Color.White, MathHelper.ToRadians(90), new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }
            //When the height is less than 0, the user is selecting an area above the starting point
            else if (length < 0)
            {
                //Draw the line starting at the start location and moving up
                for (int aCounter = -(int)dashSize.X; aCounter >= length; aCounter -= spacing)
                {
                    if (length - aCounter <= 0)
                    {
                        spriteBatch.Draw(mDottedLine, new Rectangle(xCoord, yCoord + aCounter, (int)dashSize.X, (int)dashSize.Y), new Rectangle(0, 0, mDottedLine.Width, mDottedLine.Height), Color.White, MathHelper.ToRadians(90), new Vector2(0, 0), SpriteEffects.None, 0);
                    }
                }
            }

        }

        private void drawUIElements()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Draw(squadCreate, squadCreateRec1, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(squadCreate, squadCreateRec2, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(squadCreate, squadCreateRec3, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(squadCreate, squadCreateRec4, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);

            foreach (Rectangle rec in altCreateRec)
            {
                spriteBatch.Draw(altCreate, rec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            }

            foreach (Rectangle rec in altPlusRec)
            {
                spriteBatch.Draw(altPlus, rec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            }

            foreach (Rectangle rec in altMinusRec)
            {
                spriteBatch.Draw(altMinus, rec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            /* ORIGINAL GameplayScreen Stuff
            spriteBatch.Begin();
            spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);
            spriteBatch.DrawString(gameFont, "Insert Gameplay Here",
                                   enemyPosition, Color.DarkRed);
            spriteBatch.End();
             * */

            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            map.Draw(camera);

            drawSelectedSquads();
            drawUIElements();

            GameComponent[] gcc = new GameComponent[components.Count];
            components.CopyTo(gcc, 0);
            foreach (GameComponent gc in gcc)
            {
                if (gc is Object)
                {
                    Object o = (Object)gc;
                    o.Draw(camera);
                }
                if (gc is DamageableObject)
                {
                    DamageableObject o = (DamageableObject)gc;
                    o.drawHealth(camera, spriteBatch, ScreenManager.GraphicsDevice, blankTexture);
                }
                if (gc is Squad)
                {
                    Squad o = (Squad)gc;
                    o.drawHealth(camera, spriteBatch, ScreenManager.GraphicsDevice, blankTexture);
                }
            }

            //bfRenderer.Draw(camera);
            DebugShapeRenderer.Draw(gameTime, camera.view, camera.projection);

            spriteBatch.Draw(userInterface, new Rectangle(0, 0, ScreenManager.Game.GraphicsDevice.Viewport.Width, ScreenManager.Game.GraphicsDevice.Viewport.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            DrawSelectionBox(mSelectionBox);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void drawSelectedSquads()
        {
            foreach (Squad s in selectedSquads)
            {
                Matrix[] transforms = new Matrix[selectionRingModel.Bones.Count];
                selectionRingModel.CopyAbsoluteBoneTransformsTo(transforms);

                Matrix pos = Matrix.CreateTranslation(s.position);

                foreach (ModelMesh mesh in selectionRingModel.Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.SpecularPower = 100f;
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = pos * mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }
            }
        }

        #endregion
    }
}
