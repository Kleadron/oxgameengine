using Microsoft.Xna.Framework.Graphics;

namespace Ox.Scene.FogNamespace
{
    /// <summary>
    /// The fog of a scene.
    /// </summary>
    public class Fog
    {
        /// <summary>
        /// The color of the fog.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// The beginning of the fog.
        /// </summary>
        public float Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// The end of the fog.
        /// </summary>
        public float End
        {
            get { return end; }
            set { end = value; }
        }

        /// <summary>
        /// Is fogging enabled?
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        
        private Color color = Color.Gray;
        private float start = 768;
        private float end = 1024;
        private bool enabled = true;
    }
}
