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
        Unit unit1;

        Model floorModel, buildingModel, treeModel, unitModelWarrior, unitModelRanger, unitModelMage;

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
            floorModel = ScreenManager.Game.Content.Load<Model>(@"Models\floorModel");
            buildingModel = ScreenManager.Game.Content.Load<Model>(@"Models\buildingModel");
            treeModel = ScreenManager.Game.Content.Load<Model>(@"Models\treeModel");

            unitModelWarrior = ScreenManager.Game.Content.Load<Model>(@"Models\unitModelWarrior");
            unitModelRanger = ScreenManager.Game.Content.Load<Model>(@"Models\unitModelRanger");
            unitModelMage = ScreenManager.Game.Content.Load<Model>(@"Models\unitModelMage");

            Model waypointModel = ScreenManager.Game.Content.Load<Model>(@"Models\waypointModel");

            Model[] a = new Model[4];
            a[0] = floorModel;
            a[1] = buildingModel;
            a[2] = treeModel;
            a[3] = waypointModel;

            map = new Map(ScreenManager.Game, a, ScreenManager.GraphicsDevice);
            components.Add(map);

            Model[] w = new Model[1];
            w[0] = unitModelWarrior;

            Model[] r = new Model[1];
            r[0] = unitModelRanger;

            Model[] m = new Model[1];
            m[0] = unitModelMage;

            unit1 = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(30, 0, 30), UnitType.Warrior);
            components.Add(unit1);

            Unit[] unitList = new Unit[6];
            unitList[0] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(0, 0, 0), UnitType.Warrior);
            unitList[1] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(0, 0, 0), UnitType.Ranger);
            unitList[2] = UnitFactory.createUnit(ScreenManager.Game, r, new Vector3(0, 0, 0), UnitType.Ranger);
            unitList[3] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(0, 0, 0), UnitType.Warrior);
            unitList[4] = UnitFactory.createUnit(ScreenManager.Game, m, new Vector3(0, 0, 0), UnitType.Mage);
            unitList[5] = UnitFactory.createUnit(ScreenManager.Game, w, new Vector3(0, 0, 0), UnitType.Warrior);

            Squad squad = SquadFactory.createSquad(ScreenManager.Game, unitList, SquadType.Pyramid);
            squad.setTarget(unit1);
            components.Add(squad);

            float cellSize = 2.0f;
            grid = new SpatialHashGrid(map.size.X * cellSize, map.size.Y * cellSize, cellSize, -map.size.X / 2, map.size.Y / 2);
            for (int i = 0; i < map.staticObjects.Count; ++i)
                grid.insertStaticObject(map.staticObjects[i]);
            /*
            for (int i = 0; i < map.usableBuildings.Count; ++i)//for collisions
                grid.insertStaticObject(map.usableBuildings[i]);
            */

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

            GameComponent[] gcc = new GameComponent[components.Count];
            components.CopyTo(gcc, 0);
            foreach (GameComponent gc in gcc)
            {
                if (gc is Object)
                {
                    Object o = (Object)gc;
                    o.Draw(camera);
                }
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
