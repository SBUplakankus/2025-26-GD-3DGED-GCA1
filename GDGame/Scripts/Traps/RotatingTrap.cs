using Microsoft.Xna.Framework;
using System;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Moving Obstacle Trap which inherits from <see cref="TrapBase"/>.
    /// </summary>
    public class RotatingTrap : TrapBase
    {
        #region Fields
        private float _rotSpeed = 5f;
        #endregion

        #region Constructors
        public RotatingTrap(int id, float rotSpeed) : base(id)
        {
            _rotSpeed = rotSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            if (Quaternion.Dot(TrapGO.Transform.Rotation, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi)) > 0.9f)
            {
                flip();
            }
        }

        public override void InitTrap()
        {
            
        }

        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void flip()
        {
            _rotSpeed = -_rotSpeed;
        }
    }
}
