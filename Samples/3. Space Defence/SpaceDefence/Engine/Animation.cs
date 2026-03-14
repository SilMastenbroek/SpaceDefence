using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence.Engine
{
    public class Animation
    {
        private Texture2D _spriteSheet;
        private int _frameCount;
        private int _currentFrame;

        // Time between each frame in seconds
        public float FrameSpeed { get; set; }
        private float _timeElapsed;

        public bool IsLooping { get; set; }
        public bool IsFinished { get; private set; }

        private int _frameWidth;
        private int _frameHeight;

        /// <summary>
        ///Creates a new animation
        /// </summary>
        /// <param name="texture">The sprite sheet texture</param>
        /// <param name="frameCount">Number of frames across the width of the sprite sheet</param>
        /// <param name="frameSpeed">Time per frame in seconds (e.g., 0.1f)</param>
        /// <param name="isLooping">Does the animation keep playing?</param>
        public Animation(Texture2D texture, int frameCount, float frameSpeed, bool isLooping)
        {
            _spriteSheet = texture;
            _frameCount = frameCount;
            FrameSpeed = frameSpeed;
            IsLooping = isLooping;

            // Calculate the width of 1 empty frame in the texture
            _frameWidth = _spriteSheet.Width / _frameCount;
            _frameHeight = _spriteSheet.Height;
        }

        public void Update(GameTime gameTime)
        {
            if (IsFinished) return;

            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Once enough time has passed, move on to the next frame
            if (_timeElapsed >= FrameSpeed)
            {
                _currentFrame++;
                _timeElapsed = 0f;

                // When you reach the last frame
                if (_currentFrame >= _frameCount)
                {
                    if (IsLooping)
                        _currentFrame = 0; // Restart
                    else
                    {
                        _currentFrame = _frameCount - 1; // Stay on the last frame
                        IsFinished = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, float scale)
        {
            if (IsFinished) return;

            // Calculate which part of the sprite sheet needs to be drawn
            Rectangle sourceRect = new Rectangle(
                _currentFrame * _frameWidth,
                0,
                _frameWidth,
                _frameHeight
            );

            Vector2 origin = new Vector2(_frameWidth / 2f, _frameHeight / 2f);

            spriteBatch.Draw(_spriteSheet, position, sourceRect, color, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
