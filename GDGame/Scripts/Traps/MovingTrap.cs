using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDGame.Scripts.Traps
{
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
