using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Primitive;
using Ox.Engine.Utility;
using Ox.Scene.EffectNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of a Terrain.
    /// </summary>
    public class TerrainSurface : Surface<Terrain>
    {
        /// <summary>
        /// Create a TerrainSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
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
        public TerrainSurface(OxEngine engine, Terrain component,
            Vector3 quadScale, Point gridDims, float smoothingFactor, string effectFileName,
            string heightMapFileName, string diffuseMap0FileName, string diffuseMap1FileName,
            string diffuseMap2FileName, string diffuseMap3FileName, Vector2 textureRepetition)
            : base(engine, component, effectFileName)
        {
            OxHelper.ArgumentNullCheck(heightMapFileName, diffuseMap0FileName, diffuseMap1FileName);
            OxHelper.ArgumentNullCheck(diffuseMap2FileName, diffuseMap3FileName);
            DiffuseMap0FileName = diffuseMap0FileName;
            DiffuseMap1FileName = diffuseMap1FileName;
            DiffuseMap2FileName = diffuseMap2FileName;
            DiffuseMap3FileName = diffuseMap3FileName;
            // OPTIMIZATION: circumvent properties
            {
                _quadScale = quadScale;
                _gridDims = gridDims;
                _smoothingFactor = smoothingFactor;
                _textureRepetition = textureRepetition;
            }
            // this property mutation will call RecreateInnards as needed since we circumvented
            // the properties
            HeightMapFileName = heightMapFileName;
        }

        /// <summary>
        /// The height map that represents the topography of the terrain.
        /// </summary>
        public HeightMap HeightMap
        {
            get { return _heightMap; }
        }

        /// <summary>
        /// The scale of each geometry quad.
        /// </summary>
        public Vector3 QuadScale
        {
            get { return _quadScale; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue); // VALIDATION
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue); // VALIDATION
                value.Z = MathHelper.Clamp(value.Z, 0.001f, float.MaxValue); // VALIDATION
                if (_quadScale == value) return; // OPTIMIZATION
                _quadScale = value;
                RecreatePatches();
            }
        }

        /// <summary>
        /// The scale of each terrain patch.
        /// </summary>
        public Vector3 PatchScale
        {
            get { return new Vector3(PatchDims.X, 1, PatchDims.Y) * QuadScale; }
        }

        /// <summary>
        /// The scale of the grid.
        /// </summary>
        public Vector3 GridScale
        {
            get { return new Vector3(GridDims.X, 1, GridDims.Y) * PatchScale; }
        }

        /// <summary>
        /// The offset used to center the terrain on the [x, z] origin.
        /// </summary>
        public Vector2 GridCenterOffset
        {
            get { return new Vector2(GridScale.X * -0.5f, GridScale.Z * -0.5f);; }
        }

        /// <summary>
        /// The number of times the terrain texturing repeats.
        /// </summary>
        public Vector2 TextureRepetition
        {
            get { return _textureRepetition; }
            set
            {
                value.X = MathHelper.Clamp(value.X, 0.001f, float.MaxValue); // VALIDATION
                value.Y = MathHelper.Clamp(value.Y, 0.001f, float.MaxValue); // VALIDATION
                if (_textureRepetition == value) return; // OPTIMIZATION
                _textureRepetition = value;
                RecreatePatches();
            }
        }

        /// <summary>
        /// The number of quads in each patch.
        /// </summary>
        public Point PatchDims
        {
            get { return new Point((HeightMapTexture.Width - 1) / GridDims.X, (HeightMapTexture.Height - 1) / GridDims.Y); }
        }

        /// <summary>
        /// The number of patches in the terrain.
        /// </summary>
        public Point GridDims
        {
            get { return _gridDims; }
            set
            {
                value.X = (int)MathHelper.Clamp(value.X, 1, 128); // VALIDATION
                value.Y = (int)MathHelper.Clamp(value.Y, 1, 128); // VALIDATION
                if (_gridDims == value) return; // OPTIMIZATION
                _gridDims = value;
                RecreatePatches();
            }
        }
        
        /// <summary>
        /// The name of the texture file that defines the topography.
        /// </summary>
        public string HeightMapFileName
        {
            get { return _heightMapFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_heightMapFileName == value) return; // OPTIMIZATION
                Texture2D newHeightMapTexture = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _heightMapTexture = newHeightMapTexture;
                _heightMapFileName = value;
                RecreatePatches();
            }
        }
        
        /// <summary>
        /// The name of the 0th diffuse map file.
        /// </summary>
        public string DiffuseMap0FileName
        {
            get { return _diffuseMap0FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMap0FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap0 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap0 = newDiffuseMap0;
                _diffuseMap0FileName = value;
            }
        }
        
        /// <summary>
        /// The name of the 1st diffuse map file.
        /// </summary>
        public string DiffuseMap1FileName
        {
            get { return _diffuseMap1FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMap1FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap1 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap1 = newDiffuseMap1;
                _diffuseMap1FileName = value;
            }
        }
        
        /// <summary>
        /// The name of the 2nd texture file.
        /// </summary>
        public string DiffuseMap2FileName
        {
            get { return _diffuseMap2FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMap2FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap2 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap2 = newDiffuseMap2;
                _diffuseMap2FileName = value;
            }
        }
        
        /// <summary>
        /// The name of the 3rd texture file.
        /// </summary>
        public string DiffuseMap3FileName
        {
            get { return _diffuseMap3FileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMap3FileName == value) return; // OPTIMIZATION
                Texture2D newDiffuseMap3 = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap3 = newDiffuseMap3;
                _diffuseMap3FileName = value;
            }
        }
        
        /// <summary>
        /// The amount by which the topography is smoothened.
        /// </summary>
        public float SmoothingFactor
        {
            get { return _smoothingFactor; }
            set
            {
                value = MathHelper.Clamp(value, 0, 1); // VALIDATION
                if (_smoothingFactor == value) return; // OPTIMIZATION
                _smoothingFactor = value;
                RecreatePatches();
            }
        }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return _effectFileName; }
            set
            {
                if (_effectFileName == value) return; // OPTIMIZATION
                Effect newEffect = CreateEffect(value);
                // EXCEPTIONSAFETYLINE
                _effect = newEffect;
                _effectFileName = value;
            }
        }

        /// <inheritdoc />
        protected override Effect EffectHook
        {
            get { return _effect; }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            GraphicsDevice device = Engine.GraphicsDevice;
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            ShadowReceiverEffect srEffect = OxHelper.Cast<ShadowReceiverEffect>(Effect);
            srEffect.TrySetCurrentTechnique(drawMode);
            PopulateEffect(camera, drawMode, srEffect);
            // OPTIMIZATION: cache these properties
            EffectPassCollection passes = srEffect.CurrentTechnique.Passes;
            IPrimitive[,] patchPrimitives = PatchPrimitives;
            Geometry[,] patches = Patches;

            BeginRenderState(device);
            {
                srEffect.Begin();
                {
                    for (int i = 0; i < passes.Count; ++i)
                    {
                        EffectPass pass = passes[i]; pass.Begin();
                        {
                            for (int j = 0; j < patches.GetLength(0); ++j)
                            {
                                for (int k = 0; k < patches.GetLength(1); ++k)
                                {
                                    IPrimitive patchPrimitive = patchPrimitives[j, k];
                                    Geometry patch = patches[j, k];
                                    if (camera.Contains(patchPrimitive.BoundingBox) != ContainmentType.Disjoint)
                                    {
                                        PopulateEffectPerPatch(camera, drawMode, srEffect);
                                        patch.Draw(gameTime);
                                    }
                                }
                            }
                        }
                        pass.End();
                    }
                }
                srEffect.End();
            }
            EndRenderState(device);
        }

        private IPrimitive[,] PatchPrimitives { get { return _patchPrimitives; } }

        private Geometry[,] Patches { get { return _patches; } }

        private Texture2D HeightMapTexture { get { return _heightMapTexture; } }

        private Texture2D DiffuseMap0 { get { return _diffuseMap0; } }

        private Texture2D DiffuseMap1 { get { return _diffuseMap1; } }

        private Texture2D DiffuseMap2 { get { return _diffuseMap2; } }

        private Texture2D DiffuseMap3 { get { return _diffuseMap3; } }

        private Vector2 QuadTextureScale
        {
            get
            {
                Vector2 gridDims = new Vector2(GridDims.X, GridDims.Y);
                Vector2 patchDims = new Vector2(PatchDims.X, PatchDims.Y);
                return TextureRepetition / gridDims / patchDims;
            }
        }
        
        private string HeightMapTooSmallMessage
        {
            get
            {
                return
                    "The height map requires a resolution of at least " +
                    (GridDims.X + 1) + "x" + (GridDims.Y + 1) + ".";
            }
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new ShadowReceiverEffect(Engine.GraphicsDevice, effectFromDisk);
        }

        private HeightMap CreateHeightMap()
        {
            HeightMap rawHeightMap = TerrainPatchGeometry.TextureToHeightMap(HeightMapTexture, QuadScale, GridCenterOffset);
            if (IsHeightMapTooSmall(rawHeightMap)) throw new FormatException(HeightMapTooSmallMessage);
            return TerrainPatchGeometry.SmoothenHeightMap(rawHeightMap, SmoothingFactor);
        }

        private void RecreatePatches()
        {
            _heightMap = CreateHeightMap();
            DestroyPatches();
            CreatePatches();
        }

        private void CreatePatches()
        {
            _patches = new Geometry[GridDims.X, GridDims.Y];
            _patchPrimitives = new IPrimitive[GridDims.X, GridDims.Y];
            for (int i = 0; i < GridDims.X; ++i)
            {
                for (int j = 0; j < GridDims.Y; ++j)
                {
                    CreatePatch(i, j);
                    CreatePatchPrimitive(i, j);
                }
            }
        }

        private void DestroyPatches()
        {
            for (int i = 0; i < Patches.GetLength(0); ++i)
            {
                for (int j = 0; j < Patches.GetLength(1); ++j)
                {
                    DestroyPatch(i, j);
                    DestroyPatchPrimitive(i, j);
                }
            }
            _patches = new Geometry[0, 0];
            _patchPrimitives = new IPrimitive[0, 0];
        }

        private void CreatePatch(int i, int j)
        {
            VertexFactory vertexFactory = Engine.GetService<VertexFactory>();
            Rectangle terrainPortion = new Rectangle(i * PatchDims.X, j * PatchDims.Y, PatchDims.X + 1, PatchDims.Y + 1);
            Geometry patch = TerrainPatchGeometry.Create(Engine.GraphicsDevice, vertexFactory, "PositionNormalTexture", // MAGICVALUE
                HeightMap, terrainPortion, QuadScale, QuadTextureScale);
            AddGarbage(patch);
            Patches[i, j] = patch;
        }

        private void CreatePatchPrimitive(int i, int j)
        {
            Vector3 patchMin = CalculatePatchPosition(i, j);
            Vector3 patchMax = patchMin + PatchScale;
            BoundingBox patchBox = new BoundingBox(patchMin, patchMax);
            IPrimitive patchPrimitive = new BoxPrimitive(patchBox);
            PatchPrimitives[i, j] = patchPrimitive;
        }

        private void DestroyPatch(int i, int j)
        {
            Geometry patch = Patches[i, j];
            if (patch == null) return;
            RemoveGarbage(patch);
            patch.Dispose();
            Patches[i, j] = null;
        }

        private void DestroyPatchPrimitive(int i, int j)
        {
            PatchPrimitives[i, j] = null;
        }

        private void BeginRenderState(GraphicsDevice device)
        {
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState(GraphicsDevice device)
        {
            device.EndFaceMode();
        }

        private void PopulateEffect(Camera camera, string drawMode, ShadowReceiverEffect effect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            effect.PopulateTransformWorld(camera);
            if (drawMode == "Normal")
            {
                effect.PopulateFogging(sceneSystem.Fog);
                effect.PopulateLighting(this, sceneSystem.CachedAmbientLights,
                    sceneSystem.CachedDirectionalLights, sceneSystem.CachedPointLights);
                effect.Parameters["xHeightMax"].TrySetValue(QuadScale.Y);
                effect.Parameters["xDiffuseMap0"].TrySetValue(DiffuseMap0);
                effect.Parameters["xDiffuseMap1"].TrySetValue(DiffuseMap1);
                effect.Parameters["xDiffuseMap2"].TrySetValue(DiffuseMap2);
                effect.Parameters["xDiffuseMap3"].TrySetValue(DiffuseMap3);
            }
        }

        private void PopulateEffectPerPatch(Camera camera, string drawMode, ShadowReceiverEffect effect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            Matrix world = Matrix.Identity;
            effect.PopulateTransformLocal(camera, ref world);
            effect.PopulateShadowing(this, sceneSystem.CachedDirectionalLights);
            effect.CommitChanges();
        }

        private bool IsHeightMapTooSmall(HeightMap heightMap)
        {
            return
                heightMap.GetLength(0) < GridDims.X + 1 ||
                heightMap.GetLength(1) < GridDims.Y + 1;
        }

        private Vector3 CalculatePatchPosition(int i, int j)
        {
            return new Vector3(
                PatchDims.X * QuadScale.X * i + GridCenterOffset.X,
                0,
                PatchDims.Y * QuadScale.Z * j + GridCenterOffset.Y);
        }
        
        private IPrimitive[,] _patchPrimitives = new IPrimitive[0, 0];
        private Geometry[,] _patches = new Geometry[0, 0];
        private HeightMap _heightMap;
        private Texture2D _heightMapTexture;
        private Texture2D _diffuseMap0;
        private Texture2D _diffuseMap1;
        private Texture2D _diffuseMap2;
        private Texture2D _diffuseMap3;
        private Vector3 _quadScale;
        private Vector2 _textureRepetition;
        private Effect _effect;
        private Point _gridDims;
        private string _effectFileName;
        private string _heightMapFileName;
        private string _diffuseMap0FileName;
        private string _diffuseMap1FileName;
        private string _diffuseMap2FileName;
        private string _diffuseMap3FileName;
        private float _smoothingFactor;
    }
}
