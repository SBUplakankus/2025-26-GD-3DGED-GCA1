using System.Diagnostics;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Events;
using GDEngine.Core.Rendering.Base;
using GDEngine.Core.Timing;
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
        private float _moveSpeed = 200f;
        private RigidBody _rb;
        private SphereCollider _collider;
        private GameObject _playerParent;
        private LayerMask _playerLayerMask = LayerMask.All;
        #endregion

        #region Accessors
        public RigidBody RB => _rb;
        #endregion

        #region Constructors
        public PlayerMovement(GameObject parent)
        {
            _playerParent = parent;

            InitRB();

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
            _rb.Mass = 0.01f;
            _rb.AngularDamping = 1;
            _rb.LinearDamping = 1;
            // _playerParent.AddComponent(_rb);
        }

        public void HandlePlayerCollision(CollisionEvent collision)
        {
            // Early Exit if the Collsiion doesnt match the player layer mask
            // Or if it doesn't involve the player
            var a = collision.BodyA;
            var b = collision.BodyB;

            // Try keep A as player when calling events

            if (!collision.Matches(_playerLayerMask)) return;
            if (a != _rb && b != _rb) return;

            var colName = b.GameObject.Name;

            switch (colName)
            {
                case "Game_Over":
                    Debug.WriteLine("Game Over");
                    break;
                case "Game_Won":
                    Debug.WriteLine("Game Won");
                    break;
                default:
                    Debug.WriteLine("Collison not Set Up");
                    break;
            }
        }
        private void Move(Vector3 direction, float speed)
        {
            if (_rb.Transform == null)
                return;

            direction.Normalize();

            Vector3 delta = direction * (speed * Time.DeltaTimeSecs);
            // Debug.WriteLine($"{delta.ToString()}");

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

            //Debug.WriteLine($"{moveDir.ToString()}");

            Move(moveDir, _moveSpeed);
        }
        #endregion
    }
}
