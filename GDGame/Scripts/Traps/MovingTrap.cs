using System;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Moving Obstacle Trap which inherits from <see cref="TrapBase"/>.
    /// </summary>
    public class MovingTrap : TrapBase
    {
        #region Fields
        private float _moveSpeed = 5f;
        #endregion

        #region Constructors
        public MovingTrap(int id, float moveSpeed) : base(id)
        {
            _moveSpeed = moveSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            
        }

        public override void InitTrap()
        {
            
        }

        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
