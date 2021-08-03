using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;

namespace Ox.Gui.QuickSpriteNamespace
{
    /// <summary>
    /// Compares the drawing Z of IQuickSpriteBatches.
    /// </summary>
    public class QuickSpriteBatchComparer : IComparer<QuickSpriteBatch>
    {
        public int Compare(QuickSpriteBatch x, QuickSpriteBatch y)
        {
            return OxMathHelper.Compare(x.Z, y.Z);
        }
    }

    /// <summary>
    /// Draws sprites in batches to reduce the number of XNA Draw calls per frame.
    /// </summary>
    public class QuickSpriteDrawer : Disposable
    {
        /// <summary>
        /// Create a QuickSpriteDrawer.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public QuickSpriteDrawer(OxEngine engine)
        {
            OxHelper.ArgumentNullCheck(engine);
            this.engine = engine;
        }

        /// <summary>
        /// Draw a single sprite.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the sprite is viewed.</param>
        /// <param name="sprite">The sprite to draw.</param>
        public void DrawSprite(GameTime gameTime, Camera camera, QuickSprite sprite)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera, sprite);
            if (!sprite.Visible || !sprite.Bounds.Intersects(OxConfiguration.VirtualScreen)) return; // OPTIMIZATION
            QuickSpriteBatch batch = GetCompatibleBatch(sprite);
            batch.QuickSprites.Clear();
            batch.QuickSprites.Add(sprite);
            batch.Draw(gameTime, camera);
        }

        /// <summary>
        /// Draw multiple sprites.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the sprites are viewed.</param>
        /// <param name="sprites">The sprites to draw.</param>
        public void DrawSprites(GameTime gameTime, Camera camera, IList<QuickSprite> sprites)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera, sprites);
            ClearBatches();
            PopulateBatches(sprites);
            SortBatches();
            DrawBatches(gameTime, camera);
        }

        /// <summary>
        /// Free any allocated resources used to draw sprites.
        /// </summary>
        public void FreeResources()
        {
            DisposeBatches();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) DisposeBatches();
            base.Dispose(disposing);
        }

        private void ClearBatches()
        {
            for (int i = 0; i < batches.Count; ++i) batches[i].QuickSprites.Clear();
        }

        private void PopulateBatches(IList<QuickSprite> sprites)
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                QuickSprite sprite = sprites[i];
                if (!sprite.Visible || !sprite.Bounds.Intersects(OxConfiguration.VirtualScreen)) continue;
                GetCompatibleBatch(sprite).QuickSprites.Add(sprite);
            }
        }

        private void SortBatches()
        {
            batches.Sort(batchComparer);
        }

        private void DrawBatches(GameTime gameTime, Camera camera)
        {
            for (int i = 0; i < batches.Count; ++i) batches[i].Draw(gameTime, camera);
        }

        private void DisposeBatches()
        {
            for (int i = 0; i < batches.Count; ++i) batches[i].Dispose();
            batches.Clear();
        }

        private QuickSpriteBatch GetCompatibleBatch(QuickSprite sprite)
        {
            for (int i = 0; i < batches.Count; ++i)
            {
                QuickSpriteBatch batch = batches[i];
                if (batch.IsCompatible(sprite)) return batch;
            }
            return CreateCompatibleBatch(sprite);
        }

        private QuickSpriteBatch CreateCompatibleBatch(QuickSprite sprite)
        {
            BaseEffect newEffect = new BaseEffect(engine.GraphicsDevice, engine.Load<Effect>("Ox/Effects/oxQuickSprite", OxConfiguration.GlobalDomainName));
            newEffect.DiffuseMap = engine.Load<Texture2D>(sprite.TextureFileName, OxConfiguration.GlobalDomainName);
            QuickSpriteBatch result = new QuickSpriteBatch(engine.GraphicsDevice, sprite.BlendMode, newEffect, sprite.TextureFileName);
            result.AddGarbage(newEffect);
            batches.Add(result);
            return result;
        }

        private readonly List<QuickSpriteBatch> batches = new List<QuickSpriteBatch>();
        private readonly QuickSpriteBatchComparer batchComparer = new QuickSpriteBatchComparer();
        private readonly OxEngine engine;
    }
}
