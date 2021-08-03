using Microsoft.Xna.Framework;

namespace Ox.Gui.Input
{
    /// <summary>
    /// When and how fast certain input is repeated when an input control is held.
    /// </summary>
    public class RepeatRate
    {
        public float FirstDelay
        {
            get { return firstDelay; }
            set { firstDelay = MathHelper.Clamp(value, 0, float.MaxValue); }
        }

        public float Delay
        {
            get { return delay; }
            set { delay = MathHelper.Clamp(value, 0, float.MaxValue); }
        }

        private float firstDelay = 0.5f;
        private float delay = 0.1f;
    }
}
