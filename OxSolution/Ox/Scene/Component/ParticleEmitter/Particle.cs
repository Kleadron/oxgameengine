using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ox.Scene.Component
{
    /// <summary>
    /// A functionally thin particle that can't draw itself.
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// The position of the particle in the scene.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// The velocity of the particle.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        /// <summary>
        /// The color of the particle.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// The age of the particle from [0..1] where 1 is dead.
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// The scale of the particle.
        /// </summary>
        public float Age
        {
            get { return age; }
            set { age = value; }
        }

        /// <summary>
        /// The visiblity of the particle.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        
        private Vector3 position;
        private Vector3 velocity;
        private Color color = Color.White;
        private float age;
        private float scale = 1;
        private bool visible = true;
    }
}
