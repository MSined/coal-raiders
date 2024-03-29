#region File Description
//-----------------------------------------------------------------------------
// TutorialMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace coal_raider
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class TutorialMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry ungulateMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry frobnicateMenuEntry;
        MenuEntry elfMenuEntry;
        MenuEntry difficultyMenuEntry;

        AI.Difficulty difficulty;

        Texture2D tutorial;

        enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            Llama,
        }

        static Ungulate currentUngulate = Ungulate.Dromedary;

        static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int currentLanguage = 0;

        static string[] difficultyText = { "Easy", "Medium", "Hard" };
        static int currentDifficulty = 0;

        static bool frobnicate = true;

        static int elf = 23;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public TutorialMenuScreen(ScreenManager sm)
            : base("Tutorial")
        {
            // Create our menu entries.
            //ungulateMenuEntry = new MenuEntry(string.Empty);
            //languageMenuEntry = new MenuEntry(string.Empty);
            //frobnicateMenuEntry = new MenuEntry(string.Empty);
            //elfMenuEntry = new MenuEntry(string.Empty);

            //difficultyMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            //ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
            //languageMenuEntry.Selected += LanguageMenuEntrySelected;
            //frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
            //elfMenuEntry.Selected += ElfMenuEntrySelected;
            //difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            //MenuEntries.Add(ungulateMenuEntry);
            //MenuEntries.Add(languageMenuEntry);
            //MenuEntries.Add(frobnicateMenuEntry);
            //MenuEntries.Add(elfMenuEntry);
            //MenuEntries.Add(difficultyMenuEntry);
            MenuEntries.Add(back);

            //difficulty = AI.Difficulty.Easy;
            //MainMenuScreen.difficulty = difficulty;

            tutorial = sm.Game.Content.Load<Texture2D>("tutorial");
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            //ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
            //languageMenuEntry.Text = "Language: " + languages[currentLanguage];
            //frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            //elfMenuEntry.Text = "elf: " + elf;
            //difficultyMenuEntry.Text = "Difficulty: " + difficultyText[currentDifficulty];
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;

            if (currentUngulate > Ungulate.Llama)
                currentUngulate = 0;

            SetMenuEntryText();
        }

        void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentDifficulty = (currentDifficulty + 1) % difficultyText.Length;
            if (currentDifficulty == 0)
            {
                difficulty = AI.Difficulty.Easy;
                MainMenuScreen.difficulty = difficulty;
            }
            else if (currentDifficulty == 1)
            {
                difficulty = AI.Difficulty.Medium;
                MainMenuScreen.difficulty = difficulty;
            }
            else if (currentDifficulty == 2)
            {
                difficulty = AI.Difficulty.Hard;
                MainMenuScreen.difficulty = difficulty;
            }

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            frobnicate = !frobnicate;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            elf++;

            SetMenuEntryText();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(tutorial, new Vector2(640 - tutorial.Width / 2, 150), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion
    }
}
