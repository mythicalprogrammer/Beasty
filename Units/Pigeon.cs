using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Beasty.Units
{
    class Pigeon : UserUnit
    {
        public Pigeon(ContentManager cm, Vector2 start, AbstractUnitFactory factory)
            : base(cm, factory, cm.Load<Model>("models/pigeon"), start)
        {
            this.regen = 0.3f;
            this.mass = 1f;
            this._life = 50;
            this._lifeMax = 50;
        }
    }
}
