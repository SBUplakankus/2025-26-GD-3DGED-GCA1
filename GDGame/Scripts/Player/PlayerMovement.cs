using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;

namespace GDGame.Scripts.Player
{
    public class PlayerMovement
    {
        #region Fields
        private float _moveSpeed = 1f;
        private RigidBody _rb;
        #endregion

        #region Constructors
        public PlayerMovement(GameObject parent)
        {
            _rb = new RigidBody();
            parent.AddComponent<KeyboardWASDController>();
        }
        #endregion
    }
}
