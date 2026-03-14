using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpaceDefence.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence
{
    internal class Explosion : GameObject
    {
        private Animation _animation;
        private Vector2 _position;
        private float _scale;

        public Explosion(Vector2 position, float scale = 1f)
        {
            _position = position;
            _scale = scale;
        }

        public override void Load(ContentManager content)
        {
            Texture2D spriteSheet = content.Load<Texture2D>("explosion_spritesheet_16frames");

            // Parameters: Texture, number of frames (e.g., 5), speed (e.g., 0.05 seconds per frame), looping (false)
            _animation = new Animation(spriteSheet, 16, 0.05f, false);

            base.Load(content);
        }

        public override void Update(GameTime gameTime)
        {
            // Let the timer run in the animation
            _animation.Update(gameTime);

            // Once the last image has finished loading, destroy this GameObject
            if (_animation.IsFinished)
                GameManager.GetGameManager().RemoveGameObject(this);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch, _position, Color.White, 0f, _scale);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
