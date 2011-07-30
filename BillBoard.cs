using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Beasty
{/**
    class BillBoard
    {
        /** 
         * The billboards will be stored as a Vector4: three floats for the
         * center and an additional float to hold the size of the billboard.
         * Each image is define by two triangles in 3d space that will 
         * display the image.
         * */

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


        BasicEffect basicEffect;
        QuatCam quatCam;

        Texture2D myTexture;
        VertexExplosion[] explosionVertices;
        VertexDeclaration myVertexDeclaration;
        Effect expEffect;
        float time = 0;
        Random rand;


        /**
         *  For each billboarded 2D image, you will need to calculate the size 
         *  corner points of the two triangles that will hold the image in 3D
         *  space. These vertices will be stored in the variable on the last 
         *  line: 
         *        VectorPositionTexture[] billboardVertices;
         * */

        public BillBoard()
        {
            myVertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionTexture.VertexElements);
        }

        public Texture(string givenTexture, ContentManager content)
        {
            myTexture = content.Load<Texture2D>(givenTexture);
        }

        public Effect(string givenEffect, ContentManager content)
        {
            bbEffect = Content.Load<Effect>(givenEffect);
        }
        
 
       // public void AddBillboards(Vector4 givenVector4)
        public void AddBillboards()
        {
            int CPUpower = 6;
            for (int x = -CPUpower; x < CPUpower; x++)
                for (int y = -CPUpower; y < CPUpower; y++)
                    for (int z = -CPUpower; z < CPUpower; z++)
                        billboardList.Add(new Vector4(x * 1, y * 1, z * 1, 10));
 
        }

        public void CreateBBVertices()
        {
            billboardVertices = new VertexPositionTexture[billboardList.Count * 6];

            int i = 0;
            foreach (Vector4 currentV4 in billboardList)
            {
                Vector3 center = new Vector3(currentV4.X, currentV4.Y, currentV4.Z);

                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 0));
                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));

                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 0));
                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(1, 1));
                billboardVertices[i++] = new VertexPositionTexture(center, new Vector2(0, 1));
            }
        }
    }

}
