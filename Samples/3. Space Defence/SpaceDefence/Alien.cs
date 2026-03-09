using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceDefence
{
    internal class Alien : GameObject
    {
        private CircleCollider _circleCollider;
        private Texture2D _texture;
        private float playerClearance = 100;
        private const float Friction = 2f;
        private const float PredictionTime = 0.5f;
        private const float SpeedIncrease = 20f;
        private float _accelerationSpeed = 150f;
        private Vector2 _velocity = Vector2.Zero;
        private Vector2 _acceleration = Vector2.Zero;

        public Alien()
        {

        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _texture = content.Load<Texture2D>("Alien");
            _circleCollider = new CircleCollider(Vector2.Zero, _texture.Width / 2);
            SetCollider(_circleCollider);
            RandomMove();
        }

        public override void OnCollision(GameObject other)
        {
            _accelerationSpeed += SpeedIncrease;
            RandomMove();
            base.OnCollision(other);
        }

        public void RandomMove()
        {
            GameManager gm = GameManager.GetGameManager();
            _circleCollider.Center = gm.RandomScreenLocation();

            Vector2 centerOfPlayer = gm.Player.GetPosition().Center.ToVector2();
            while ((_circleCollider.Center - centerOfPlayer).Length() < playerClearance)
                _circleCollider.Center = gm.RandomScreenLocation();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameManager gm = GameManager.GetGameManager();

            // Predict where the player will be
            Vector2 predictedPos = gm.Player.GetPosition().Center.ToVector2() + gm.Player.Velocity * PredictionTime;

            // Steer towards predicted player position
            _acceleration = LinePieceCollider.GetDirection(_circleCollider.Center, predictedPos);

            // Apply acceleration and friction
            _velocity += _acceleration * _accelerationSpeed * dt;
            _velocity -= _velocity * Friction * dt;

            // Move alien
            _circleCollider.Center += _velocity * dt;

            // Check if alien is too close to the player
            Vector2 playerPos = gm.Player.GetPosition().Center.ToVector2();
            if ((_circleCollider.Center - playerPos).Length() < playerClearance)
                gm.Game.Exit();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _circleCollider.GetBoundingBox(), Color.White);
            base.Draw(gameTime, spriteBatch);
        }


    }
}
