using System;
using particlex.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using particlex.Core.Configuration;
using particlex.Core.Simulation;
using static System.Net.Mime.MediaTypeNames;

namespace particlex.Core
{
    public class particlexGame : Game
    {
        // todo: move to configuration
        private const int GRID_SIZE = 2; // px

        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;
        private Simulation.Simulation Simulation;
        private Ui.Ui Ui;
        public SpriteFont Font;
        private Texture2D baseTexture;
        private Configuration.Model.Configuration Configuration;
        
        private MouseState LastMouseState;
        private KeyboardState LastKeyboardState;
        
        private int BrushRadius = 2; // px

        public particlexGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);
            graphicsDeviceManager.SupportedOrientations =
                DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            
            // todo: move resolution to configuration
            int w = 800 / GRID_SIZE;
            int h = 600 / GRID_SIZE;

            Simulation = new Simulation.Simulation(w, h);
            Simulation.CreateBorder();
        }

        protected override void Initialize()
        {
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.PreferredBackBufferWidth = 800;
            graphicsDeviceManager.PreferredBackBufferHeight = 600;
            // graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            graphicsDeviceManager.ApplyChanges();
            
            base.Initialize();
            IsMouseVisible = true;
            
            baseTexture = new Texture2D(GraphicsDevice, 1, 1);
            baseTexture.SetData(new[] {Color.White});
            
            var configurationLoader = new ConfigurationLoader();
            Configuration = configurationLoader.LoadConfiguration();
            
            Ui = new Ui.Ui(Configuration);

            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }
            
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            
            Font = Content.Load<SpriteFont>("Fonts/Hud");
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();
            
            if (keyboardState.IsKeyDown(Keys.OemPlus) && !LastKeyboardState.IsKeyDown(Keys.OemPlus))
            {
                BrushRadius++;
                Console.WriteLine("Brush radius: " + BrushRadius);
            }
            
            if (keyboardState.IsKeyDown(Keys.OemMinus) && !LastKeyboardState.IsKeyDown(Keys.OemMinus))
            {
                BrushRadius--;
                Console.WriteLine("Brush radius: " + BrushRadius);
            }

            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X / GRID_SIZE;
            int y = mouseState.Y / GRID_SIZE;

            if (x > 0 && y > 0 && x <= Simulation.Grid.GetWidth() && y <= Simulation.Grid.GetHeight())
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    for (int drawX = x - BrushRadius; drawX < x + BrushRadius; drawX++)
                    {
                        for (int drawY = y - BrushRadius; drawY < y + BrushRadius; drawY++)
                        {
                            Simulation.Grid.SetCellType(drawX, drawY, Ui.SelectedCellType);
                        }
                    }
                }
            
                if (mouseState.ScrollWheelValue > LastMouseState.ScrollWheelValue)
                {
                    var newCellType = Enum.GetValues(typeof(CellType)).Cast<CellType>()
                        .SkipWhile(e => e != Ui.SelectedCellType).Skip(1).FirstOrDefault();

                    if (newCellType == null)
                    {
                        newCellType = CellType.Solid;
                    }
                
                    Ui.SelectedCellType = newCellType;
                }
            }

            LastMouseState = mouseState;
            LastKeyboardState = keyboardState;
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Simulation.Update();
            watch.Stop();
            // Console.WriteLine("Simulation time: " + watch.ElapsedMilliseconds + "ms");

            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            for (int x = 0; x < Simulation.Grid.GetWidth(); x++)
            {
                for (int y = 0; y < Simulation.Grid.GetHeight(); y++)
                {
                    var cell = Simulation.Grid.GetCell(x, y);
                    if (cell != null)
                    {
                        var color = Configuration.GetColorForCellType(cell.Type);
                    
                        spriteBatch.Draw(baseTexture, new Rectangle(GRID_SIZE * x, GRID_SIZE * y, GRID_SIZE, GRID_SIZE), color);
                    }
                }
            }

            var framerate = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
            
            spriteBatch.DrawString(Font, "FPS: " + framerate.ToString(), Vector2.Zero, Color.MonoGameOrange);
            
            Ui.Draw(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
            
            watch.Stop();
            // Console.WriteLine("Draw time: " + watch.ElapsedMilliseconds + "ms");
        }
    }
}