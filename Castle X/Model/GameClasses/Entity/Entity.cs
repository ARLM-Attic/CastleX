
namespace CastleX.Model.GameClasses.Entity
{

    #region Enums
    /// <summary>
    /// Facing direction along the X axis.
    /// </summary>
    public enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// Facing direction along the Y axis.
    /// </summary>
    public enum VerticalDirection
    {
        Up = -1,
        Down = 1,
    }

    /// <summary>
    /// Type of enemy entity.
    /// </summary>
    public enum EnemyType
    {
        Ghost = 1,
        Monster = 2,
        Flying = 3,
        Swimming
    }
    #endregion

    public abstract class Entity
    {

        // This class will act as the base class for entities

        #region Fields

        // Animations
        internal Animation idleAnimation;
        internal Animation runAnimation;
        internal Animation dieAnimation;
        internal AnimationPlayer sprite;

        internal ScreenManager screenManager;

        #endregion

        #region Properties

        internal bool IsAlive { get; set; }

        #endregion

    }
}
