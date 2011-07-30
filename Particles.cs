using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Beasty
{
    class Particles
    {
        public struct VertexExplosion
        {
            public Vector3 Position;
            public Vector4 TexCoord;
            public Vector4 AdditionalInfo;

            public VertexExplosion(Vector3 Position, Vector4 TexCoord, Vector4 AdditionalInfo)
            {
                this.Position = Position;
                this.TexCoord = TexCoord;
                this.AdditionalInfo = AdditionalInfo;
            }

            public static readonly VertexElement[] VertexElements = new VertexElement[]
            {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                new VertexElement(0, 12, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(0, 28, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
            };
            public static readonly int SizeInBytes = sizeof(float) * (3 + 4 + 4);
        }

        GraphicsDevice device;
        Texture2D myTexture;
        VertexExplosion[] explosionVertices;
        VertexDeclaration myVertexDeclaration;
        Effect expEffect;
        Random rand;
        QuaternionCam quatCam;

        public Particles(string givenTexture, string givenEffect, ContentManager Content, GraphicsDevice givenDevice)
        {
            rand = new Random();
            device = givenDevice;
            myTexture = Content.Load<Texture2D>(givenTexture);
            expEffect = Content.Load<Effect>(givenEffect);
            myVertexDeclaration = new VertexDeclaration(device, VertexExplosion.VertexElements);
            quatCam = new QuaternionCam(givenDevice.Viewport);
        }

        public void CreateExplosionVertices(float time)
        {
            int particles = 2;
            explosionVertices = new VertexExplosion[particles * 6];

            int i = 0;
            for (int partnr = 0; partnr < particles; partnr++)
            {
                Vector3 startingPos = new Vector3(5, 0, 0);

                float r1 = (float)rand.NextDouble() - 0.5f;
                float r2 = (float)rand.NextDouble() - 0.5f;
                float r3 = (float)rand.NextDouble() - 0.5f;
                Vector3 moveDirection = new Vector3(r1, r2, r3);

                moveDirection.Normalize();

                float r4 = (float)rand.NextDouble();
                r4 = r4 / 4.0f * 3.0f + 0.25f;

                temp += 0.3f;
                Matrix test = Matrix.Identity*Matrix.CreateRotationZ(temp);

                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(1, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(0, 0, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(1, 0, time, 1000), new Vector4(moveDirection, r4));

                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(1, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(0, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(startingPos, new Vector4(0, 0, time, 1000), new Vector4(moveDirection, r4));
            }
        }

        private float temp = 1f;
        //public void DrawExplosion(Matrix ProjectionMatrix, Matrix ViewMatrix, Vector3 Position, Vector3 UpVector, GameTime gameTime)
        public void DrawExplosion(GameTime gameTime)
        {
            temp += 0.3f;
            // device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Blue, 1, 0);
            if (explosionVertices != null)
            {
                temp += 0.3f;
                //draw billboards Matrix.Identity*
                expEffect.CurrentTechnique = expEffect.Techniques["Explosion"];
                expEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
                expEffect.Parameters["xProjection"].SetValue(quatCam.ProjectionMatrix*Matrix.CreateRotationZ(temp));
                expEffect.Parameters["xView"].SetValue(quatCam.ViewMatrix);

                expEffect.Parameters["xCamPos"].SetValue(quatCam.Position);
                expEffect.Parameters["xExplosionTexture"].SetValue(myTexture);
                expEffect.Parameters["xCamUp"].SetValue(quatCam.UpVector);
                expEffect.Parameters["xTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);

                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.One;
                device.RenderState.DepthBufferWriteEnable = false;

                expEffect.Begin();
                foreach (EffectPass pass in expEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    device.VertexDeclaration = myVertexDeclaration;
                    device.DrawUserPrimitives<VertexExplosion>(PrimitiveType.TriangleList, explosionVertices, 0, explosionVertices.Length / 3);
                    pass.End();
                }
                expEffect.End();

                device.RenderState.DepthBufferWriteEnable = true;
            }
            device.RenderState.AlphaBlendEnable = false;
            

        }


    }
}
