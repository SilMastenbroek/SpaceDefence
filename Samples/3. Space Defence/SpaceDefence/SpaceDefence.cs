using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceDefence.Engine;
namespace SpaceDefence
{
    public class SpaceDefence : Game
    {
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private GameManager _gameManager;
        private ScreenManager _screenManager;

        public SpaceDefence()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;

            // Set the size of the screen
            _graphics.PreferredBackBufferWidth = 2000;
            _graphics.PreferredBackBufferHeight = 1200;
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Initialize the GameManager
            _gameManager = GameManager.GetGameManager();
            _screenManager = new ScreenManager();
            base.Initialize();

            // Place the player at the center of the screen
            Ship player = new Ship(new Point(GameManager.WorldWidth / 2, GameManager.WorldHeight / 2));

            // Add the starting objects to the GameManager
            _gameManager.Initialize(Content, this, player);
            _gameManager.AddGameObject(player);
            _gameManager.AddGameObject(new Alien());
            _gameManager.AddGameObject(new Asteroid());
            _gameManager.AddGameObject(new Supply());
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager.Load(Content);
            _screenManager.Load(Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Always update ScreenManager first, so it can handle input and change the game state if needed.
            _screenManager.Update();

            if (_screenManager.CurrentState == GameState.Playing)
            {
                // Update the game only when we are playing
                _gameManager.Update(gameTime);
            }
            else
            {
                // Update input anyway so that there is no backlog when resuming
                _gameManager.InputManager.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Always draw the game, even as a background during breaks
            if (_screenManager.CurrentState != GameState.MainMenu)
                _gameManager.Draw(gameTime, _spriteBatch);

            // Draw the menu or pause screen on top of the game
            _screenManager.Draw(_spriteBatch);

            base.Draw(gameTime);
        }



    }
}
