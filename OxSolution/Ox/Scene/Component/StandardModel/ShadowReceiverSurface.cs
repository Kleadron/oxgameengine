using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Scene.EffectNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A surface that receives shadows.
    /// </summary>
    public class ShadowReceiverSurface : StandardSurface
    {
        /// <summary>
        /// Create a ShadowReceiverSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        public ShadowReceiverSurface(OxEngine engine, StandardModel component, string effectFileName)
            : base(engine, component, effectFileName) { }

        /// <summary>
        /// Create a ShadowReceiverSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public ShadowReceiverSurface(OxEngine engine, StandardModel component)
            : this(engine, component, "Ox/Effects/oxShadowReceiver") { }

        /// <inheritdoc />
        protected override void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            base.PopulateEffectHook(gameTime, camera, drawMode);
            ShadowReceiverEffect srEffect = OxHelper.Cast<ShadowReceiverEffect>(Effect);
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            srEffect.PopulateShadowing(this, sceneSystem.CachedDirectionalLights);
        }

        /// <inheritdoc />
        protected override Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new ShadowReceiverEffect(Engine.GraphicsDevice, effectFromDisk);
        }
    }
}
