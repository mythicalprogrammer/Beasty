using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Beasty.Units
{
    class Tank : UserUnit
    {
        public Tank(ContentManager cm, Vector2 start, AbstractUnitFactory factory)
            : base(cm, factory, cm.Load<Model>("models/hairship"), start)
        {
            this.scale = 1.6f;
            this._life = 500;
            this._lifeMax = 500;
        }
    }
}
