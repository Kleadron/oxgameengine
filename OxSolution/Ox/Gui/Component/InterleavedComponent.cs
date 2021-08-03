using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Gui.QuickSpriteNamespace;

namespace Ox.Gui.Component
{
    /// <summary>
    /// A component that is visually interleaved with gui components.
    /// </summary>
    public class InterleavedComponent : UpdateableComponent
    {
        /// <summary>
        /// Create an InterleavedComponent.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain</param>
        public InterleavedComponent(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain) { }

        /// <summary>
        /// Is the component visible?
        /// </summary>
        public bool Visible
        {
            get { return VisibleHook; }
            set { VisibleHook = value; }
        }

        /// <summary>
        /// Draw the component.
        /// </summary>
        public void Draw(GameTime gameTime, Camera camera)
        {
            OxHelper.ArgumentNullCheck(gameTime, camera);

            CollectQuickSprites(cachedSprites);
            {
                DrawSprites(gameTime, camera, cachedSprites);
            }
            cachedSprites.Clear();
        }

        /// <summary>
        /// Collect all the components's sprites.
        /// </summary>
        public IList<QuickSprite> CollectQuickSprites(IList<QuickSprite> result)
        {
            OxHelper.ArgumentNullCheck(result);
            return CollectQuickSpritesHook(result);
        }

        /// <summary>
        /// Handle getting / setting visibility.
        /// </summary>
        protected virtual bool VisibleHook
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Handle collecting the internal quick sprites.
        /// </summary>
        protected virtual IList<QuickSprite> CollectQuickSpritesHook(IList<QuickSprite> result)
        {
            return result;
        }

        private void DrawSprites(GameTime gameTime, Camera camera, IList<QuickSprite> cachedSprites)
        {
            Engine
                .GetService<QuickSpriteDrawer>()
                .DrawSprites(gameTime, camera, cachedSprites);
        }

        private readonly IList<QuickSprite> cachedSprites = new List<QuickSprite>();
        private bool _visible = true;
    }
}
