using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Scene.SurfaceNamespace;
using Ox.Scene.EffectNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A surface that is normal mapped.
    /// </summary>
    public class NormalMappedSurface : ShadowReceiverSurface
    {
        /// <summary>
        /// Create a NormalMappedSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        public NormalMappedSurface(OxEngine engine, StandardModel component, string effectFileName)
            : base(engine, component, effectFileName)
        {
            NormalMapFileName = "Ox/Textures/waves";
        }

        /// <summary>
        /// Create a NormalMappedSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public NormalMappedSurface(OxEngine engine, StandardModel component)
            : this(engine, component, "Ox/Effects/oxNormalMapped") { }

        /// <summary>
        /// The normal map.
        /// </summary>
        public Texture2D NormalMap { get { return _normalMap; } }

        /// <summary>
        /// The normal map file name.
        /// </summary>
        public string NormalMapFileName
        {
            get { return _normalMapFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_normalMapFileName == value) return; // OPTIMIZATION
                Texture2D newNormalMap = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _normalMap = newNormalMap;
                _normalMapFileName = value;
            }
        }
        
        /// <summary>
        /// Is normal mapping enabled?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return normalMapEnabled; }
            set { normalMapEnabled = value; }
        }

        /// <inheritdoc />
        protected override void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            base.PopulateEffectHook(gameTime, camera, drawMode);
            NormalMappedEffect nmEffect = OxHelper.Cast<NormalMappedEffect>(Effect);
            nmEffect.NormalMap = NormalMap;
            nmEffect.NormalMapEnabled = normalMapEnabled;
        }

        /// <inheritdoc />
        protected override Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new NormalMappedEffect(Engine.GraphicsDevice, effectFromDisk);
        }
        
        private bool normalMapEnabled = true;
        private Texture2D _normalMap;
        private string _normalMapFileName = string.Empty;
    }
}
