using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Beasty.Units;

namespace Beasty.Projectiles
{
    class Bullet : BaseUnit
    {
        public Bullet(ContentManager cm, Vector2 startPosition, Vector2 startVelocity)
            : base(cm.Load<Model>("models/bullet"), startPosition)
        {
            this._velocity = startVelocity;
            this.scale = 0.3f;
        }

        protected override void hitWall()
        {
            this._life = 0;
        }

        public override void Draw(Matrix projection, Matrix view)
        {
            float rotz = 0f;
            if (this._velocity.X != 0)
            { rotz = (float)Math.Atan2(_velocity.Y, _velocity.X); }

            DrawModel(_body, projection, view,
                new Vector3(position, 0), new Vector3(0, (float) Math.PI/2, rotz), this.scale);
        }
    }
}
