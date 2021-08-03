using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.MathNamespace;
using Ox.Engine.Utility;
using Ox.Scene.FogNamespace;
using Ox.Scene.LightNamespace;
using Ox.Scene.SurfaceNamespace;
using XNAnimation;
using XNAnimation.Controllers;
using XNAnimation.Effects;

namespace Ox.Scene.Component
{
    /// <summary>
    /// The surface of an AnimatedModel.
    /// </summary>
    public class AnimatedSurface : Surface<AnimatedModel>
    {
        /// <summary>
        /// Create an AnimatedSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        public AnimatedSurface(OxEngine engine, AnimatedModel component)
            : base(engine, component, string.Empty) { }

        /// <summary>
        /// The material's emissive color.
        /// TODO: consider adding this to Ox standard shader then moving to IMaterial.
        /// </summary>
        public Color EmissiveColor
        {
            get { return emissiveColor; }
            set { emissiveColor = value; }
        }

        /// <summary>
        /// Use normal mapping?
        /// </summary>
        public bool NormalMapEnabled
        {
            get { return normalMapEnabled; }
            set { normalMapEnabled = value; }
        }

        /// <inheritdoc />
        protected override Effect EffectHook
        {
            get { return Component.SkinnedModel.Model.Meshes[0].MeshParts[0].Effect; }
        }

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
            if (drawMode != "Normal" && drawMode != "DirectionalShadow") return;
            IAnimationController animationController = Component.AnimationController;
            GraphicsDevice device = Engine.GraphicsDevice;
            SkinnedModel skinnedModel = Component.SkinnedModel;
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            Matrix world = TransformWorldScaled;
            Matrix view = camera.View;
            Matrix projection = camera.Projection;
            Fog fog = sceneSystem.Fog;

            BeginRenderState(device);
            {
                for (int i = 0; i < skinnedModel.Model.Meshes.Count; ++i)
                {
                    ModelMesh modelMesh = skinnedModel.Model.Meshes[i];
                    for (int j = 0; j < modelMesh.MeshParts.Count; ++j)
                    {
                        SkinnedModelBasicEffect effect =
                            OxHelper.Cast<SkinnedModelBasicEffect>(modelMesh.MeshParts[j].Effect);

                        // world, view, and projection
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;

                        // bones
                        effect.Bones = animationController.SkinnedBoneTransforms;

                        // camera position
                        effect.CameraPosition = camera.Position;

                        // draw mode
                        effect.DrawMode = drawMode;

                        if (drawMode == "Normal")
                        {
                            // material
                            effect.Material.EmissiveColor = EmissiveColor.ToVector3();
                            effect.Material.DiffuseColor = DiffuseColor.ToVector3();
                            effect.Material.SpecularColor = SpecularColor.ToVector3();
                            effect.Material.SpecularPower = SpecularPower;

                            // normal mapping
                            effect.NormalMapEnabled = normalMapEnabled;

                            // fogging
                            effect.FogEnabled = fog.Enabled;

                            if (fog.Enabled) // OPTIMIZATION
                            {
                                effect.FogStart = fog.Start;
                                effect.FogEnd = fog.End;
                                effect.FogColor = fog.Color;
                            }

                            // lighting enabled
                            effect.LightEnabled = LightingEnabled;

                            // light count
                            effect.EnabledLights = skinnedModelEnabledLights;

                            // ambient light
                            Vector3 ambientLightColor = Vector3.Zero;
                            for (int k = 0; k < sceneSystem.CachedAmbientLights.Count; ++k)
                            {
                                AmbientLight ambientLight = sceneSystem.CachedAmbientLights[k];
                                if (ambientLight.EnabledWorld) ambientLightColor += ambientLight.Color.ToVector3();
                            }

                            effect.AmbientLightColor = ambientLightColor;

                            // directional lights emulated as point lights
                            for (
                                int k = 0;
                                k < SceneConfiguration.DirectionalLightCount &&
                                k < intSkinnedModelEnabledsLights;
                                ++k)
                            {
                                XNAnimation.Effects.PointLight effectPointLight = effect.PointLights[k];

                                if (k >= sceneSystem.CachedDirectionalLights.Count)
                                {
                                    effectPointLight.Color = Vector3.Zero;
                                    effectPointLight.Position = Vector3.Zero;
                                }
                                else
                                {
                                    DirectionalLight directionalLight = sceneSystem.CachedDirectionalLights[k];
                                    Vector3 emulatedPosition = -directionalLight.Direction * emulatedPointLightDistance;
                                    Vector3 emulatedColor = (directionalLight.DiffuseColor.ToVector3() + directionalLight.SpecularColor.ToVector3()) * 0.5f;
                                    effectPointLight.Color = emulatedColor;
                                    effectPointLight.Position = emulatedPosition;
                                }
                            }

                            // point lights
                            sceneSystem.CachedPointLights.DistanceSort(BoundingBoxWorld.GetCenter(), SpatialSortOrder.NearToFar);

                            for (
                                int k = 0;
                                k < SceneConfiguration.PointLightCount &&
                                k + sceneSystem.CachedDirectionalLights.Count < intSkinnedModelEnabledsLights;
                                ++k)
                            {
                                XNAnimation.Effects.PointLight effectPointLight =
                                    effect.PointLights[k + sceneSystem.CachedDirectionalLights.Count];

                                if (k >= sceneSystem.CachedPointLights.Count)
                                {
                                    effectPointLight.Color = Vector3.Zero;
                                    effectPointLight.Position = Vector3.Zero;
                                }
                                else
                                {
                                    Ox.Scene.LightNamespace.PointLight pointLight = sceneSystem.CachedPointLights[k];
                                    Vector3 emulatedColor = (pointLight.DiffuseColor.ToVector3() + pointLight.SpecularColor.ToVector3()) * 0.5f;
                                    effectPointLight.Color = emulatedColor;
                                    effectPointLight.Position = pointLight.PositionWorld;
                                }
                            }
                        }
                    }

                    modelMesh.Draw();
                }
            }
            EndRenderState(device);
        }

        private void BeginRenderState(GraphicsDevice device)
        {
            device.BeginFaceMode(FaceMode);
        }

        private void EndRenderState(GraphicsDevice device)
        {
            device.EndFaceMode();
        }
        
        private const EnabledLights skinnedModelEnabledLights = EnabledLights.Eight;
        private const float emulatedPointLightDistance = 100000;
        private const float emulatedPointLightRange = emulatedPointLightDistance * 2;
        private const int intSkinnedModelEnabledsLights = 8;

        private Color emissiveColor;
        private bool normalMapEnabled = AnimatedModelDefaults.NormalMapEnabled;
    }
}
