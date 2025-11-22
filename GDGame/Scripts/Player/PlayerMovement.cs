using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;

namespace GDGame.Scripts.Player
{
    public class PlayerMovement
    {
        #region Fields
        private float _moveSpeed = 1f;
        private RigidBody _rb;
        #endregion

        #region Constructors
        public PlayerMovement()
        {
            _rb = new RigidBody();
        }
        #endregion
    }
}
