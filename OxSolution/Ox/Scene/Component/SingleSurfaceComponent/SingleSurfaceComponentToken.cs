using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.Utility;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for a SingleSurfaceComponent.
    /// </summary>
    public abstract class SingleSurfaceComponentToken<T> : SceneComponentToken
        where T : BaseSurface
    {
        /// <summary>
        /// Create a SingleSurfaceComponentToken.
        /// </summary>
        /// <param name="effectFileName">The name of the effect file.</param>
        public SingleSurfaceComponentToken(string effectFileName)
        {
            drawStyle = new ProxyMemberProperty<DrawStyle>(this, "Surface", "DrawStyle", DrawStyle.Opaque);
            faceMode = new ProxyMemberProperty<FaceMode>(this, "Surface", "FaceMode", FaceMode.FrontFaces);
            diffuseColor = new ProxyMemberProperty<Color>(this, "Surface", "DiffuseColor", Color.White);
            specularColor = new ProxyMemberProperty<Color>(this, "Surface", "SpecularColor", Color.Gray);
            this.effectFileName = new ProxyMemberProperty<string>(this, "Surface", "EffectFileName", effectFileName);
            specularPower = new ProxyMemberProperty<float>(this, "Surface", "SpecularPower", 8);
            lightingEnabled = new ProxyMemberProperty<bool>(this, "Surface", "LightingEnabled", true);
            drawTransparentPixels = new ProxyMemberProperty<bool>(this, "Surface", "DrawTransparentPixels", false);
            drawPriority = new ProxyMemberProperty<int>(this, "Surface", "DrawPriority");
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new SingleSurfaceComponent<T> Instance
        {
            get { return OxHelper.Cast<SingleSurfaceComponent<T>>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(DrawStyle.Opaque)]
        public DrawStyle DrawStyle
        {
            get { return drawStyle.Value; }
            set { drawStyle.Value = value; }
        }
        
        [DefaultValue(FaceMode.FrontFaces)]
        public FaceMode FaceMode
        {
            get { return faceMode.Value; }
            set { faceMode.Value = value; }
        }
        
        public Color DiffuseColor
        {
            get { return diffuseColor.Value; }
            set { diffuseColor.Value = value; }
        }
        
        public Color SpecularColor
        {
            get { return specularColor.Value; }
            set { specularColor.Value = value; }
        }
        
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string EffectFileName
        {
            get { return effectFileName.Value; }
            set { effectFileName.Value = value; }
        }
        
        [DefaultValue(8.0f)]
        public float SpecularPower
        {
            get { return specularPower.Value; }
            set { specularPower.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool LightingEnabled
        {
            get { return lightingEnabled.Value; }
            set { lightingEnabled.Value = value; }
        }
        
        [DefaultValue(false)]
        public bool DrawTransparentPixels
        {
            get { return drawTransparentPixels.Value; }
            set { drawTransparentPixels.Value = value; }
        }
        
        [DefaultValue(0)]
        public int DrawPriority
        {
            get { return drawPriority.Value; }
            set { drawPriority.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            throw new InvalidOperationException("SingleSceneComponentToken cannot create an instance.");
        }

        private readonly ProxyMemberProperty<DrawStyle> drawStyle;
        private readonly ProxyMemberProperty<FaceMode> faceMode;
        private readonly ProxyMemberProperty<Color> diffuseColor;
        private readonly ProxyMemberProperty<Color> specularColor;
        private readonly ProxyMemberProperty<string> effectFileName;
        private readonly ProxyMemberProperty<float> specularPower;
        private readonly ProxyMemberProperty<bool> lightingEnabled;
        private readonly ProxyMemberProperty<bool> drawTransparentPixels;
        private readonly ProxyMemberProperty<int> drawPriority;
    }
}
