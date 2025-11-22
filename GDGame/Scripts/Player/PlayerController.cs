using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;
        private Vector3 _startPos = new (0, 0, 0);
        private Vector3 _startRot = new (0, 0, 0);
        #endregion

        #region Constructors
        public PlayerController(float aspectRatio) 
        {
            _playerGO = new GameObject(AppData.PLAYER_NAME);

            _playerMovement = new PlayerMovement();

            _playerCamera = new PlayerCamera(aspectRatio);

            _playerGO.AddComponent(this);
            _playerGO.Transform.TranslateTo(_startPos);
            _playerGO.Transform.RotateEulerBy(_startRot);

        }
        #endregion

        #region Accessors
        public GameObject PlayerGO => _playerGO;
        public GameObject PlayerCamGO => _playerCamera.CameraGO;
        public Camera PlayerCam => _playerCamera.Cam;
        #endregion

        #region Methods
      
        #endregion
    }
}
