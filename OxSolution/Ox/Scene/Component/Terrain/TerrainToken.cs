using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for a Terrain.
    /// </summary>
    public class TerrainToken : SingleSurfaceComponentToken<TerrainSurface>
    {
        /// <summary>
        /// Create a TerrainToken.
        /// </summary>
        public TerrainToken() : base(TerrainDefaults.EffectFileName)
        {
            quadScale = new ProxyMemberProperty<Vector3>(this, "Surface", "QuadScale", TerrainDefaults.QuadScale);
            textureRepetition = new ProxyMemberProperty<Vector2>(this, "Surface", "TextureRepetition", TerrainDefaults.TextureRepetition);
            gridDims = new ProxyMemberProperty<Point>(this, "Surface", "GridDims", TerrainDefaults.GridDims);
            heightMapFileName = new ProxyMemberProperty<string>(this, "Surface", "HeightMapFileName", TerrainDefaults.HeightMapFileName);
            diffuseMap0FileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMap0FileName", TerrainDefaults.DiffuseMap0FileName);
            diffuseMap1FileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMap1FileName", TerrainDefaults.DiffuseMap1FileName);
            diffuseMap2FileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMap2FileName", TerrainDefaults.DiffuseMap2FileName);
            diffuseMap3FileName = new ProxyMemberProperty<string>(this, "Surface", "DiffuseMap3FileName", TerrainDefaults.DiffuseMap3FileName);
            smoothingFactor = new ProxyMemberProperty<float>(this, "Surface", "SmoothingFactor", TerrainDefaults.SmoothingFactor);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new Terrain Instance
        {
            get { return OxHelper.Cast<Terrain>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Vector3 QuadScale
        {
            get { return quadScale.Value; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue);
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue);
                value.Z = MathHelper.Clamp(value.Z, 0.001f, float.MaxValue);
                quadScale.Value = value;
            }
        }
        
        public Vector2 TextureRepetition
        {
            get { return textureRepetition.Value; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue);
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue);
                textureRepetition.Value = value;
            }
        }
        
        public Point GridDims
        {
            get { return gridDims.Value; }
            set
            {
                value.X = (int)MathHelper.Clamp(value.X, 1, 128);
                value.Y = (int)MathHelper.Clamp(value.Y, 1, 128);
                gridDims.Value = value;
            }
        }
        
        [DefaultValue(TerrainDefaults.HeightMapFileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string HeightMapFileName
        {
            get { return heightMapFileName.Value; }
            set { heightMapFileName.Value = value; }
        }
        
        [DefaultValue(TerrainDefaults.DiffuseMap0FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap0FileName
        {
            get { return diffuseMap0FileName.Value; }
            set { diffuseMap0FileName.Value = value; }
        }
        
        [DefaultValue(TerrainDefaults.DiffuseMap1FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap1FileName
        {
            get { return diffuseMap1FileName.Value; }
            set { diffuseMap1FileName.Value = value; }
        }
        
        [DefaultValue(TerrainDefaults.DiffuseMap2FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap2FileName
        {
            get { return diffuseMap2FileName.Value; }
            set { diffuseMap2FileName.Value = value; }
        }
        
        [DefaultValue(TerrainDefaults.DiffuseMap3FileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string DiffuseMap3FileName
        {
            get { return diffuseMap3FileName.Value; }
            set { diffuseMap3FileName.Value = value; }
        }
        
        [DefaultValue(TerrainDefaults.SmoothingFactor)]
        public float SmoothingFactor
        {
            get { return smoothingFactor.Value; }
            set { smoothingFactor.Value = MathHelper.Clamp(value, 0, 1); }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new Terrain(engine, domainName, ownedByDomain, QuadScale, GridDims,
                SmoothingFactor, EffectFileName, HeightMapFileName, DiffuseMap0FileName,
                DiffuseMap1FileName, DiffuseMap2FileName, DiffuseMap3FileName, TextureRepetition);
        }
        
        private readonly ProxyMemberProperty<Vector3> quadScale;
        private readonly ProxyMemberProperty<Vector2> textureRepetition;
        private readonly ProxyMemberProperty<Point> gridDims;
        private readonly ProxyMemberProperty<string> heightMapFileName;
        private readonly ProxyMemberProperty<string> diffuseMap0FileName;
        private readonly ProxyMemberProperty<string> diffuseMap1FileName;
        private readonly ProxyMemberProperty<string> diffuseMap2FileName;
        private readonly ProxyMemberProperty<string> diffuseMap3FileName;
        private readonly ProxyMemberProperty<float> smoothingFactor;
    }
}
