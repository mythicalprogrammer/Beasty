using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Beasty.Units;

namespace Beasty
{
    class Collider
    {
        public static Boolean collides(BaseUnit first, BaseUnit second) {
            Rectangle a = first.getBound();
            Rectangle b = second.getBound();

            return (a.Right > b.Left && a.Left < b.Right &&
                    a.Bottom > b.Top && a.Top < b.Bottom);
        }
    }
}
