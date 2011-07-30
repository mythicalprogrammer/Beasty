using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Beasty.Units;
using Beasty.Projectiles;

namespace Beasty.Units
{
    /// <summary>
    /// The available unit types we can create
    /// </summary>
    public enum UnitType
    {
        Tank, Pigeon
    }

    public enum ProjectileType
    {
        Bullet
    }
    
    #region Abstract Unit Factory
    /// <summary>
    /// The abstract unit factory defines how a unit factory behaves
    /// it exposes a default constructor and create unit type.
    /// </summary>
    public abstract class AbstractUnitFactory
    {
        protected ContentManager content;

        protected List<BaseUnit> units;
        protected List<BaseUnit> projectiles;

        public AbstractUnitFactory(ContentManager Content, List<BaseUnit> units, List<BaseUnit> projectiles) {
            this.content = Content;
            this.units = units;
            this.projectiles = projectiles;
        }

        public abstract void CreateUnit(UnitType type, Vector2 startPosition);
        public abstract void CreateProjectile(ProjectileType type, Vector2 startPosition, Vector2 startVelocity);
    }
    #endregion

    #region Concrete Unit Factory
    /// <summary>
    /// The concreate unit factory creates units!
    /// </summary>
    public class ConcreteUnitFactory : AbstractUnitFactory
    {
        public ConcreteUnitFactory(ContentManager cm, List<BaseUnit> units, List<BaseUnit> projectiles)
            : base(cm, units, projectiles) { }

        public override void CreateUnit(UnitType type, Vector2 startPosition)
        {
            switch (type)
            {
                case UnitType.Tank:
                    units.Add(new Tank(this.content, startPosition, this));
                    return;
                case UnitType.Pigeon:
                    units.Add(new Pigeon(this.content, startPosition, this));
                    return;
                default: return;
            }
        }

        public override void CreateProjectile(ProjectileType type, Vector2 startPosition, Vector2 startVelocity)
        {
            switch (type)
            {
                case ProjectileType.Bullet:
                    projectiles.Add(new Bullet(this.content, startPosition, startVelocity));
                    return;
                default: return;
            }
        }
    }
    #endregion
}
