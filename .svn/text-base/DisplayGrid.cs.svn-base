using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Beasty
{
    class DisplayGrid
    {
        // A line-type boundry
        private VertexPositionColor[] boundVertices = new VertexPositionColor[4];
        private short[] boundIndices = new short[8] { 0, 1, 1, 2, 2, 3, 3, 0 };
        private const short gridSize = 20;
        private VertexPositionColor[] gridVerts = new VertexPositionColor[gridSize * gridSize / 2];
        private GraphicsDevice GraphicsDevice;

        public DisplayGrid(GraphicsDevice GraphicsDevice)
        {
            this.GraphicsDevice = GraphicsDevice;

            // Create the boundVertices
            boundVertices[0] = new VertexPositionColor(new Vector3(-5000f, 0f, 0f), Color.White);
            boundVertices[1] = new VertexPositionColor(new Vector3(-5000f, 5000f, 0f), Color.White);
            boundVertices[2] = new VertexPositionColor(new Vector3(5000f, 5000f, 0f), Color.White);
            boundVertices[3] = new VertexPositionColor(new Vector3(5000f, 0f, 0f), Color.White);


            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize / 2; j++)
                {
                    float inc = 5000 / gridSize;
                    float xpos = -5000 + i * inc * 2 + inc;
                    float ypos = j * inc * 2 + inc;
                    gridVerts[i + j * gridSize] = new VertexPositionColor(new Vector3(xpos, ypos, 0f), Color.Red);
                }
            }
        }

        public void Draw(Matrix projection, Matrix view)
        {
            // draw the bounding box
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.VertexColorEnabled = true;

            basicEffect.World = Matrix.Identity;
            basicEffect.View = view;
            basicEffect.Projection = projection;

            basicEffect.Begin();

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    boundVertices,
                    0,  // vertex buffer offset to add to each element of the index buffer
                    4,  // number of vertices in pointList
                    boundIndices,  // the index buffer
                    0,  // first index element to read
                    4   // number of primitives to draw
                );

                GraphicsDevice.RenderState.PointSize = 2;
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.PointList,
                    gridVerts,
                    0,  // index of the first vertex to draw
                    gridSize * gridSize / 2   // number of primitives
                );
                GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                pass.End();
            }
            basicEffect.End();
        }
    }
}
