using JigLibX.Collision;

namespace Ox.Scripts
{
    /// <summary>
    /// Represents an object that can collide with other objects.
    /// </summary>
    public interface ICollidable
    {
        bool OwnsCollisionSkin(CollisionSkin skin);
    }
}
