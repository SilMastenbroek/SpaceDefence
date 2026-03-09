using System;
using SpaceDefence.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceDefence
{
    public class Ship : GameObject
    {
        private Texture2D ship_body;
        private Texture2D base_turret;
        private Texture2D laser_turret;
        private float buffTimer = 100;
        private float buffDuration = 10f;
        private RectangleCollider _rectangleCollider;
        private Point target;

        private Vector2 _velocity = Vector2.Zero;
        public Vector2 Velocity => _velocity;
        private Vector2 _acceleration = Vector2.Zero;
        private const float AccelerationSpeed = 400f;
        private const float SpeedMult = 5f;
        private const float Friction = 4f;
        private Vector2 _position;
        private float _shipAngle = 0;

        /// <summary>
        /// The player character
        /// </summary>
        /// <param name="Position">The ship's starting position</param>
        public Ship(Point Position)
        {
            _position = Position.ToVector2();
            _rectangleCollider = new RectangleCollider(new Rectangle(Position, Point.Zero));
            SetCollider(_rectangleCollider);
        }

        public override void Load(ContentManager content)
        {
            // Ship sprites from: https://zintoki.itch.io/space-breaker
            ship_body = content.Load<Texture2D>("ship_body");
            base_turret = content.Load<Texture2D>("base_turret");
            laser_turret = content.Load<Texture2D>("laser_turret");
            _rectangleCollider.shape.Size = ship_body.Bounds.Size;
            _rectangleCollider.shape.Location -= new Point(ship_body.Width/2, ship_body.Height/2);
            base.Load(content);
        }



        public override void HandleInput(InputManager inputManager)
        {
            base.HandleInput(inputManager);
            target = inputManager.CurrentMouseState.Position;

            // WASD movement input
            _acceleration = Vector2.Zero;
            if (inputManager.IsKeyDown(Keys.W))
                _acceleration.Y -= SpeedMult;
            if (inputManager.IsKeyDown(Keys.A)) 
                _acceleration.X -= SpeedMult;
            if (inputManager.IsKeyDown(Keys.S))
                _acceleration.Y += SpeedMult;
            if (inputManager.IsKeyDown(Keys.D)) 
                _acceleration.X += SpeedMult;
            if (_acceleration != Vector2.Zero)
            {
                _acceleration.Normalize();
                _acceleration *= SpeedMult;
            }

            if(inputManager.LeftMousePress())
            {

                Vector2 aimDirection = LinePieceCollider.GetDirection(GetPosition().Center, target);
                Vector2 turretExit = _rectangleCollider.shape.Center.ToVector2() + aimDirection * base_turret.Height / 2f;
                if (buffTimer <= 0)
                {
                    GameManager.GetGameManager().AddGameObject(new Bullet(turretExit, aimDirection, 150));
                }
                else
                {
                    GameManager.GetGameManager().AddGameObject(new Laser(new LinePieceCollider(turretExit, target.ToVector2()),400));
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the Buff timer
            if (buffTimer > 0)
                buffTimer -= dt;

            // Apply acceleration and friction
            _velocity += _acceleration * AccelerationSpeed * dt;
            _velocity -= _velocity * Friction * dt;

            if (_velocity != Vector2.Zero)
                _shipAngle = LinePieceCollider.GetAngle(_velocity);

            // Move position
            _position += _velocity * dt;

            // Sync collider to position
            _rectangleCollider.shape.Location = _position.ToPoint();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = ship_body.Bounds.Size.ToVector2() / 2f;
            spriteBatch.Draw(ship_body, _rectangleCollider.shape.Center.ToVector2(), null, Color.White, _shipAngle, origin, 1f, SpriteEffects.None, 0);
            float aimAngle = LinePieceCollider.GetAngle(LinePieceCollider.GetDirection(GetPosition().Center, target));
            if (buffTimer <= 0)
            {
                Rectangle turretLocation = base_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(base_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            else
            {
                Rectangle turretLocation = laser_turret.Bounds;
                turretLocation.Location = _rectangleCollider.shape.Center;
                spriteBatch.Draw(laser_turret, turretLocation, null, Color.White, aimAngle, turretLocation.Size.ToVector2() / 2f, SpriteEffects.None, 0);
            }
            base.Draw(gameTime, spriteBatch);
        }


        public void Buff()
        {
            buffTimer = buffDuration;
        }

        public Rectangle GetPosition()
        {
            return _rectangleCollider.shape;
        }
    }
}
