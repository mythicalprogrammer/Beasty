using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beasty.Units
{
    public enum Facing {
        Left, Right
    }

    public class BaseUnit
    {
        protected Facing facing { get; set; }
        protected float scale = 1f;

        protected Model _body;
        protected Vector2 _position = new Vector2();
        protected Vector2 _velocity = new Vector2();

        protected int _energy = 100;
        protected int _energyMax = 1000; // default max energy
        protected float regen = 0.1f; // default energy regeneration rate.

        protected int _life = 100;
        protected int _lifeMax = 100;

        protected float mass = 5f;


        public Vector2 position
        {
            get { return _position; }
        }
        public Vector2 velocity
        {
            get { return _velocity; }
        }
        public int energy
        {
            get { return _energy; }
        }
        public int life
        {
            get { return _life; }
        }

        public BaseUnit(
            Model def_body, 
            Vector2 startPosition)
        {
            this._body = def_body;

            // deep copy (is there a better way?)
            this._position.X = startPosition.X;
            this._position.Y = startPosition.Y;
        }

        public virtual void MoveUnit(Vector2 ForceDirection)
        {
            if (_energy > 0)
            {
                Vector2 accel = new Vector2(ForceDirection.X, ForceDirection.Y) / this.mass;

                // change the facing of the ship (many animate this?)
                // when the user moves it.
                if (accel.X != 0)
                {
                    this.facing = (accel.X > 0) ? Facing.Right : Facing.Left;
                }

                // acceleration is added to _velocity
                this._velocity += accel;
            }

            int usage = Math.Max(1, (int)(ForceDirection * 10).Length());
            _energy -= usage;
        }

        public virtual void MoveAimer(Vector2 ForceDirection) { }

        public virtual void Fire() { }

        protected virtual void hitWall() { }

        // updates a time step (applies the _velocity)
        internal virtual void Update(GameTime time)
        {
            // Air FRICTION!
            this._velocity.X /= 1.001f;
            this._velocity.Y /= 1.001f;

            // Gravity
            this._velocity.Y -= 0.1f;

            // Floor collision
            if (this._position.Y < 0)
            {
                // ground friction
                this._velocity.X /= 1.05f;
                this._velocity.Y *= -0.5f;
                this._position.Y *= -1f;
                hitWall();
            }

            // Sidewall collision
            if (Math.Abs(_position.X) > 5000)
            {
                this._velocity.X *= -0.75f;
                if (_position.X > 5000)
                {
                    this._position.X = 4995f;
                }
                else
                {
                    this._position.X = -4995f;
                }
                hitWall();
            }

            // ceiling collision
            if (_position.Y > 5000)
            {
                this._velocity.Y *= -0.75f;
                this._position.Y = 5000f;
            }

            // unit position is updated
            this._position += this._velocity;

            if (_energy < _energyMax)
            {
                _energy += (int) (regen * time.ElapsedGameTime.TotalMilliseconds);
            }
            // TODO: check if moved into other quadrant
        }
        
        internal void Collide(BaseUnit other)
        {
            // try using p=mv to conserve momentum, mmmmkay?
            Vector2 diff = other.position - this._position;
            this._position -= diff;

            //this._velocity.X *= (float)-0.5;
            //this._velocity.Y *= (float)-0.5;

            this._velocity = this._velocity * ((this.mass - other.mass) / (this.mass + other.mass));
            other._velocity = this._velocity * (this._velocity * (this.mass * 2) / (this.mass + other.mass));

            this._life -= (int) (velocity.Length() + other.velocity.Length());
        }

        internal void Reset()
        {
            this._position.X = 0;
            this._position.Y = 0;
            this._velocity.X = 0;
            this._velocity.Y = 0;
        }

        internal Rectangle getBound()
        {
            int width = (int) 200;
            int height = (int) 50;
            return new Rectangle(
                (int) this._position.X - width / 2,
                (int) this._position.Y - height / 2,
                width, height);
        }

        public virtual void Draw(Matrix projection, Matrix view)
        {
            // First, draw the base model
            Vector3 modelPosition = new Vector3(this.position.X, this.position.Y, 0);
            float modelRotation = (float)Math.PI / 2;

            // check if we're facing left or right
            if (this.facing == Facing.Right)
            { modelRotation = -modelRotation; }

            DrawModel(_body, projection, view, 
                modelPosition, new Vector3(0, modelRotation, 0), this.scale);
        }
        
        protected void DrawModel(
            Model myModel, Matrix projection, Matrix view,
            Vector3 modelPosition, Vector3 modelRotation, float modelScale)
        {
            DrawModel(myModel, projection, view, modelPosition, modelRotation, new Vector3(modelScale), Vector3.Zero);
        }

        protected void DrawModel(
            Model myModel, Matrix projection, Matrix view,
            Vector3 modelPosition, Vector3 modelRotation, float modelScale, Vector3 diffuseColor)
        {
            DrawModel(myModel, projection, view, modelPosition, modelRotation, new Vector3(modelScale), diffuseColor);
        }

        protected void DrawModel(
            Model myModel, Matrix projection, Matrix view,
            Vector3 modelPosition, Vector3 modelRotation, Vector3 modelScale)
        {
            DrawModel(myModel, projection, view, modelPosition, modelRotation, modelScale, Vector3.Zero);
        }

        protected void DrawModel(
            Model myModel, Matrix projection, Matrix view, 
            Vector3 modelPosition, Vector3 modelRotation, Vector3 modelScale, Vector3 diffuseColor)
        {
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (diffuseColor != Vector3.Zero)
                    {
                        effect.DiffuseColor = diffuseColor;
                    }
                    

                    // first apply the bone transform,
                    // followed by the scale, rotation and finally translation
                    // (order is important!)
                    effect.World =
                        transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(modelScale)
                        * Matrix.CreateRotationX(modelRotation.X)
                        * Matrix.CreateRotationY(modelRotation.Y)
                        * Matrix.CreateRotationZ(modelRotation.Z)
                        * Matrix.CreateTranslation(modelPosition)
                        ;

                    effect.Projection = projection;
                    effect.View = view; 
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

    }
}
