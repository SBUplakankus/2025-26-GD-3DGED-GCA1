using GDEngine.Core.Components;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;
using System;
using System.Security.AccessControl;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Moving Obstacle Trap which inherits from <see cref="TrapBase"/>.
    /// </summary>
    public class MovingTrap : TrapBase
    {
        #region Fields
        private float _moveSpeed = 5f;
        private Vector3 _startPosition = new Vector3(0, 0, 0);
        #endregion

        #region Constructors
        public MovingTrap(int id, Vector3 position, Vector3 rotation, Vector3 scale, string textureName, string modelName, string objectName, float moveSpeed) : base(id)
        {
            _trapGO = ModelGenerator.Instance.GenerateModel(
                            position,
                            rotation,
                            scale,
                            textureName,
                            modelName,
                            objectName + id);
            _trapGO.AddComponent<BoxCollider>();
            SceneController.AddToCurrentScene(_trapGO);
            _moveSpeed = moveSpeed;
        }
        #endregion

        #region Methods
        public override void UpdateTrap()
        {
            _trapGO.Transform.TranslateBy(new Vector3(0, _moveSpeed, 0));
            if (_trapGO.Transform.Position.Y > _startPosition.Y+5f || _trapGO.Transform.Position.Y < _startPosition.Y -5f)
            {
                flip();
            }
        }

        public override void InitTrap()
        {
            _startPosition = _trapGO.Transform.Position;
        }

        public override void HandlePlayerHit()
        {
            throw new NotImplementedException();
        }
        #endregion

        public void flip()
        {
            _moveSpeed = -_moveSpeed;
        }
    }
}
