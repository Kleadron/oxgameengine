using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.CameraNamespace;
using Ox.Engine.Component;
using Ox.Engine.GeometryNamespace;
using Ox.Scene.Component;

namespace Ox.Scene.SurfaceNamespace
{
    /// <summary>
    /// Represents a style in which to draw a surface.
    /// </summary>
    public enum DrawStyle
    {
        Opaque = 0,
        Transparent,
        Prioritized
    }

    /// <summary>
    /// Represents a combination of drawing properties for a surface.
    /// </summary>
    [Flags]
    public enum DrawProperties
    {
        None = 0,
        Shadowing = 1,
        Reflecting = 1 << 1,
        DependantTransform = 1 << 2
    }

    /// <summary>
    /// A drawable surface that is dependent on its parent component's transformation.
    /// </summary>
    public abstract class BaseSurface : BaseTransformableSubcomponent
    {
        /// <summary>
        /// Initialize a BaseSurface.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="component">The parent component.</param>
        /// <param name="effectFileName">See property EffectFileName.</param>
        public BaseSurface(OxEngine engine, SceneComponent component, string effectFileName)
            : base(engine, component)
        {
            EffectFileName = effectFileName;
        }

        /// <summary>
        /// The drawing properties.
        /// </summary>
        public DrawProperties DrawProperties
        {
            get { return drawProperties; }
            set { drawProperties = value; }
        }

        /// <summary>
        /// The drawing style.
        /// </summary>
        public DrawStyle DrawStyle
        {
            get { return drawStyle; }
            set { drawStyle = value; }
        }

        /// <summary>
        /// How to draw the faces of the geometry.
        /// </summary>
        public FaceMode FaceMode
        {
            get { return faceMode; }
            set { faceMode = value; }
        }

        /// <summary>
        /// The effect used to draw.
        /// </summary>
        public Effect Effect { get { return EffectHook; } }

        /// <summary>
        /// The diffuse color.
        /// </summary>
        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        /// <summary>
        /// The specular color.
        /// </summary>
        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        /// <summary>
        /// The name of the effect file.
        /// </summary>
        public string EffectFileName
        {
            get { return EffectFileNameHook; }
            set
            {
                OxHelper.ArgumentNullCheck(value);
                if (value.IsNotEmpty()) EffectFileNameHook = value;
            }
        }

        /// <summary>
        /// The specular power.
        /// </summary>
        public float SpecularPower
        {
            get { return specularPower; }
            set { specularPower = value; }
        }

        /// <summary>
        /// Is the surface affected by light?
        /// </summary>
        public bool LightingEnabled
        {
            get { return lightingEnabled; }
            set { lightingEnabled = value; }
        }

        /// <summary>
        /// Should the transparent pixels be drawn, thereby affecting the depth map?
        /// </summary>
        public bool DrawTransparentPixels
        {
            get { return drawTransparentPixels; }
            set { drawTransparentPixels = value; }
        }

        /// <summary>
        /// The drawing order for prioritized drawing.
        /// </summary>
        public int DrawPriority
        {
            get { return drawPriority; }
            set { drawPriority = value; }
        }

        /// <summary>
        /// PreDraw the component in a scene. See notes on PreDraw in
        /// <see cref="SceneSystem"/>.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the component is viewed.</param>
        public void PreDraw(GameTime gameTime, Camera camera)
        {
            PreDrawHook(gameTime, camera);
        }

        /// <summary>
        /// Draw the component in a scene using the specified drawing mode.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="camera">The camera from which the component is viewed.</param>
        /// <param name="drawMode">The manner in which to draw the component.</param>
        public void Draw(GameTime gameTime, Camera camera, string drawMode)
        {
            DrawHook(gameTime, camera, drawMode);
        }

        /// <summary>
        /// Does the component have the specified DrawProperties?
        /// </summary>
        /// <param name="properties">The DrawProperties to check for.</param>
        /// <returns>True if the surface has the specified DrawProperties.</returns>
        public bool HasDrawProperties(DrawProperties properties)
        {
            return (DrawProperties & properties) == properties;
        }

        /// <summary>
        /// Handle getting the effect.
        /// </summary>
        protected abstract Effect EffectHook { get; }

        /// <summary>
        /// Handle getting and setting the effect file name.
        /// </summary>
        protected abstract string EffectFileNameHook { get; set; }

        /// <summary>
        /// Handle predrawing the surface.
        /// </summary>
        protected abstract void PreDrawHook(GameTime gameTime, Camera camera);

        /// <summary>
        /// Handle predrawing the surface.
        /// </summary>
        protected abstract void DrawHook(GameTime gameTime, Camera camera, string drawMode);

        private DrawProperties drawProperties = DrawProperties.Reflecting | DrawProperties.Shadowing;
        private DrawStyle drawStyle = DrawStyle.Opaque;
        private FaceMode faceMode = FaceMode.FrontFaces;
        private Color diffuseColor = Color.White;
        private Color specularColor = Color.Gray;
        private string normalTechnique = string.Empty;
        private string directionalShadowTechnique = string.Empty;
        private float specularPower = 8;
        private bool lightingEnabled = true;
        private bool drawTransparentPixels;
        private int drawPriority;
    }
}
