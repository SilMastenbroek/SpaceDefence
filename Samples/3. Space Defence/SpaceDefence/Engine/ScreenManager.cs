using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence.Engine
{
    public class ScreenManager
    {
        private GameState _currentState = GameState.MainMenu;
        private SpriteFont _font;
        private Texture2D _overlayTexture;
        private GraphicsDevice _graphicsDevice;
        private InputManager _inputManager;

        // Add this timer variable to wait after the player dies
        private double _deathTimer = 1.0f;
        private bool _isDying = false;

        public GameState CurrentState => _currentState;

        public ScreenManager()
        {
            _inputManager = new InputManager();
        }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _font = content.Load<SpriteFont>("GameFont");

            // Create a 1x1 white pixel texture, to draw colored rectangles
            _overlayTexture = new Texture2D(graphicsDevice, 1, 1);
            _overlayTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            _inputManager.Update();

            // When we're playing, and the player has just died
            if (_currentState == GameState.Playing && GameManager.GetGameManager().IsGameOver == true && !_isDying)
                _isDying = true;

            // Count down so we see the explosion first, and end the game if the time runs out
            if (_isDying)
            {
                _deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                if (_deathTimer <= 0)
                {
                    _isDying = false;
                    _deathTimer = 1.25f;
                    _currentState = GameState.GameOver;
                }
            }

            switch (_currentState)
            {
                case GameState.MainMenu:
                    if (_inputManager.IsKeyPress(Keys.Enter))
                        _currentState = GameState.Playing;
                    if (_inputManager.IsKeyPress(Keys.Q))
                        GameManager.GetGameManager().Game.Exit();
                    break;

                case GameState.Playing:
                    if (_inputManager.IsKeyPress(Keys.Escape) && !_isDying)
                        _currentState = GameState.Paused;
                    break;

                case GameState.Paused:
                    if (_inputManager.IsKeyPress(Keys.Enter))
                        _currentState = GameState.Playing;
                    if (_inputManager.IsKeyPress(Keys.Q))
                        GameManager.GetGameManager().Game.Exit();
                    break;

                case GameState.GameOver:
                    if (_inputManager.IsKeyPress(Keys.Enter))
                    {
                        GameManager.GetGameManager().ResetGame();
                        _currentState = GameState.Playing;
                    }
                    if (_inputManager.IsKeyPress(Keys.Q))
                        GameManager.GetGameManager().Game.Exit();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (_currentState)
            {
                case GameState.MainMenu:
                    DrawMenu(spriteBatch);
                    break;
                case GameState.Paused:
                    DrawPause(spriteBatch);
                    break;
                case GameState.GameOver:
                    DrawGameOver(spriteBatch);
                    break;
            }
        }

        private void DrawMenu(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(
                _graphicsDevice.Viewport.Width / 2f,
                _graphicsDevice.Viewport.Height / 2f);

            spriteBatch.Begin();

            // Black background
            spriteBatch.Draw(_overlayTexture, _graphicsDevice.Viewport.Bounds, Color.Black);

            // Text
            DrawCenteredText(spriteBatch, "SPACE DEFENCE", center + new Vector2(0, -120), Color.White, 3f);
            DrawCenteredText(spriteBatch, "Press ENTER to start", center, Color.LightGreen, 1.2f);
            DrawCenteredText(spriteBatch, "Press Q to quit the game", center + new Vector2(0, 60), Color.OrangeRed, 1.2f);

            spriteBatch.End();
        }

        private void DrawPause(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(
                _graphicsDevice.Viewport.Width / 2f,
                _graphicsDevice.Viewport.Height / 2f);

            spriteBatch.Begin();

            // Semi-transparent dark overlay over the game 
            spriteBatch.Draw(_overlayTexture, _graphicsDevice.Viewport.Bounds, Color.Black * 0.6f);

            // Text
            DrawCenteredText(spriteBatch, "PAUSED", center + new Vector2(0, -120), Color.White, 3f);
            DrawCenteredText(spriteBatch, "Press ENTER to continue", center, Color.LightGreen, 1.2f);
            DrawCenteredText(spriteBatch, "Press Q to quit the game", center + new Vector2(0, 60), Color.OrangeRed, 1.2f);

            spriteBatch.End();
        }

        private void DrawGameOver(SpriteBatch spriteBatch)
        {
            Vector2 center = new Vector2(
                _graphicsDevice.Viewport.Width / 2f,
                _graphicsDevice.Viewport.Height / 2f);

            spriteBatch.Begin();

            // Red overlay (0.6f = semi-transparant)
            spriteBatch.Draw(_overlayTexture, _graphicsDevice.Viewport.Bounds, Color.DarkRed * 0.6f);

            // Tekst
            DrawCenteredText(spriteBatch, "GAME OVER", center + new Vector2(0, -120), Color.Red, 4f);
            DrawCenteredText(spriteBatch, "Press ENTER to restart", center, Color.LightGreen, 1.2f);
            DrawCenteredText(spriteBatch, "Press Q to quit the game", center + new Vector2(0, 60), Color.Yellow, 1.2f);

            spriteBatch.End();
        }

        private void DrawCenteredText(SpriteBatch spriteBatch, string text, Vector2 center, Color color, float scale)
        {
            // https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteFont.html#Microsoft_Xna_Framework_Graphics_SpriteFont_MeasureString_System_String_
            Vector2 textSize = _font.MeasureString(text) * scale;
            spriteBatch.DrawString(_font, text, center - textSize / 2f, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
