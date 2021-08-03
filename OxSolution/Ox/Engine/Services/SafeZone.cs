using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.Utility;

namespace Ox.Engine.ServicesNamespace
{
    /// <summary>
    /// Displays a parameterized safe zone on the screen. Stolen off the internets and refactored
    /// ever so slightly.
    /// </summary>
    public class SafeZone : Disposable2
    {
        public SafeZone(GraphicsDevice device, Point targetResolution)
        {
            OxHelper.ArgumentNullCheck(device);

            this.targetResolution = targetResolution;

            AddGarbage(spriteBatch = new SpriteBatch(device));
            AddGarbage(safeZoneTexture = new Texture2D(device, targetResolution.X, targetResolution.Y, 1, TextureUsage.None, SurfaceFormat.Color));
            AddGarbage(failZoneTexture = new Texture2D(device, targetResolution.X, targetResolution.Y, 1, TextureUsage.None, SurfaceFormat.Color));

            Color[] safePixels = new Color[targetResolution.X * targetResolution.Y];
            Color[] failPixels = new Color[targetResolution.X * targetResolution.Y];
            Rectangle safe = GetTitleSafeArea(OxConfiguration.SafeZoneMultiplier);
            Rectangle fail = GetTitleSafeArea((1 + OxConfiguration.SafeZoneMultiplier) * 0.5f);

            for (int x = 0; x < targetResolution.X; x++)
            {
                for (int y = 0; y < targetResolution.Y; y++)
                {
                    int i = y * targetResolution.X + x;

                    safePixels[i] = (safe.Contains(x, y))
                        ? Color.TransparentWhite : (fail.Contains(x, y))
                        ? new Color(255, 255, 0, 150) : Color.TransparentWhite;

                    failPixels[i] = (fail.Contains(x, y))
                        ? Color.TransparentWhite : new Color(255, 0, 0, 150);
                }
            }

            safeZoneTexture.SetData(safePixels);
            failZoneTexture.SetData(failPixels);
        }

        public Point TargetResolution { get { return targetResolution; } }

        public void Draw(GameTime gameTime)
        {
            OxHelper.ArgumentNullCheck(gameTime);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(safeZoneTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(failZoneTexture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        private Rectangle GetTitleSafeArea(float percent)
        {
            Rectangle result = new Rectangle(0, 0, targetResolution.X, targetResolution.Y);
            float border = (1 - percent) / 2;
            result.X = (int)(border * result.Width);
            result.Y = (int)(border * result.Height);
            result.Width = (int)(percent * result.Width);
            result.Height = (int)(percent * result.Height);
            return result;
        }

        private readonly SpriteBatch spriteBatch;
        private readonly Texture2D safeZoneTexture;
        private readonly Texture2D failZoneTexture;
        private readonly Point targetResolution;
    }
}
