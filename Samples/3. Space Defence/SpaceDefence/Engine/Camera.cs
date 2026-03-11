using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence.Engine
{
    /// <summary>
    /// 2D camera that follows the player via a transformation matrix
    /// Based on the pattern from:
    /// https://roguesharp.wordpress.com/2014/07/13/tutorial-5-creating-a-2d-camera-with-pan-and-zoom-in-monogame/
    /// </summary>
    public class Camera
    {
        private readonly GraphicsDevice _graphicsDevice;

        public Vector2 Position { get; private set; }
        public Matrix WorldTransform { get; private set; }

        public int WorldWidth { get; }
        public int WorldHeight { get; }

        public Camera(GraphicsDevice graphicsDevice, int worldWidth, int worldHeight)
        {
            _graphicsDevice = graphicsDevice;
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
        }

        public void Update(Vector2 targetPosition)
        {
            float halfW = _graphicsDevice.Viewport.Width / 2f;
            float halfH = _graphicsDevice.Viewport.Height / 2f;

            // Make sure the camera does not look outside the world
            float x = MathHelper.Clamp(targetPosition.X, halfW, WorldWidth - halfW);
            float y = MathHelper.Clamp(targetPosition.Y, halfH, WorldHeight - halfH);
            Position = new Vector2(x, y);

            // Shift the world so that the player is on the origin (0,0)
            Matrix tObj = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
            // Move to the center of the screen
            Matrix tCenter = Matrix.CreateTranslation(
                _graphicsDevice.Viewport.Width / 2f,
                _graphicsDevice.Viewport.Height / 2f,
                0);

            WorldTransform = tObj * tCenter;
        }

        /// <summary>
        /// Convert a screen coordinate to a world coordinate.
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(WorldTransform));
        }
    }
}
