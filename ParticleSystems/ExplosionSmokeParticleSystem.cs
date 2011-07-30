#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Beasty.ParticleSystems
{
    /// <summary>
    /// Custom particle system for creating the smokey part of the explosions.
    /// </summary>
    class ExplosionSmokeParticleSystem : ParticleSystem
    {
        public ExplosionSmokeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "textures/particles/smoke";

            settings.MaxParticles = 200;

            settings.Duration = TimeSpan.FromSeconds(4);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 50;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 50;

            settings.Gravity = new Vector3(0, -20, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.LightGray;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            settings.MinStartSize = 10;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 100;
            settings.MaxEndSize = 200;
        }
    }
}
