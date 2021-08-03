using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;
using XNAnimation;
using XNAnimation.Controllers;

namespace Ox.Scene.Component
{
    /// <summary>
    /// Various default values use by AnimatedModels.
    /// </summary>
    public static class AnimatedModelDefaults
    {
        public const string ModelFileName = "Ox/Models/PlayerMarine";
        public const string AnimationClip = "";
        public const bool NormalMapEnabled = true;
    }

    /// <summary>
    /// An animated model that can be placed in a scene.
    /// </summary>
    public class AnimatedModel : SingleSurfaceComponent<AnimatedSurface>
    {
        /// <summary>
        /// Create an AnimatedModel.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="skinnedModelFileName">See property SkinnedModelFileName.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the model.</param>
        public AnimatedModel(OxEngine engine, string domainName, bool ownedByDomain,
            string skinnedModelFileName, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, primitive)
        {
            OxHelper.ArgumentNullCheck(skinnedModelFileName);
            SkinnedModelFileName = skinnedModelFileName;
            // populate surface AFTER becoming an otherwise valid object
            AddGarbage(surface = engine.GetService<SurfaceFactory>().CreateSurface<AnimatedSurface>(this));
        }

        /// <summary>
        /// The animation controller. All changes to AnimationController are discarded when
        /// SkinnedModelFileName is changed.
        /// </summary>
        public IAnimationController AnimationController { get { return _animationController; } }

        /// <summary>
        /// The skinned model. All changes to Model are discarded when SkinnedModelFileName is
        /// changed.
        /// </summary>
        public SkinnedModel SkinnedModel { get { return _skinnedModel; } }

        /// <summary>
        /// The name of the skinned model file.
        /// </summary>
        public string SkinnedModelFileName
        {
            get { return _skinnedModelFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_skinnedModelFileName == value) return; // OPTIMIZATION
                SkinnedModel newSkinnedModel = Engine.Load<SkinnedModel>(value, DomainName);
                IAnimationController newAnimationController = new AnimationController(newSkinnedModel.SkeletonBones);
                // EXCEPTIONSAFETYLINE
                _skinnedModel = newSkinnedModel;
                _animationController = newAnimationController;
                _skinnedModelFileName = value;
            }
        }

        /// <summary>
        /// A bone transform in absolute space.
        /// </summary>
        public void GetBoneAbsolute(int boneIndex, out Matrix transform)
        {
            // get the bone absolute transform with undesired local scaling
            Matrix inverseBindPoseTransform = SkinnedModel.SkeletonBones[boneIndex].InverseBindPoseTransform;
            Matrix bindPoseTransform; Matrix.Invert(ref inverseBindPoseTransform, out bindPoseTransform);
            Matrix skinnedBoneTransform = AnimationController.SkinnedBoneTransforms[boneIndex];
            Matrix transformScaled; Matrix.Multiply(ref bindPoseTransform, ref skinnedBoneTransform, out transformScaled);

            // get the desired bone absolute transform by removing local scaling
            Vector3 inverseScaleVector = Vector3.One / Scale;
            Matrix inverseScale; Matrix.CreateScale(ref inverseScaleVector, out inverseScale);
            Matrix.Multiply(ref inverseScale, ref transformScaled, out transform);
        }

        /// <summary>
        /// A bone transform in absolute world space.
        /// </summary>
        public void GetBoneAbsoluteWorld(int boneIndex, out Matrix transform)
        {
            Matrix transformWorld; GetTransformWorldScaled(out transformWorld);
            Matrix boneAbsolute; GetBoneAbsolute(boneIndex, out boneAbsolute);
            Matrix.Multiply(ref boneAbsolute, ref transformWorld, out transform);
        }

        /// <inheritdoc />
        protected override AnimatedSurface SurfaceHook { get { return surface; } }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new AnimatedModelToken();
        }

        /// <inheritdoc />
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            AnimationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            UpdateWorldPropertyOfChildren("Transform"); // update transform of children mounted to bones
        }

        /// <inheritdoc />
        protected override bool TryGenerateBoundingBoxHook(out BoundingBox boundingBox)
        {
            boundingBox = SkinnedModel.Model.GenerateBoundingBox();
            return true;
        }

        /// <inheritdoc />
        protected override void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            if (IsBoneMount(mountPoint)) GetBoneAbsoluteWorld(mountPoint - 1, out transform);
            else base.GetMountPointTransformHook(mountPoint, out transform);
        }

        private bool IsBoneMount(int mountPoint)
        {
            return mountPoint > 0 && mountPoint < SkinnedModel.SkeletonBones.Count + 1;
        }

        private readonly AnimatedSurface surface;
        private IAnimationController _animationController;
        private SkinnedModel _skinnedModel;
        private string _skinnedModelFileName;
    }
}
