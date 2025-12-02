using GDEngine.Core.Components;
using GDGame.Scripts.Systems;
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
        private float _startAngle = -90f;
        private float _endAngle = 90f;
        private float _currentAngle = 0f;
        private bool _rotatingClockwise = true;
        #endregion

        #region Constructors
        public RotatingTrap(int id, float rotSpeed) : base(id)
        {
            _trapGO = ModelGenerator.Instance.GenerateCube(new Vector3(0, 10, -10), Vector3.Zero, new Vector3(5, 5, 5), "ground_grass", AppData.TRAP_NAME + id);
            _trapGO.AddComponent<BoxCollider>();
            SceneController.AddToCurrentScene(_trapGO);
            _rotSpeed = rotSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            TrapGO.Transform.RotateBy(Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.ToRadians(_rotSpeed)));
            if (_rotatingClockwise)
            {
                _currentAngle++;
            }
            else
            {
                _currentAngle--;
            }

            if (_currentAngle >= _endAngle)
            {
                _rotatingClockwise = false;
                flip();
            }
            if (_currentAngle <= _startAngle)
            {
                _rotatingClockwise = true;
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
