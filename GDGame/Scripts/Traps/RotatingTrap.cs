using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Timing;
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
        private float _centreAngle = 0f;
        private float _endAngle = 60f;
        private float _currentAngle = 0f;
        private double _rotDelay = 0;
        private bool _rotatingClockwise = true;
        #endregion

        #region Constructors
        public RotatingTrap(
            int id,
            Vector3 position,
            Vector3 rotation,
            Vector3 scale,
            string textureName,
            string modelName,
            string objectName,
            double rotDelay,
            float rotSpeed) : base(id)
        {
            _rotDelay = rotDelay;
            _rotSpeed = rotSpeed;

            _trapGO = ModelGenerator.Instance.GenerateModel(
                position,
                rotation,
                scale,
                textureName,
                modelName,
                objectName + id);

            _trapGO.AddComponent<BoxCollider>();
            SceneController.AddToCurrentScene(_trapGO);
        }
        #endregion

        #region Methods
        //public override void UpdateTrap()
        //{
        //    _trapGO.Transform.RotateBy(Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.ToRadians(_rotSpeed)));
        //    if (_rotatingClockwise)
        //    {
        //        _currentAngle++;
        //    }
        //    else
        //    {
        //        _currentAngle--;
        //    }

        //    if (_currentAngle >= _endAngle)
        //    {
        //        _rotatingClockwise = false;
        //        flip();
        //    }
        //    if (_currentAngle <= _startAngle)
        //    {
        //        _rotatingClockwise = true;
        //        flip();
        //    }


        //}

        public override void UpdateTrap()
        {
            float baseRad = MathHelper.ToRadians(_rotSpeed);

            //Compute a target angle as a sine wave around 0 with phase offset
            float t = (float)Time.RealtimeSinceStartupSecs;
            float targetAngle = MathF.Sin(t * _rotSpeed + (float)_rotDelay) * _endAngle;

            //Compute delta from current angle
            float degChange = targetAngle - _currentAngle;

            //Apply easing
            float distFromCenter = MathF.Abs(_currentAngle) / _endAngle;
            distFromCenter = Math.Clamp(distFromCenter, 0f, 1f);

            float easing = (1f + MathF.Cos(distFromCenter * MathF.PI)) * 0.5f;
            float easedMul = MathHelper.Lerp(0.2f, 1f, easing);

            degChange *= easedMul;

            //Update current angle and rotation direction
            _rotatingClockwise = degChange >= 0f;
            _currentAngle += degChange;

            if (_currentAngle >= _endAngle)
            {
                _currentAngle = _endAngle;
                flip();
            }
            else if (_currentAngle <= -_endAngle)
            {
                _currentAngle = -_endAngle;
                flip();
            }

            //Apply rotation
            _trapGO.Transform.RotateBy(
                Quaternion.CreateFromAxisAngle(
                    Vector3.Forward,
                    MathHelper.ToRadians(degChange)
                )
            );
        }

        public override void InitTrap()
        {
            _currentAngle = 0f;
            _rotatingClockwise = true;
        }



        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void flip()
        {
            _rotSpeed = -_rotSpeed;
            _rotatingClockwise = !_rotatingClockwise;
        }
    }
}
