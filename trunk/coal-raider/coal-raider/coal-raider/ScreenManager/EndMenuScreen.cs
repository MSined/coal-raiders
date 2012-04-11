#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace coal_raider
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class EndMenuScreen : MenuScreen
    {
        #region Initialization

        string winner;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EndMenuScreen(string winner)
            : base("")
        {
            // Create our menu entries.
            //MenuEntry restartGameMenuEntry = new MenuEntry("Restart Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            // Hook up menu event handlers.
            //restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            //MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            this.winner = winner;

            MediaPlayer.Pause();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //const string message = "Are you sure you want to quit this game?";

            //MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            //confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            //ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen(ScreenManager));
        }

        void RestartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //const string message = "Are you sure you want to quit this game?";

            //MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            //confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            //ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);

            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen(ScreenManager));
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen(ScreenManager));
        }


        #endregion

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(winner) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, winner, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
