using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.MathNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Various default values use by Terrains.
    /// </summary>
    public static class TerrainDefaults
    {
        public static readonly Vector3 QuadScale = new Vector3(8, 64, 8);
        public static readonly Vector2 TextureRepetition = new Vector2(8, 8);
        public static readonly Point GridDims = new Point(8, 8);
        public const string EffectFileName = "Ox/Effects/oxTerrain";
        public const string HeightMapFileName = "Ox/HeightMaps/heightMap";
        public const string DiffuseMap0FileName = "Ox/Textures/Sand";
        public const string DiffuseMap1FileName = "Ox/Textures/Grass";
        public const string DiffuseMap2FileName = "Ox/Textures/Rock";
        public const string DiffuseMap3FileName = "Ox/Textures/Snow";
        public const float SmoothingFactor = 1.0f;
    }

    /// <summary>
    /// A terrain that is split into patches.
    /// </summary>
    public class Terrain : SingleSurfaceComponent<TerrainSurface>
    {
        /// <summary>
        /// Create a Terrain.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="gridDims">See property GridDims.</param>
        /// <param name="quadScale">See property QuadScale.</param>
        /// <param name="smoothingFactor">See property SmoothingFactor.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        /// <param name="heightMapFileName">See property HeightMapFileName.</param>
        /// <param name="diffuseMap0FileName">See property DiffuseMap0FileName.</param>
        /// <param name="diffuseMap1FileName">See property DiffuseMap1FileName.</param>
        /// <param name="diffuseMap2FileName">See property DiffuseMap2FileName.</param>
        /// <param name="diffuseMap3FileName">See property DiffuseMap3FileName.</param>
        /// <param name="textureRepetition">See property TextureRepetition.</param>
        public Terrain(OxEngine engine, string domainName, bool ownedByDomain,
            Vector3 quadScale, Point gridDims, float smoothingFactor, string effectFileName,
            string heightMapFileName, string diffuseMap0FileName, string diffuseMap1FileName,
            string diffuseMap2FileName, string diffuseMap3FileName, Vector2 textureRepetition)
            : base(engine, domainName, ownedByDomain)
        {
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateTerrainSurface(
                this, quadScale, gridDims, smoothingFactor, effectFileName, heightMapFileName,
                diffuseMap0FileName, diffuseMap1FileName, diffuseMap2FileName, diffuseMap3FileName,
                textureRepetition));
        }

        /// <summary>
        /// The height map that represents the topography of the terrain.
        /// </summary>
        public HeightMap HeightMap { get { return surface.HeightMap; } }

        /// <summary>
        /// The scale of each geometry quad.
        /// </summary>
        public Vector3 QuadScale { get { return surface.QuadScale; } }

        /// <summary>
        /// The scale of each terrain patch.
        /// </summary>
        public Vector3 PatchScale { get { return surface.PatchScale; } }

        /// <summary>
        /// The scale of the grid.
        /// </summary>
        public Vector3 GridScale { get { return surface.GridScale; } }

        /// <summary>
        /// The offset used to center the terrain on the [x, z] origin.
        /// </summary>
        public Vector2 GridCenterOffset { get { return surface.GridCenterOffset; } }

        /// <summary>
        /// The number of quads in each patch.
        /// </summary>
        public Point PatchDims { get { return surface.PatchDims; } }

        /// <summary>
        /// The number of patches in the terrain.
        /// </summary>
        public Point GridDims { get { return surface.GridDims; } }

        /// <inheritdoc />
        protected override TerrainSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new TerrainToken();
        }

        private readonly TerrainSurface surface;
    }
}
