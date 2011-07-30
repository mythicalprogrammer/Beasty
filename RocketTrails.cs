#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
#endregion

namespace Beasty
{
    class RocketTrails
    {
        #region Private Variables

        //ParticleSystem
        private ParticleSystem explosionParticles;
        private ParticleSystem explosionSmokeParticles;
        private ParticleSystem projectileTrailParticles;
        private ParticleSystem smokePlumeParticles;
        private ParticleSystem fireParticles;

        // The explosions effect works by firing projectiles up into the
        // air, so we need to keep track of all the active projectiles.
        protected List<Projectile> projectilesSystemList = new List<Projectile>();

        private TimeSpan timeToNextProjectile = TimeSpan.Zero;


        // Random number generator for the fire effect.
        private Random random = new Random();

        //Game
        private Game givenGame;

        //position
        private Vector3 position; 

        //camera
        private Vector3 cameraLookAt = new Vector3();

        #endregion

        public RocketTrails(Game game, ContentManager Content)
        {
            givenGame = game;

            explosionParticles = new Beasty.ParticleSystems.ExplosionParticleSystem(game, Content);
            explosionSmokeParticles = new Beasty.ParticleSystems.ExplosionSmokeParticleSystem(game, Content);
            projectileTrailParticles = new Beasty.ParticleSystems.ProjectileTrailParticleSystem(game, Content);
            //smokePlumeParticles = new Beasty.ParticleSystems.SmokePlumeParticleSystem(game, Content);
            //fireParticles = new Beasty.ParticleSystems.FireParticleSystem(game, Content);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 10;
            explosionSmokeParticles.DrawOrder = 20;
            projectileTrailParticles.DrawOrder = 30;
            explosionParticles.DrawOrder = 40;
            fireParticles.DrawOrder = 50;

            // Register the particle system components.
            game.Components.Add(explosionParticles);
            game.Components.Add(explosionSmokeParticles);
            game.Components.Add(projectileTrailParticles);
            game.Components.Add(smokePlumeParticles);
            game.Components.Add(fireParticles);

        }

        public void Update(Vector3 position, GameTime gameTime, float aspectRatio, Matrix viewTemp, Matrix projectionTemp)
        {
            this.position = position;

            UpdateExplosions(gameTime);
            UpdateProjectiles(gameTime);


            // Pass camera matrices through to the particle system components.
            explosionParticles.SetCamera(viewTemp, projectionTemp);
            explosionSmokeParticles.SetCamera(viewTemp, projectionTemp);
            projectileTrailParticles.SetCamera(viewTemp, projectionTemp);
            smokePlumeParticles.SetCamera(viewTemp, projectionTemp);
            fireParticles.SetCamera(viewTemp, projectionTemp);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            timeToNextProjectile -= gameTime.ElapsedGameTime;

            if (timeToNextProjectile <= TimeSpan.Zero)
            {
                // Create a new projectile once per second. The real work of moving
                // and creating particles is handled inside the Projectile class.
                projectilesSystemList.Add(new Projectile(explosionParticles,
                                               explosionSmokeParticles,
                                               projectileTrailParticles));

                timeToNextProjectile += TimeSpan.FromSeconds(1);
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            int i = 0;

            while (i < projectilesSystemList.Count)
            {
                if (!projectilesSystemList[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    projectilesSystemList.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }


    }
}
