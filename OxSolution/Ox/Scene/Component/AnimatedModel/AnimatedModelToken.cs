using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;
using XNAnimation;
using Microsoft.Xna.Framework.Graphics;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A serializable token for an AnimatedModel.
    /// </summary>
    public class AnimatedModelToken : SingleSurfaceComponentToken<AnimatedSurface>
    {
        /// <summary>
        /// Create an AnimatedModelToken.
        /// </summary>
        public AnimatedModelToken() : base(string.Empty)
        {
            skinnedModelFileName = new ProxyProperty<string>(
                this, "SkinnedModelFileName", AnimatedModelDefaults.ModelFileName);
            animationClip = new ProxyUserProperty<string, AnimatedModel>(
                this, AnimationClipGetter, AnimationClipSetter, "AnimationClip", AnimatedModelDefaults.AnimationClip);
            emissiveColor = new ProxyMemberProperty<Color>(
                this, "Surface", "EmissiveColor", new Color());
            normalMapEnabled = new ProxyMemberProperty<bool>(
                this, "Surface", "NormalMapEnabled", AnimatedModelDefaults.NormalMapEnabled);
        }

        /// <inheritdoc />
        [XmlIgnore, Browsable(false)]
        public new AnimatedModel Instance
        {
            get { return OxHelper.Cast<AnimatedModel>(base.Instance); }
            set { base.Instance = value; }
        }
        
        [DefaultValue(AnimatedModelDefaults.ModelFileName)]
        [Editor(typeof(ContentFileNameEditor), typeof(UITypeEditor))]
        public string SkinnedModelFileName
        {
            get { return skinnedModelFileName.Value; }
            set { skinnedModelFileName.Value = value; }
        }
        
        [DefaultValue(AnimatedModelDefaults.AnimationClip)]
        public string AnimationClip
        {
            get { return animationClip.Value; }
            set { animationClip.Value = value; }
        }

        [DefaultValue(AnimatedModelDefaults.NormalMapEnabled)]
        public Color EmissiveColor
        {
            get { return emissiveColor.Value; }
            set { emissiveColor.Value = value; }
        }
        
        [DefaultValue(AnimatedModelDefaults.NormalMapEnabled)]
        public bool NormalMapEnabled
        {
            get { return normalMapEnabled.Value; }
            set { normalMapEnabled.Value = value; }
        }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new AnimatedModel(engine, domainName, ownedByDomain, SkinnedModelFileName, BoxPrimitive);
        }

        private string AnimationClipGetter(AnimatedModel y)
        {
            AnimationClip clip = y.AnimationController.AnimationClip;
            return clip == null ? default(string) : clip.Name;
        }

        private void AnimationClipSetter(string x, AnimatedModel y)
        {
            AnimationClip clip;
            if (y.SkinnedModel.AnimationClips.TryGetValue(x, out clip))
                y.AnimationController.PlayClip(clip);
        }

        private readonly ProxyMemberProperty<Color> emissiveColor;
        private readonly ProxyMemberProperty<bool> normalMapEnabled;
        private readonly ProxyUserProperty<string, AnimatedModel> animationClip;
        private readonly ProxyProperty<string> skinnedModelFileName;
    }
}
