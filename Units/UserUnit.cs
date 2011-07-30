using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Beasty.Units
{
    /// <summary>
    /// A UserUnit is a unit that the user controls.
    /// It is different from projectiles as it displays an aimer, life and energy bars..
    /// </summary>
    class UserUnit : BaseUnit
    {
        // factory used to create projectiles.
        // TODO: do we need this here?
        //  or is this a part of weapons class?
        protected AbstractUnitFactory factory;
        protected ContentManager content;

        private Model aimerModel;
        private Model bar;

        protected float fireEnergy = 1.0f;
        protected int coolDown = 500;
        protected int lastFire = 0;

        // Where the aimer is located. { current, minimum, maximum }
        private Vector2[] aimerPosition = new Vector2[3] { new Vector2(100f, 100f), new Vector2(50f, 50f), new Vector2(400f, 400f) };

        public UserUnit(ContentManager cm, AbstractUnitFactory f, Model def_body, Vector2 def_pos)
            : base(def_body, def_pos)
        {
            this.content = cm;
            this.factory = f;

            aimerModel = content.Load<Model>("models/box");
            bar = content.Load<Model>("models/box");
        }

        public override void Fire()
        {
            if(lastFire < 0 && _energy > 0)
            {
                // draw the aimer.
                Vector2 aimpos = aimerPosition[0];
                if (this.facing == Facing.Left)
                { aimpos.X = -aimpos.X; }
                
                // creates a projectile!
                factory.CreateProjectile(ProjectileType.Bullet, _position + new Vector2(0, 10), (aimpos / 10) + _velocity);
                _energy -= (int)(aimpos*2).Length();
                lastFire = coolDown;
            }
        }

        internal override void Update(GameTime time)
        {
            lastFire -= (int) time.ElapsedGameTime.TotalMilliseconds;
            base.Update(time);
        }

        public override void MoveAimer(Vector2 ForceDirection)
        {
            if (facing == Facing.Left)
            {
                ForceDirection.X = -ForceDirection.X;
            }
            aimerPosition[0].X = Math.Max(Math.Min(3*ForceDirection.X + aimerPosition[0].X, aimerPosition[2].X), aimerPosition[1].X);
            aimerPosition[0].Y = Math.Max(Math.Min(3*ForceDirection.Y + aimerPosition[0].Y, aimerPosition[2].Y), aimerPosition[1].Y);
        }

        private void DrawBar(Matrix projection, Matrix view, float ypos, float curr, float max, Color posColor, Color negColor)
        {
            Color color = posColor;
            if (curr < 0)
            {
                color = negColor;
                curr = -curr;
            }
            
            float amount = (curr / max) * 4;
            float startx = amount * 25 - 100f;
            Vector3 pos = new Vector3(this.position.X + startx, position.Y + ypos, 0f);
            DrawModel(bar, projection, view, pos, Vector3.Zero, new Vector3(amount, 1f, 1f), color.ToVector3());
        }

        public override void Draw(Matrix projection, Matrix view)
        {

            DrawBar(projection, view, 180, _life, _lifeMax, Color.Green, Color.Red);
            DrawBar(projection, view, 250, _energy, _energyMax, Color.Blue, Color.Red);
            
            // draw the aimer.
            Vector2 aimpos = aimerPosition[0];
            if (this.facing == Facing.Left)
            { aimpos.X = -aimpos.X; }
            Vector3 pos = new Vector3(this.position + aimpos, 0f);
            DrawModel(this.aimerModel, projection, view, pos, Vector3.Zero, 1.0f, new Vector3(1,1,1));

            base.Draw(projection, view);
        }
    }
}
