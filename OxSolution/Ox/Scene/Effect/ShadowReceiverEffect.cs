using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.EffectNamespace;
using Ox.Engine.Utility;
using Ox.Scene.Component;
using Ox.Scene.LightNamespace;
using Ox.Scene.Shadow;
using Ox.Scene.SurfaceNamespace;

namespace Ox.Scene.EffectNamespace
{
    /// <summary>
    /// Represents a shader effect that draws lit and shadowed geometry.
    /// </summary>
    public class ShadowReceiverEffect : LightReceiverEffect
    {
        /// <summary>
        /// Initializes a new instance of ShadowReceiverEffect by cloning an existing effect.
        /// </summary>
        /// <param name="device">The graphics device that will create the effect.</param>
        /// <param name="cloneSource">The effect to clone.</param>
        public ShadowReceiverEffect(GraphicsDevice device, Effect cloneSource)
            : base(device, cloneSource)
        {
            directionalShadowDepthBiasParam = Parameters["xDirectionalShadowDepthBias"];
            directionalShadowEnabledsParam = Parameters["xDirectionalShadowEnableds"];
            directionalShadowPositionsParam = Parameters["xDirectionalShadowPositions"];
            directionalShadowWorldViewProjectionsParam = Parameters["xDirectionalShadowWorldViewProjections"];
            directionalShadow0Param = Parameters["xDirectionalShadow0"];
        }

        /// <summary>
        /// The bias applied during a depth test to determine is a pixel is in a shadow.
        /// </summary>
        public float DirectionalShadowDepthBias
        {
            get { return _directionalShadowDepthBias; }
            set
            {
                if (_directionalShadowDepthBias == value) return; // OPTIMIZATION
                _directionalShadowDepthBias = value;
                directionalShadowDepthBiasParam.TrySetValue(value);
            }
        }

        /// <summary>
        /// Set a bool array of size SceneConfiguration.DirectionalShadowCount that tells if each
        /// directional shadow is enabled.
        /// </summary>
        public void SetDirectionalShadowEnableds(bool[] enableds)
        {
            if (_directionalShadowEnableds.EqualsValue(enableds)) return; // OPTIMIZATION
            _directionalShadowEnableds.CopyFrom(enableds, enableds.Length);
            directionalShadowEnabledsParam.TrySetValue(_directionalShadowEnableds);
        }

        /// <summary>
        /// Set a Vector3 array of size SceneConfiguration.DirectionalShadowCount that tells the
        /// world position of each directional shadow.
        /// </summary>
        public void SetDirectionalShadowPositions(Vector3[] positions)
        {
            if (_directionalShadowPositions.EqualsValue(positions)) return; // OPTIMIZATION
            _directionalShadowPositions.CopyFrom(positions, positions.Length);
            directionalShadowPositionsParam.TrySetValue(_directionalShadowPositions);
        }

        /// <summary>
        /// Set a Matrix array of size SceneConfiguration.DirectionalShadowCount that tells the
        /// world * view * projection of each directional shadow.
        /// </summary>
        public void SetDirectionalShadowWorldViewProjections(Matrix[] worldViewProjections)
        {
            if (_directionalShadowWorldViewProjections.EqualsValue(worldViewProjections)) return; // OPTIMIZATION
            _directionalShadowWorldViewProjections.CopyFrom(worldViewProjections, worldViewProjections.Length);
            directionalShadowWorldViewProjectionsParam.TrySetValue(_directionalShadowWorldViewProjections);
        }

        /// <summary>
        /// Set the shadow map texture of directional shadow 0.
        /// </summary>
        public void SetDirectionalShadow0(Texture2D shadow0)
        {
            if (_directionalShadow0 == shadow0) return; // OPTIMIZATION
            _directionalShadow0 = shadow0;
            directionalShadow0Param.TrySetValue(shadow0);
        }

        /// <summary>
        /// Populate the shadowing parameters.
        /// </summary>
        /// <param name="surface">The shadow-casting surface.</param>
        /// <param name="directionalLights">The directional lights affecting the item.</param>
        public void PopulateShadowing(BaseSurface surface, IList<DirectionalLight> directionalLights)
        {
            lock (staticLock)
            {
                OxHelper.ArgumentNullCheck(surface, directionalLights);

                // directional shadowing
                for (int i = 0; i < SceneConfiguration.DirectionalShadowCount; ++i)
                {
                    if (i >= directionalLights.Count) directionalShadowEnableds[i] = false;
                    else
                    {
                        DirectionalLight light = directionalLights[i];
                        IDirectionalShadow shadow = light.Shadow;
                        Texture2D shadowMap = shadow.VolatileShadowMap;
                        Camera shadowCamera = shadow.Camera;
                        Matrix shadowViewProjection;
                        shadowCamera.GetViewProjection(out shadowViewProjection);
                        Matrix shadowWorldViewProjection;
                        Matrix surfaceWorld;
                        surface.GetTransformWorldScaled(out surfaceWorld);
                        Matrix.Multiply(ref surfaceWorld, ref shadowViewProjection, out shadowWorldViewProjection);

                        directionalShadowEnableds[i] = light.EnabledWorld && shadow.Enabled && shadowMap != null;
                        directionalShadowPositions[i] = shadowCamera.Position;
                        directionalShadowWorldViewProjections[i] = shadowWorldViewProjection;

                        if (shadowMap != null)
                        {
                            // only directional light 0 can have a shadow due to hardware limitations
                            if (i == 0) SetDirectionalShadow0(shadow.VolatileShadowMap);
                        }
                    }
                }

                DirectionalShadowDepthBias = SceneConfiguration.DirectionalShadowDepthBias;
                SetDirectionalShadowEnableds(directionalShadowEnableds);
                SetDirectionalShadowPositions(directionalShadowPositions);
                SetDirectionalShadowWorldViewProjections(directionalShadowWorldViewProjections);
            }
        }

        private static readonly object staticLock = new object();
        private static bool[] directionalShadowEnableds = new bool[SceneConfiguration.DirectionalShadowCount];
        private static Vector3[] directionalShadowPositions = new Vector3[SceneConfiguration.DirectionalShadowCount];
        private static Matrix[] directionalShadowWorldViewProjections = new Matrix[SceneConfiguration.DirectionalShadowCount];

        private readonly EffectParameter directionalShadowDepthBiasParam;
        private readonly EffectParameter directionalShadowEnabledsParam;
        private readonly EffectParameter directionalShadowPositionsParam;
        private readonly EffectParameter directionalShadowWorldViewProjectionsParam;
        private readonly EffectParameter directionalShadow0Param;
        private float _directionalShadowDepthBias;
        private int[] _directionalShadowEnableds = new int[SceneConfiguration.DirectionalShadowCount];
        private Vector3[] _directionalShadowPositions = new Vector3[SceneConfiguration.DirectionalShadowCount];
        private Matrix[] _directionalShadowWorldViewProjections = new Matrix[SceneConfiguration.DirectionalShadowCount];
        /// <summary>May be null.</summary>
        private Texture2D _directionalShadow0;
    }
}
