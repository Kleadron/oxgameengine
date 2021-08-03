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
    /// A surface of a BasicModel.
    /// </summary>
    public class BasicSurface : Surface<BasicModel>
    {
        /// <summary>
        /// Create a BasicSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public BasicSurface(OxEngine engine, BasicModel component)
            : base(engine, component, string.Empty) { }

        /// <summary>
        /// The index of the model mesh that the surface draws.
        /// </summary>
        public int MeshIndex
        {
            get { return meshIndex; }
            set { meshIndex = value; }
        }

        /// <summary>
        /// The index of the model mesh part that the surface draws.
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
        /// The model mesh part that the surface draws.
        /// </summary>
        protected ModelMeshPart Part { get { return Mesh.MeshParts[partIndex]; } }

        /// <inheritdoc />
        protected override Effect EffectHook { get { return Part.Effect; } }

        /// <inheritdoc />
        protected override string EffectFileNameHook
        {
            get { return string.Empty; }
            set { }
        }

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;
            PopulateEffect(camera, drawMode);
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

        private void PopulateEffect(Camera camera, string drawMode)
        {
            BasicEffect basicEffect = OxHelper.Cast<BasicEffect>(Effect);
            basicEffect.TrySetCurrentTechnique("BasicEffect");
            PopulateEffectTransform(camera, basicEffect);
            PopulateEffectFogging(basicEffect);
            PopulateEffectLighting(basicEffect);
        }

        private void PopulateEffectTransform(Camera camera, BasicEffect basicEffect)
        {
            Matrix modelWorld; GetTransformWorldScaled(out modelWorld);
            Matrix boneAbsolute; GetBoneAbsolute(out boneAbsolute);
            Matrix world; Matrix.Multiply(ref boneAbsolute, ref modelWorld, out world);
            basicEffect.PopulateTransform(camera, ref world);
        }

        private void PopulateEffectFogging(BasicEffect basicEffect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            basicEffect.PopulateFogging(sceneSystem.Fog);
        }

        private void PopulateEffectLighting(BasicEffect basicEffect)
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            basicEffect.PopulateLighting(this, sceneSystem.CachedAmbientLights,
                sceneSystem.CachedDirectionalLights, sceneSystem.CachedPointLights);
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

        private void GetBoneAbsolute(out Matrix transform)
        {
            Component.GetBoneAbsolute(Mesh.ParentBone.Index, out transform);
        }

        private int meshIndex;
        private int partIndex;
    }
}
