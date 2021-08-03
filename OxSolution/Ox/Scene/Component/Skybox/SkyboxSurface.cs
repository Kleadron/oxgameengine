using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.GeometryNamespace;
using Ox.Engine.Utility;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of a Skybox.
    /// </summary>
    public class SkyboxSurface : Surface<Skybox>
    {
        /// <summary>
        /// Create a SkyboxSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public SkyboxSurface(OxEngine engine, Skybox component)
            : base(engine, component, SkyboxDefaults.EffectFileName)
        {
            VertexFactory vertexFactory = engine.GetService<VertexFactory>();
            AddGarbage(geometry = SkyboxGeometry.Create(engine.GraphicsDevice, vertexFactory, "PositionNormalTexture")); // MAGICVALUE
            DiffuseMapFileName = SkyboxDefaults.DiffuseMapFileName;
            FaceMode = SkyboxDefaults.FaceModeDefault;
            DrawStyle = SkyboxDefaults.DrawStyleDefault;
            DrawProperties = DrawProperties.Reflecting | DrawProperties.DependantTransform;
        }
        
        /// <summary>
        /// The name of the diffuse map file.
        /// </summary>
        public string DiffuseMapFileName
        {
            get { return _diffuseMapFileName; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (_diffuseMapFileName == value) return; // OPTIMIZATION
                TextureCube newDiffuseMap = Engine.Load<TextureCube>(value, DomainName);
                // EXCEPTIONSAFETYLINE
                _diffuseMap = newDiffuseMap;
                _diffuseMapFileName = value;
            }
        }

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

        /// <inheritdoc />
        protected override void PreDrawHook(GameTime gameTime, Camera camera) { }

        /// <inheritdoc />
        protected override void DrawHook(GameTime gameTime, Camera camera, string drawMode)
        {
            if (drawMode != "Normal") return;

            BeginRenderState();
            {
                BeginEffect(camera);
                {
                    DrawGeometryPasses(gameTime);
                }
                EndEffect();
            }
            EndRenderState();
        }

        private TextureCube DiffuseMap { get { return _diffuseMap; } }

        private void BeginRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.RenderState.DepthBufferWriteEnable = false;
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState()
        {
            GraphicsDevice device = Engine.GraphicsDevice;
            device.EndFaceMode();
            device.RenderState.DepthBufferWriteEnable = true;
        }

        private void BeginEffect(Camera camera)
        {
            Matrix world = Matrix.Identity;
            BaseEffect baseEffect = OxHelper.Cast<BaseEffect>(Effect);
            baseEffect.TrySetCurrentTechnique("Normal");
            baseEffect.Parameters["xSkyMap"].TrySetValue(DiffuseMap);
            baseEffect.PopulateTransform(camera, ref world);
            baseEffect.Begin();
        }

        private void EndEffect()
        {
            BaseEffect baseEffect = OxHelper.Cast<BaseEffect>(Effect);
            baseEffect.End();
        }

        private void DrawGeometryPasses(GameTime gameTime)
        {
            EffectPassCollection passes = Effect.CurrentTechnique.Passes;
            for (int i = 0; i < passes.Count; ++i) DrawGeometryPass(gameTime, passes[i]);
        }

        private void DrawGeometryPass(GameTime gameTime, EffectPass pass)
        {
            pass.Begin();
            {
                geometry.Draw(gameTime);
            }
            pass.End();
        }

        private Effect CreateEffect(string effectFileName)
        {
            Effect effectFromDisk = Engine.Load<Effect>(effectFileName, DomainName);
            return new BaseEffect(Engine.GraphicsDevice, effectFromDisk);
        }

        private readonly Geometry geometry;
        private TextureCube _diffuseMap;
        private Effect _effect;
        private string _effectFileName;
        private string _diffuseMapFileName;
    }
}
