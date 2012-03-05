
namespace CastleX.Model.GameClasses.Entity
{
    /// <summary>
    /// Facing direction along the X axis.
    /// </summary>
    public enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    public enum VerticalDirection
    {
        Up = -1,
        Down = 1,
    }

    public enum EnemyType
    {
        Ghost = 1,
        Monster = 2,
        Flying = 3
    }

    public abstract class Entity
    {

        // This class will act as the base class for entities

        // Animations
        internal Animation idleAnimation;
        internal Animation runAnimation;
        internal Animation dieAnimation;

        internal AnimationPlayer sprite;

    }
}
