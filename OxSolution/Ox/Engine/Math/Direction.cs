using System.ComponentModel;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents a four-way direction.
    /// </summary>
    public enum Direction2D
    {
        Up = 0,
        Down,
        Left,
        Right,
        [Browsable(false)]
        Count
    }

    /// <summary>
    /// Represents a six-way direction.
    /// </summary>
    public enum Direction
    {
        Up = 0,
        Down,
        Left,
        Right,
        Forward,
        Backward,
        [Browsable(false)]
        Count
    }
}
