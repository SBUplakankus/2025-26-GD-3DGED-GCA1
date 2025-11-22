using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;

namespace GDGame.Scripts.Player
{
    public class PlayerController : Component
    {
        #region Fields
        private GameObject _playerGO;
        private float _playerSpeed = 1f;
        #endregion

        #region Constructors
        public PlayerController() 
        {
            _playerGO = new GameObject(AppData.PLAYER_NAME);
        }
        #endregion
    }
}
