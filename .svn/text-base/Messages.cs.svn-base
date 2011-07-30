using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Beasty
{
    class Messages
    {
        SpriteBatch spriteBatch;
        ContentManager content;
        GraphicsDevice device;
        SpriteFont font;

        public Messages(ContentManager cm, SpriteBatch sb, GraphicsDevice gd)
        {
            this.spriteBatch = sb;
            this.content = cm;
            this.device = gd;
            font = content.Load<SpriteFont>("msg");
        }

        public void Draw(string msg)
        {
            this.Draw(msg, Color.Black);
        }

        public void Draw(string msg, Color color)
        {
            Vector3 position = new Vector3(device.Viewport.Width / 2, device.Viewport.Height / 2, 0);
            this.Draw(position, 1f, msg, color);
        }

        public void Draw(Vector3 position, float scale, string msg)
        {
            this.Draw(position, scale, msg, Color.Black);
        }

        public void Draw(Vector3 position, float scale, string msg, Color color)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.SaveState);
            Vector2 FontOrigin = font.MeasureString(msg) / 2;
            spriteBatch.DrawString(font, msg, new Vector2(position.X, position.Y), color, 0, FontOrigin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
        }
    }
}
