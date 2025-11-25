using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDGame.Demos.Controllers;

namespace GDGame.Scripts.Traps
{
    public class MovingTrap : TrapBase
    {
        #region Fields
        private float _moveSpeed = 5f;
        private Transform[] _movePoints;
        private Transform _currentTarget;
        #endregion

        #region Constructors
        public MovingTrap(int id, float moveSpeed, Transform[] points) : base(id)
        {
            _moveSpeed = moveSpeed;
            _movePoints = points;
            _currentTarget = points[0];
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
