using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Beasty.Land;
using Beasty.Units;

namespace Beasty
{
    // states the game can be in
    public enum BeastyStates
    {
        Menu, Normal, Paused
    };


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Core : Microsoft.Xna.Framework.Game
    {
        private GraphicsDevice device;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        // user messages
        private Messages messages;

        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // The current state of the game
        private BeastyStates state = BeastyStates.Normal;

        // a list of controllers
        private List<Controller> controllers = new List<Controller>();

        // list of drawable units, projectiles, misc.
        private List<BaseUnit> units = new List<BaseUnit>();
        private List<BaseUnit> projectiles = new List<BaseUnit>();

        // Grid and boundary
        private DisplayGrid grid;       

        // A shared random number generator
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        // testing creation of land masses
        private LandSample landtest;
        
        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            //TODO double buffering
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferMultiSampling = true;
            /**
            graphics.PreparingDeviceSettings +=
                  new EventHandler<PreparingDeviceSettingsEventArgs>(
                      graphics_PreparingDeviceSettings);
            **/
            Content.RootDirectory = "Content";
        }

        #region Preparing for Anti-Aliasing
        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // Xbox 360 and most PCs support FourSamples/0 
            // (4x) and TwoSamples/0 (2x) antialiasing.
            PresentationParameters pp = e.GraphicsDeviceInformation.PresentationParameters;
#if XBOX
            pp.MultiSampleQuality = 0;
            pp.MultiSampleType = MultiSampleType.FourSamples;
            return;
#else
            int quality = 0;
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            // Check for 4xAA
            if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, format,
                false, MultiSampleType.FourSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                pp.MultiSampleQuality = 0;
                pp.MultiSampleType =
                    MultiSampleType.FourSamples;
            }
            // Check for 2xAA
            else if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware,
                format, false, MultiSampleType.TwoSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                pp.MultiSampleQuality = 0;
                pp.MultiSampleType =
                    MultiSampleType.TwoSamples;
            }
            return;
#endif
        }
        #endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            device = graphics.GraphicsDevice;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            // Create a new sprite batch, this will be used to draw messages
            spriteBatch = new SpriteBatch(GraphicsDevice);

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            landtest = new LandSample(Content, spriteBatch);

            messages = new Messages(Content, spriteBatch, device);

            // TODO: Menu system for creating units

            // creating our units.
            AbstractUnitFactory factory = new ConcreteUnitFactory(Content, units, projectiles);
            factory.CreateUnit(UnitType.Pigeon, new Vector2(-700, 100));
            factory.CreateUnit(UnitType.Tank, new Vector2(700, 100));

            // assign controller(s) to units
            // TODO: refactor? this is a little magical
            Controller p1 = new Controller(PlayerIndex.One, units[0]);
            controllers.Add(p1);

            // set up the boundry grid (for testing..)
            grid = new DisplayGrid(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            #region Keyboard Control
            if (keyState.IsKeyDown(Keys.W)) { units[0].MoveUnit(new Vector2(0, 1)); }
            if (keyState.IsKeyDown(Keys.S)) { units[0].MoveUnit(new Vector2(0, -1)); }
            if (keyState.IsKeyDown(Keys.A)) { units[0].MoveUnit(new Vector2(-1, 0)); }
            if (keyState.IsKeyDown(Keys.D)) { units[0].MoveUnit(new Vector2(1, 0)); }

            if (keyState.IsKeyDown(Keys.Q)) { units[0].Fire(); }

            if (keyState.IsKeyDown(Keys.Up)) { units[1].MoveUnit(new Vector2(0, 1)); }
            if (keyState.IsKeyDown(Keys.Down)) { units[1].MoveUnit(new Vector2(0, -1)); }
            if (keyState.IsKeyDown(Keys.Left)) { units[1].MoveUnit(new Vector2(-1, 0)); }
            if (keyState.IsKeyDown(Keys.Right)) { units[1].MoveUnit(new Vector2(1, 0)); }

            if (keyState.IsKeyDown(Keys.Escape))
            {
                foreach (BaseUnit unit in units) { unit.Reset(); }
            }
            #endregion

            #region Gamepad Control
            state = BeastyStates.Normal;
            foreach (Controller controller in controllers)
            {
                if (controller.Update(gameTime) == ControllerState.Exit) this.Exit();
                if (controller.paused) { 
                    state = BeastyStates.Paused;
                  

                }
            }
            #endregion

            #region Units and Camera Update
            // Updates each unit
            if(state == BeastyStates.Normal) 
            {
                if (projectiles.Count > 0)
                {
                    projectiles.RemoveAll(i => i.life <= 0);

                    foreach (BaseUnit projectile in projectiles)
                    { projectile.Update(gameTime); }
                }

                foreach (BaseUnit unit in units)
                { unit.Update(gameTime); }

                // HAHA, ridiculous, no sense of momentum >__<
                // try using p=mv to conserve momentum, mmmmkay?
                if (Collider.collides(this.units[0], this.units[1]))
                {
                    this.units[0].Collide(this.units[1]);
                    //this.units[1].Collide(this.units[0]);
                }

                cameraPosition.Z = Math.Min(7000f, Math.Max(1000f,
                    Vector2.Distance(units[0].position, units[1].position) * 1.2f));
                cameraLookAt.X = (units[0].position.X + units[1].position.X) / 2;
                cameraLookAt.Y = (units[0].position.Y + units[1].position.Y) / 2;
                cameraPosition.X = cameraLookAt.X;
                cameraPosition.Y = cameraLookAt.Y;

                base.Update(gameTime);
            }
            #endregion
        }

        #region Reset Render States
        /// <summary>
        /// Helper function for setting renderstates back to normal.
        /// </summary>
        void SetDefaultRenderState(RenderState renderState)
        {
            // Enable point sprites.
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;

            // Set the alpha blend mode.
            renderState.AlphaBlendEnable = false;

            // Set the alpha test mode.
            renderState.AlphaTestEnable = false;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;

            renderState.CullMode = CullMode.CullCounterClockwiseFace;
        }
        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0f, 50f, 5000f);
        Vector3 cameraLookAt = new Vector3();
        ///------------------

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix view = Matrix.CreateLookAt(
                cameraPosition,
                cameraLookAt,
                Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                aspectRatio, 1.0f, 50000.0f);

            // SetDefaultRenderState(device.RenderState);                

            // draw each unit
            foreach (BaseUnit unit in this.units)
            { unit.Draw(projection, view); }

            foreach (BaseUnit projectile in this.projectiles)
            { projectile.Draw(projection, view); }

            // landtest.Draw();
            grid.Draw(projection, view);

            switch (state)
            {
                case BeastyStates.Paused :
                    messages.Draw("paused");
                    break;
                default:
                    break;
            }


            foreach (UserUnit unit in units)
            {
                // Draw 2d messages...
                Vector3 pos = device.Viewport.Project(new Vector3(unit.position.X, unit.position.Y - 100, 0f), projection, view, Matrix.Identity);
                float scale = 1000 / Vector3.Distance(cameraPosition, new Vector3(unit.position, 0));
                messages.Draw(pos, scale, "Energy:" + unit.energy + "\nLife:" + unit.life);
            }

            base.Draw(gameTime);

        }
    }
}
