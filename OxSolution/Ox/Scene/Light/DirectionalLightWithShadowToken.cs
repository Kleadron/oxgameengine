using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Scene.LightNamespace
{
    /// <summary>
    /// A serializable token for a DirectionalLight with shadowing enabled.
    /// </summary>
    public class DirectionalLightWithShadowToken : DirectionalLightToken
    {
        /// <summary>
        /// Create a DirectionalLightWithShadowToken.
        /// </summary>
        public DirectionalLightWithShadowToken()
        {
            shadowCameraPosition = new ProxyProperty<Vector3>(this, "ShadowCameraPosition", Vector3.Up * SceneConfiguration.DirectionalShadowRange * 0.5f);
            shadowCameraOffset = new ProxyProperty<float>(this, "ShadowCameraOffset", SceneConfiguration.DirectionalShadowRange * 0.5f);
            shadowCameraSnap = new ProxyProperty<float>(this, "ShadowCameraSnap", 4);
            shadowCameraRelativeToViewCamera = new ProxyProperty<bool>(this, "ShadowCameraRelativeToViewCamera", true);
            shadowEnabled = new ProxyMemberProperty<bool>(this, "Shadow", "Enabled", true);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new DirectionalLight Instance
        {
            get { return OxHelper.Cast<DirectionalLight>(base.Instance); }
            set { base.Instance = value; }
        }

        public Vector3 ShadowCameraPosition
        {
            get { return shadowCameraPosition.Value; }
            set { shadowCameraPosition.Value = value; }
        }
        
        public float ShadowCameraOffset
        {
            get { return shadowCameraOffset.Value; }
            set { shadowCameraOffset.Value = value; }
        }
        
        [DefaultValue(4)]
        public float ShadowCameraSnap
        {
            get { return shadowCameraSnap.Value; }
            set { shadowCameraSnap.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool ShadowCameraRelativeToViewCamera
        {
            get { return shadowCameraRelativeToViewCamera.Value; }
            set { shadowCameraRelativeToViewCamera.Value = value; }
        }
        
        [DefaultValue(true)]
        public bool ShadowEnabled
        {
            get { return shadowEnabled.Value; }
            set { shadowEnabled.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new DirectionalLight(engine, domainName, ownedByDomain, true);
        }

        private readonly ProxyProperty<Vector3> shadowCameraPosition;
        private readonly ProxyProperty<float> shadowCameraOffset;
        private readonly ProxyProperty<float> shadowCameraSnap;
        private readonly ProxyProperty<bool> shadowCameraRelativeToViewCamera;
        private readonly ProxyMemberProperty<bool> shadowEnabled;
    }
}
