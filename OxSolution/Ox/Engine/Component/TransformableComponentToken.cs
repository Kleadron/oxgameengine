using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Primitive;

namespace Ox.Engine.Component
{
    /// <summary>
    /// A serializable transformable component token.
    /// </summary>
    public class TransformableComponentToken : UpdateableComponentToken
    {
        /// <summary>
        /// Create a TransformableComponentToken.
        /// </summary>
        public TransformableComponentToken()
        {
            position = new ProxyProperty<Vector3>(this, "Position");
            scale = new ProxyProperty<Vector3>(this, "Scale", Vector3.One);
            orientation = new ProxyOrientationProperty(this, "Orientation");
            box = new ProxyBoxProperty(this, "Box", new Box(Vector3.Zero, new Vector3(0.5f)));
            mountPoint = new ProxyProperty<int>(this, "MountPoint", 0);
        }
        
        /// <summary>May be null.</summary>
        [XmlIgnore, Browsable(false)]
        public new TransformableComponent Instance
        {
            get { return OxHelper.Cast<TransformableComponent>(base.Instance); }
            set { base.Instance = value; }
        }
        
        public Vector3 Position
        {
            get { return position.Value; }
            set { position.Value = value; }
        }
        
        public Vector3 Scale
        {
            get { return scale.Value; }
            set { scale.Value = value; }
        }
        
        public Vector3 Orientation
        {
            get { return orientation.ValueDesignTime; }
            set { orientation.ValueDesignTime = value; }
        }
        
        [Browsable(false), DefaultValue(null)]
        public Matrix? OrientationPlayTime
        {
            get { return orientation.ValuePlayTime; }
            set { orientation.ValuePlayTime = value; }
        }

        public Box Box
        {
            get { return box.Value; }
            set { box.Value = value; }
        }

        [DefaultValue(0)]
        public int MountPoint
        {
            get { return mountPoint.Value; }
            set { mountPoint.Value = value; }
        }

        /// <summary>
        /// The box primitive.
        /// </summary>
        protected IPrimitive BoxPrimitive { get { return box.Primitive; } }

        /// <inheritdoc />
        protected override OxComponent CreateInstanceHook(OxEngine engine, string domainName, bool ownedByDomain)
        {
            return new TransformableComponent(engine, domainName, ownedByDomain, box.Primitive);
        }

        private readonly ProxyProperty<Vector3> position;
        private readonly ProxyProperty<Vector3> scale;
        private readonly ProxyOrientationProperty orientation;
        private readonly ProxyBoxProperty box;
        private readonly ProxyProperty<int> mountPoint;
    }
}
