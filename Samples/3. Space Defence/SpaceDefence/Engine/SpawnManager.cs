using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDefence.Engine
{
    public class SpawnManager
    {
        private GameManager _gameManager;

        // Timers in seconds
        private double _alienSpawnTimer;
        private double _asteroidSpawnTimer;

        // How long it takes between spawns
        private double _alienSpawnInterval = 5f;
        private double _asteroidSpawnInterval = 10f;

        // The shorter this interval becomes, the faster they come (difficulty ramp)
        private const double MinAlienInterval = 0.5f;
        private const double MinAsteroidInterval = 1f;

        // How much time we save per spawned enemywwwww
        private const double DifficultyRamp = 0.05f;

        public SpawnManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            _alienSpawnTimer = _alienSpawnInterval;
            _asteroidSpawnTimer = _asteroidSpawnInterval;
        }

        public void Update(GameTime gameTime)
        {
            double elapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds;

            // Alien Countdown Timer
            _alienSpawnTimer -= elapsedSeconds;
            if (_alienSpawnTimer <= 0)
                SpawnAlien();

            // Asteroid Countdown Timer
            _asteroidSpawnTimer -= elapsedSeconds;
            if (_asteroidSpawnTimer <= 0)
                SpawnAsteroid();
        }

        private void SpawnAlien()
        {
            _gameManager.AddGameObject(new Alien());

            // Increase the difficulty: speed up the spawn time, but not faster than the minimum.
            _alienSpawnInterval = Math.Max(MinAlienInterval, _alienSpawnInterval - DifficultyRamp);

            // Reset the timer
            _alienSpawnTimer = _alienSpawnInterval;
        }

        private void SpawnAsteroid()
        {
            _gameManager.AddGameObject(new Asteroid());

            // Increase the difficulty for the asteroids.
            _asteroidSpawnInterval = Math.Max(MinAsteroidInterval, _asteroidSpawnInterval - DifficultyRamp);

            // Reset the timer
            _asteroidSpawnTimer = _asteroidSpawnInterval;
        }
    }
}
