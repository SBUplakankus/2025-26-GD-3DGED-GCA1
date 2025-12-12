using GDEngine.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace GDGame.Scripts.Traps
{
    /// <summary>
    /// Creates and Updates the traps in the game which are based off of <see cref="TrapBase"/>.
    /// </summary>
    public class TrapManager : Component
    {
        #region Fields
        private List<TrapBase> _trapList;
        #endregion

        #region Constructor
        public TrapManager() 
        {
            _trapList = new List<TrapBase>();
        }
        #endregion

        #region Accessors
        public List<TrapBase> TrapList => _trapList;
        #endregion

        #region Game Methods
        public void AddTrap(int id, Vector3 position, Vector3 rotation, Vector3 scale, string textureName, string modelName, string objectName, float rotSpeed)
        {
            
            RotatingTrap trap = new RotatingTrap(id, position, rotation, scale, textureName, modelName, objectName, rotSpeed);
            _trapList.Add(trap);
            trap.InitTrap();
        }

        public void AddMovingTrap(int id, Vector3 position, Vector3 rotation, Vector3 scale, string textureName, string modelName, string objectName, float moveSpeed)
        {
            MovingTrap trap = new MovingTrap(id, position,rotation,scale,textureName,modelName,objectName, moveSpeed);
            _trapList.Add(trap);
            trap.InitTrap();
        }

        private void InitTraps()
        {
            if (_trapList.Count == 0) return;

            foreach (var trap in _trapList)
            {
                trap.InitTrap();
            }
                
        }
        private void UpdateTraps()
        {
            if(_trapList.Count == 0) return;

            foreach(var trap in _trapList)
                trap.UpdateTrap();
        }
        #endregion

        #region Engine Methods
        protected override void Start()
        {
            //_trapList.Add(new MovingTrap(1, 1f));
            //_trapList.Add(new RotatingTrap(2, 5f));

            InitTraps();
        }
        protected override void Update(float deltaTime)
        {
            UpdateTraps();
        }
        #endregion
    }
}
