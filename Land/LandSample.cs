using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Beasty.Land
{
    class LandSample
    {
        // just display the land in the background
        Texture2D map;
        SpriteBatch spriteBatch;

        public LandSample(ContentManager cm, SpriteBatch sb)
        {
            this.spriteBatch = sb;
            map = cm.Load<Texture2D>("land/test1");
        }

        public void Draw()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.SaveState);
            // Draw our images
            spriteBatch.Draw(map, new Rectangle(0, 0, 640, 480), Color.White);
            spriteBatch.End();
        }

    }
}
