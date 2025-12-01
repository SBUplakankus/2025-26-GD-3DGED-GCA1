using System.Numerics;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Events;
using GDEngine.Core.Rendering.Base;
using GDEngine.Core.Services;
using GDGame.Scripts.Audio;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player.
    /// Uses <see cref="PlayerCamera"/> and <see cref="PlayerMovement"/>.
    /// </summary>
    public class PlayerController : Component
    {
        #region Fields
        private GameObject _playerGO;
        private PlayerMovement _playerMovement;
        private PlayerCamera _playerCamera;
        private PlayerStats _playerStats;
        private PlayerEventChannel _playerEventChannel;
        private Vector3 _startPos = new (0, 0, 0);
        private Vector3 _startRot = new (0, 0, 0);
        #endregion

        #region Constructors
        public PlayerController(float aspectRatio, AudioController audio) 
        {
            _playerGO = new GameObject(AppData.PLAYER_NAME);

            _playerCamera = new PlayerCamera(_playerGO, aspectRatio);
            _playerMovement = new PlayerMovement(_playerGO);

            _playerGO.AddComponent(this);
            _playerGO.AddComponent(audio);

            _playerGO.Transform.TranslateTo(_startPos);
            _playerGO.Transform.RotateEulerBy(_startRot);

            _playerStats = new PlayerStats();
            _playerStats.Initialise();

            InitPlayerEvents();

            SceneController.AddToCurrentScene(_playerGO);

        }
        #endregion

        #region Accessors
        public GameObject PlayerGO => _playerGO;
        public Camera PlayerCam => _playerCamera.Cam;
        public PlayerStats Stats => _playerStats;
        #endregion

        #region Events
        
        private void InitPlayerEvents()
        {
            _playerEventChannel = EventChannelManager.Instance.PlayerEvents;
            _playerEventChannel.OnOrbCollected.Subscribe(_playerStats.HandleOrbCollection);
            _playerEventChannel.OnPlayerDamaged.Subscribe(_playerStats.TakeDamage);
            //_playerEventChannel.OnPlayerCollision.Subscribe(_playerMovement.HandlePlayerCollision);
            EngineContext.Instance.Events.Subscribe<CollisionEvent>(_playerMovement.HandlePlayerCollision);
            EventChannelManager.Instance.InputEvents.OnMovementInput.Subscribe(_playerMovement.HandleMovement);
        }
        #endregion

        #region Methods
        #endregion
    }
}
