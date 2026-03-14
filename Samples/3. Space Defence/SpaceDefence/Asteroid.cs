using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence
{
    public class Asteroid : GameObject
    {
        private Texture2D _texture;
        private CircleCollider _circleCollider;

        // Determine how much distance there should be between the player and the spawn location
        private float playerClearance = 300f;

        public Asteroid()
        {
        }

        public override void Load(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Asteroid");

            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);

            RandomMove();
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            // Make sure the asteroid doesn't spawn directly in front of the player
            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void OnCollision(GameObject other)
        {
            // If it collides with the Player
            if (other is Ship)
                GameManager.GetGameManager().Game.Exit();

            // If it collides with an Alien
            else if (other is Alien)
                GameManager.GetGameManager().RemoveGameObject(other);

            base.OnCollision(other);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the asteroid. Its position is the bounding box of the circleCollider.
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
