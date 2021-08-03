using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.EffectNamespace;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A standard surface that receives fog and light.
    /// </summary>
    public class StandardSurface : Surface<StandardModel>
    {
        /// <summary>
        /// Create a StandardSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        public StandardSurface(OxEngine engine, StandardModel component, string effectFileName)
            : base(engine, component, effectFileName) { }

        /// <summary>
        /// Create a StandardSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public StandardSurface(OxEngine engine, StandardModel component)
            : this(engine, component, "Ox/Effects/oxLightReceiver") { }
        
        /// <summary>
        /// The diffuse map.
        /// May be null.
        /// </summary>
        public Texture2D DiffuseMap { get { return _diffuseMap; } }
        
        /// <summary>
        /// The diffuse map file name.
        /// </summary>
        public string DiffuseMapFileName
        {
            get { return _diffuseMapFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMapFileName == value) return; // OPTIMIZATION
                if (value.IsEmpty()) return;
                Texture2D newDiffuseMap = Engine.Load<Texture2D>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap = newDiffuseMap;
                _diffuseMapFileName = value;
            }
        }

        /// <summary>
        /// The mesh index of the model mesh part to draw.
        /// </summary>
        public int MeshIndex
        {
            get { return meshIndex; }
            set { meshIndex = value; }
        }

        /// <summary>
        /// The part index of the model mesh part to draw.
        /// </summary>
        public int PartIndex
        {
            get { return partIndex; }
            set { partIndex = value; }
        }

        /// <summary>
        /// The model.
        /// </summary>
        protected Model Model { get { return Component.Model; } }

        /// <summary>
        /// The mesh.
        /// </summary>
        protected ModelMesh Mesh { get { return Model.Meshes[meshIndex]; } }

        /// <summary>
        /// The model mesh part to draw.
        /// </summary>
        protected ModelMeshPart Part { get { return Mesh.MeshParts[partIndex]; } }

        /// <summary>
        /// The effect that the model mesh part was loaded with.
        /// </summary>
        protected Effect OriginalEffect { get { return Part.Effect; } }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return _effect; } }

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

        /// <summary>
        /// Handle creating the effect with the specified file name.
        /// </summary>
        protected virtual Effect CreateEffectHook(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new LightReceiverEffect(Engine.GraphicsDevice, effectFromDisk);
        }

        /// <summary>
        /// Handle populating the effect's parameters.
        /// </summary>
        protected virtual void PopulateEffectHook(GameTime gameTime, Camera camera, string drawMode)
        {
            LightReceiverEffect lrEffect = OxHelper.Cast<LightReceiverEffect>(Effect);
            PopulateEffectTransform(camera, lrEffect);
            if (drawMode == "Normal")
            {
                PopulateEffectDiffuseMap(lrEffect);
                PopulateEffectFogging(lrEffect);
                PopulateEffectLighting(lrEffect);
            }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            PopulateEffect(gameTime, camera, drawMode);
            // OPTIMIZATION: cache these properties
            GraphicsDevice device = Engine.GraphicsDevice;
            ModelMesh mesh = Mesh;
            ModelMeshPart part = Part;
            Effect effect = Effect;
            EffectPassCollection passes = effect.CurrentTechnique.Passes;

            BeginRenderState();
            {
                effect.Begin();
                {
                    for (int i = 0; i < passes.Count; ++i)
                    {
                        EffectPass pass = passes[i]; pass.Begin();
                        {
                            device.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                            device.Indices = mesh.IndexBuffer;
                            device.VertexDeclaration = part.VertexDeclaration;
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0,
                                part.NumVertices, part.StartIndex, part.PrimitiveCount);
                        }
                        pass.End();
                    }
                }
                effect.End();
            }
            EndRenderState();
        }

        private Effect CreateEffect(string effectFileName)
        {
            return CreateEffectHook(effectFileName);
        }

        private void PopulateEffect(GameTime gameTime, Camera camera, string drawMode)
        {
            Effect.TrySetCurrentTechnique(drawMode);
            PopulateEffectHook(gameTime, camera, drawMode);
        }

        private void PopulateEffectTransform(Camera camera, LightReceiverEffect lrEffect)
        {
            Matrix modelWorld; GetTransformWorldScaled(out modelWorld);
            Matrix boneAbsolute; Component.GetBoneAbsolute(Mesh.ParentBone.Index, out boneAbsolute);
            Matrix world; Matrix.Multiply(ref boneAbsolute, ref modelWorld, out world);
            lrEffect.PopulateTransform(camera, ref world);
        }

        private void PopulateEffectDiffuseMap(LightReceiverEffect lrEffect)
        {
            // set diffuse map to manually configured diffuse map if available
            if (DiffuseMap != null) lrEffect.DiffuseMap = DiffuseMap;
            // if no diffuse map set, set from imported diffuse map
            if (lrEffect.DiffuseMap == null) lrEffect.DiffuseMap = OxHelper.Cast<BasicEffect>(OriginalEffect).Texture;
        }

        private void PopulateEffectFogging(LightReceiverEffect lrEffect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            lrEffect.PopulateFogging(sceneSystem.Fog);
        }

        private void PopulateEffectLighting(LightReceiverEffect lrEffect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            lrEffect.PopulateLighting(this, sceneSystem.CachedAmbientLights,
                sceneSystem.CachedDirectionalLights, sceneSystem.CachedPointLights);
        }

        private void GetBoneAbsolute(out Matrix transform)
        {
            Component.GetBoneAbsolute(Mesh.ParentBone.Index, out transform);
        }

        private void BeginRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.BeginFaceMode(FaceMode);
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                if (!DrawTransparentPixels)
                {
                    device.RenderState.AlphaTestEnable = true;
                    device.RenderState.AlphaFunction = CompareFunction.Greater;
                }
            }
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.EndFaceMode();
            if (DrawStyle == DrawStyle.Transparent)
            {
                device.RenderState.AlphaBlendEnable = false;
                device.RenderState.SourceBlend = Blend.One;
                device.RenderState.DestinationBlend = Blend.Zero;
                if (!DrawTransparentPixels)
                {
                    device.RenderState.AlphaTestEnable = false;
                    device.RenderState.AlphaFunction = CompareFunction.Always;
                }
            }
        }
        
        private int meshIndex;
        private int partIndex;
        /// <summary>May be null.</summary>
        private Texture2D _diffuseMap;
        private Effect _effect;
        private string _diffuseMapFileName = string.Empty;
        private string _effectFileName = string.Empty;
    }
}
