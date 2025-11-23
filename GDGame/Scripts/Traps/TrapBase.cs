    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GDEngine.Core.Entities;

    namespace GDGame.Scripts.Traps
    {
        public abstract class TrapBase
        {
            #region Fields
            private GameObject _trapGO;
            private int _trapID;
            #endregion

            #region Constructors
            public TrapBase(int id)
            {
                _trapID = id;
                _trapGO = new GameObject(AppData.TRAP_NAME + _trapID);
            }
            #endregion

            #region Accessors
            public GameObject TrapGO => _trapGO;
            #endregion

            #region Methods
            public abstract void InitTrap();
            public abstract void UpdateTrap();
            public abstract void HandlePlayerHit();
            #endregion
        }
    }
