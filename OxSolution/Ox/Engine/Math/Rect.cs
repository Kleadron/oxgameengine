using System;
using Microsoft.Xna.Framework;

namespace Ox.Engine.MathNamespace
{
    /// <summary>
    /// Represents justification in two dimensions.
    /// </summary>
    public enum Justification2D
    {
        TopLeft = 0,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight,
    }

    /// <summary>
    /// Ox's representation of a rectangle. It replaces XNA's Rectangle when fractional precision
    /// is needed.
    /// </summary>
    public struct Rect : IEquatable<Rect>
    {
        /// <summary>
        /// Create a Rect.
        /// </summary>
        /// <param name="x">See property X.</param>
        /// <param name="y">See property Y.</param>
        /// <param name="width">See property Width.</param>
        /// <param name="height">See property Height.</param>
        public Rect(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Create a Rect.
        /// </summary>
        /// <param name="position">See property Position.</param>
        /// <param name="scale">See property Scale.</param>
        public Rect(Vector2 position, Vector2 scale)
            : this(position.X, position.Y, scale.X, scale.Y) { }

        /// <summary>
        /// The position of the rectangle.
        /// </summary>
        public Vector2 Position { get { return new Vector2(X, Y); } }

        /// <summary>
        /// The scale of the rectangle.
        /// </summary>
        public Vector2 Scale { get { return new Vector2(Width, Height); } }

        /// <summary>
        /// The position of the top-left corner.
        /// </summary>
        public Vector2 TopLeft { get { return new Vector2(Left, Top); } }

        /// <summary>
        /// The position of the top-right corner.
        /// </summary>
        public Vector2 TopRight { get { return new Vector2(Right, Top); } }

        /// <summary>
        /// The position of the bottom-left corner.
        /// </summary>
        public Vector2 BottomLeft { get { return new Vector2(Left, Bottom); } }

        /// <summary>
        /// The position of the bottom-right corner.
        /// </summary>
        public Vector2 BottomRight { get { return new Vector2(Right, Bottom); } }

        /// <summary>
        /// The position of the center.
        /// </summary>
        public Vector2 Center { get { return new Vector2(HorizontalCenter, VerticalCenter); } }

        /// <summary>
        /// The x-position of the left side.
        /// </summary>
        public float Left { get { return X; } }

        /// <summary>
        /// The x-position of the right side.
        /// </summary>
        public float Right { get { return X + Width; } }

        /// <summary>
        /// The y-position of the top side.
        /// </summary>
        public float Top { get { return Y; } }

        /// <summary>
        /// The y-position of the bottom side.
        /// </summary>
        public float Bottom { get { return Y + Height; } }

        /// <summary>
        /// The x-position of the center.
        /// </summary>
        public float HorizontalCenter { get { return X + Width * 0.5f; } }

        /// <summary>
        /// The y-position of the center.
        /// </summary>
        public float VerticalCenter { get { return Y + Height * 0.5f; } }

        /// <summary>
        /// Is this rectangle equal to another?
        /// </summary>
        public bool Equals(Rect other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Width == other.Width &&
                Height == other.Height;
        }

        /// <summary>
        /// Apply justification to a rect using this object's coordinates as the limits.
        /// </summary>
        /// <param name="justification">The justification to apply.</param>
        /// <param name="rect">The source rect to apply justification to.</param>
        /// <returns>The result of applying justification to the rect.</returns>
        public Rect Justify(Justification2D justification, Rect rect)
        {
            Rect result = new Rect();
            result.Width = rect.Width;
            result.Height = rect.Height;
            switch (justification)
            {
                case Justification2D.TopLeft:
                    result.X = Left;
                    result.Y = Top;
                    break;
                case Justification2D.Top:
                    result.X = HorizontalCenter - rect.Width * 0.5f;
                    result.Y = Top;
                    break;
                case Justification2D.TopRight:
                    result.X = Right - rect.Width;
                    result.Y = Top;
                    break;
                case Justification2D.Left:
                    result.X = Left;
                    result.Y = VerticalCenter - rect.Height * 0.5f;
                    break;
                case Justification2D.Center:
                    result.X = HorizontalCenter - rect.Width * 0.5f;
                    result.Y = VerticalCenter - rect.Height * 0.5f;
                    break;
                case Justification2D.Right:
                    result.X = Right - rect.Width;
                    result.Y = VerticalCenter - rect.Height * 0.5f;
                    break;
                case Justification2D.BottomLeft:
                    result.X = Left;
                    result.Y = Bottom - rect.Height;
                    break;
                case Justification2D.Bottom:
                    result.X = HorizontalCenter - rect.Width * 0.5f;
                    result.Y = Bottom - rect.Height;
                    break;
                case Justification2D.BottomRight:
                    result.X = Right - rect.Width;
                    result.Y = Bottom - rect.Height;
                    break;
                default:
                    result.X = rect.X;
                    result.Y = rect.Y;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Find the overlapping portion of this and another rect.
        /// </summary>
        public Rect Union(Rect rect)
        {
            Rect result;
            result.X = Math.Max(X, rect.X);
            result.Y = Math.Max(Y, rect.Y);
            float resultRight = Math.Min(Right, rect.Right);
            float resultBottom = Math.Min(Bottom, rect.Bottom);
            result.Width = resultRight - result.X;
            result.Height = resultBottom - result.Y;
            return result;
        }

        /// <summary>
        /// Determine if there's an intersection between this and another rect.
        /// </summary>
        public bool Intersects(Rect rect)
        {
            // OPTIMIZATION: made faster for the usual case of non-intersection
            return !(
                Left > rect.Right ||
                Right < rect.Left ||
                Top > rect.Bottom ||
                Bottom < rect.Top);
        }

        /// <summary>
        /// Determine if the point is inside where the right and bottom are considered inside.
        /// </summary>
        public bool ContainsAllInclusive(Vector2 point)
        {
            // OPTIMIZATION: made faster for the usual case of non-containment
            return !(
                point.X < Left ||
                point.X > Right ||
                point.Y < Top ||
                point.Y > Bottom);
        }

        /// <summary>
        /// Determine if the point is inside.
        /// </summary>
        public bool Contains(Vector2 point)
        {
            // OPTIMIZATION: made faster for the usual case of non-containment
            return !(
                point.X < Left ||
                point.X >= Right ||
                point.Y < Top ||
                point.Y >= Bottom);
        }

        /// <summary>
        /// Determine if the incoming rect is inside where this rect's right and bottom are
        /// considered inside.
        /// </summary>
        public bool ContainsAllInclusive(Rect rect)
        {
            // OPTIMIZATION: made faster for the usual case of non-containment
            return !(
                rect.Left < Left ||
                rect.Right > Right ||
                rect.Top < Top ||
                rect.Bottom > Bottom);
        }

        /// <summary>
        /// Determine if the incoming rect is inside this one.
        /// </summary>
        public bool Contains(Rect rect)
        {
            // OPTIMIZATION: made faster for the usual case of non-containment
            return !(
                rect.Left < Left ||
                rect.Right >= Right ||
                rect.Top < Top ||
                rect.Bottom >= Bottom);
        }

        /// <summary>
        /// The position of the rectangle on the x-axis.
        /// </summary>
        public float X;
        /// <summary>
        /// The position of the rectangle on the y-axis.
        /// </summary>
        public float Y;
        /// <summary>
        /// The width of the rectangle.
        /// </summary>
        public float Width;
        /// <summary>
        /// The height of the rectangle.
        /// </summary>
        public float Height;
    }
}
