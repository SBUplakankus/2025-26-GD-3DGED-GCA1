using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player physics movement.
    /// Created in <see cref="PlayerController"/>.
    /// </summary>
    public class PlayerMovement
    {
        #region Fields
        private float _moveSpeed = 2f;
        private RigidBody _rb;
        private SphereCollider _collider;
        private GameObject _playerParent;
        #endregion

        #region Constructors
        public PlayerMovement(GameObject parent)
        {
            _playerParent = parent;

            // InitRB();

            parent.AddComponent<KeyboardWASDController>();
        }
        #endregion

        #region Methods
        private void InitRB()
        {
            _collider = new SphereCollider();
            _collider.Diameter = Vector3.One.Length();
            _playerParent.AddComponent(_collider);

            _rb = new RigidBody();
            _rb.BodyType = BodyType.Dynamic;
            _rb.Mass = 0.1f;
            _playerParent.AddComponent(_rb);
        }
        private void Move(Vector3 dir)
        {
            dir.Normalize();
            Vector3 delta = dir * _moveSpeed;
            _playerParent.Transform.TranslateBy(delta, true);
            _rb.AddForce(delta);
        }
        public void HandleMovement(int dir)
        {
            var forward = _playerParent.Transform.Forward;
            var right = _playerParent.Transform.Right;

            forward.Y = 0;
            right.Y = 0;

            var moveDir = Vector3.Zero;

            if (dir == AppData.FORWARD_MOVE_NUM)
                moveDir += forward;

            if(dir == AppData.BACKWARD_MOVE_NUM)
                moveDir -= forward;

            if (dir == AppData.LEFT_MOVE_NUM)
                moveDir -= right;

            if(dir == AppData.RIGHT_MOVE_NUM)
                moveDir += right;

            if(moveDir.LengthSquared() == 0) return;

            Move(moveDir);
        }
        #endregion
    }
}
