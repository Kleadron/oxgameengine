using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Primitive;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A model that leverages XNA's BasicEffect.
    /// </summary>
    public class BasicModel : SceneComponent
    {
        /// <summary>
        /// Create a BasicModel.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="domainName">See property DomainName.</param>
        /// <param name="ownedByDomain">See property OwnedByDomain.</param>
        /// <param name="primitive">The transformable primitive that describes the space occupied by the model.</param>
        public BasicModel(OxEngine engine, string domainName, bool ownedByDomain, IPrimitive primitive)
            : base(engine, domainName, ownedByDomain, primitive)
        {
            ModelFileName = "Ox/Models/cube";
        }

        /// <summary>
        /// The XNA Model.
        /// </summary>
        public Model Model { get { return _model; } }

        /// <summary>
        /// The name of the model.
        /// </summary>
        public string ModelFileName
        {
            get { return _modelFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_modelFileName == value) return; // OPTIMIZATION
                Model newModel = Engine.Load<Model>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _model = newModel;
                _modelFileName = value;
                RecreateSurfaces();
                RefreshBonesAbsolute();
            }
        }

        /// <summary>
        /// Get the surface at [meshIndex, partIndex].
        /// </summary>
        public BasicSurface GetSurface(int meshIndex, int partIndex)
        {
            BasicSurface result;
            if (TryGetSurface(meshIndex, partIndex, out result)) return result;
            throw new InvalidOperationException("BasicModel is missing a surface for a ModelMeshPart.");
        }

        /// <summary>
        /// A bone transform in absolute space.
        /// </summary>
        public void GetBoneAbsolute(int boneIndex, out Matrix transform)
        {
            transform = bonesAbsolute[boneIndex];
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
        protected override void UpdateHook(GameTime gameTime)
        {
            base.UpdateHook(gameTime);
            RefreshBonesAbsolute();
        }

        /// <inheritdoc />
        protected override ComponentToken CreateComponentTokenHook()
        {
            return new BasicModelToken();
        }

        /// <inheritdoc />
        protected override bool TryGenerateBoundingBoxHook(out BoundingBox boundingBox)
        {
            boundingBox = Model.GenerateBoundingBox();
            return true;
        }

        /// <inheritdoc />
        protected override void GetMountPointTransformHook(int mountPoint, out Matrix transform)
        {
            if (IsBoneMount(mountPoint)) GetBoneAbsoluteWorld(mountPoint - 1, out transform);
            else base.GetMountPointTransformHook(mountPoint, out transform);
        }

        private void RecreateSurfaces()
        {
            DestroySurfaces();
            CreateSurfaces();
        }

        private void CreateSurfaces()
        {
            for (int i = 0; i < Model.Meshes.Count; ++i)
                for (int j = 0; j < Model.Meshes[i].MeshParts.Count; ++j)
                    CreateSurface(i, j);
        }

        private void DestroySurfaces()
        {
            for (int i = 0; i < Model.Meshes.Count; ++i)
                for (int j = 0; j < Model.Meshes[i].MeshParts.Count; ++j)
                    DestroySurface(i, j);
        }

        private void CreateSurface(int meshIndex, int partIndex)
        {
            BasicSurface surface = Engine.GetService<SurfaceFactory>().CreateSurface <BasicSurface>(this);
            surface.MeshIndex = meshIndex;
            surface.PartIndex = partIndex;
        }

        private void DestroySurface(int meshIndex, int partIndex)
        {
            BasicSurface surface;
            if (TryGetSurface(meshIndex, partIndex, out surface)) surface.Dispose();
        }

        private bool TryGetSurface(int meshIndex, int partIndex, out BasicSurface result)
        {
            IList<BasicSurface> surfaces = CollectSubcomponents(
                x => x.MeshIndex == meshIndex && x.PartIndex == partIndex, // MEMORYCHURN
                new List<BasicSurface>()); // MEMORYCHURN
            if (surfaces.Count > 1) throw new InvalidOperationException("BasicModel has more than one surface per ModelMeshPart.");
            if (surfaces.Count == 1)
            {
                result = surfaces[0];
                return true;
            }
            else
            {
                result = default(BasicSurface);
                return false;
            }
        }

        private void RefreshBonesAbsolute()
        {
            EnsureBonesAbsoluteCapacity();
            Model.CopyAbsoluteBoneTransformsTo(bonesAbsolute);
            UpdateWorldPropertyOfChildren("Transform"); // update transform of children mounted to bones
        }

        private void EnsureBonesAbsoluteCapacity()
        {
            int bonesCount = Model.Bones.Count;
            if (bonesAbsolute.Length >= bonesCount) return;
            bonesAbsolute = new Matrix[bonesCount];
        }

        private bool IsBoneMount(int mountPoint)
        {
            return mountPoint > 0 && mountPoint < Model.Bones.Count + 1;
        }

        private Matrix[] bonesAbsolute = new Matrix[0];
        private Model _model;
        private string _modelFileName;
    }
}
