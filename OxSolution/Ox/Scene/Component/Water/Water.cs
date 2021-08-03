using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Various default values use by Water.
    /// </summary>
    public static class WaterDefaults
    {
        public static readonly Vector2 WaveMap0Velocity = new Vector2(0.1f, 0);
        public static readonly Vector2 WaveMap1Velocity = new Vector2(0, 0.1f);
        public static readonly Color ColorMultiplier = Color.DarkBlue;
        public static readonly Color ColorAdditive;
        public const DrawStyle DrawStyleDefault = DrawStyle.Transparent;
        public const string EffectFileName = "Ox/Effects/oxWater";
        public const string WaveMap0FileName = "Ox/Textures/waves";
        public const string WaveMap1FileName = "Ox/Textures/waves";
        public const float WaveLength = 0.5f;
        public const float WaveHeight = 0.5f;
    }

    /// <summary>
    /// A body of water on the x,z plane.
    /// </summary>
    public class Water : SingleSurfaceComponent<WaterSurface>
    {
        /// <summary>
        /// Create a Water object.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        public Water(OxEngine engine, string domainName, bool ownedByDomain)
            : base(engine, domainName, ownedByDomain, new BoxPrimitive(Vector3.Zero, new Vector3(0.5f)))
        {
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateSurface<WaterSurface>(this));
        }

        /// <inheritdoc />
        protected override WaterSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new WaterToken();
        }
        
        private readonly WaterSurface surface;
    }
}
