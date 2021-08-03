using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ox.Engine.CameraNamespace;

namespace Ox.Scene.Shadow
{
    /// <summary>
    /// Represents a directional shadow.
    /// </summary>
	public interface IDirectionalShadow : IDisposable
	{
	    /// <summary>
	    /// The shadow map that results from drawing a scene's shadows.
        /// May be null.
	    /// </summary>
        Texture2D VolatileShadowMap { get; }
        /// <summary>
        /// The camera view from which the shadow map is drawn.
        /// </summary>
        Camera Camera { get; }
        /// <summary>
        /// Is the shadow enabled?
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Draw the shadow map for a scene.
        /// </summary>
        void Draw(GameTime gameTime);
    }
}
