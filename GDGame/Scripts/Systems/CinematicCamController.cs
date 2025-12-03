using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.Events.Channels;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Controls the intro of the game with the cinematic camera movement and audio.
    /// </summary>
    public class CinematicCamController : Component
    {
        #region Fields
        private readonly GameObject _cineCamGO;
        private AudioEventChannel _audioEventChannel;
        private PlayerEventChannel _playerEventChannel;
        private readonly Camera _camera;
        private Vector3 _startPos = new (8,18,8);
        private Vector3 _endPos = new (18, 15, 18);
        private bool _isActive;
        private readonly float _duration = 0.1f;
        private float _counter = 0f;
        private readonly float _cameraFOV = 0.9f;
        private const int FAR_PLANE_LIMIT = 1000;
        #endregion

        #region Constructors
        public CinematicCamController(float aspect)
        {
            _cineCamGO = new GameObject("CineCam");
            _cineCamGO.AddComponent(this);
            _camera = _cineCamGO.AddComponent<Camera>();
            _camera.FarPlane = FAR_PLANE_LIMIT;
            _camera.AspectRatio = aspect;
            _camera.FieldOfView = _cameraFOV;
        }
        #endregion

        #region Methods
        public void Initialise()
        {
            SceneController.AddToCurrentScene(_cineCamGO);
            SceneController.SetActiveCamera(_camera);
            _audioEventChannel = EventChannelManager.Instance.AudioEvents;
            _playerEventChannel = EventChannelManager.Instance.PlayerEvents;
            StartCameraMovement();
        }

        private void StartCameraMovement()
        {
            _cineCamGO.Transform.TranslateTo(_startPos);
            _isActive = true;
            _audioEventChannel.OnSFXRequested.Raise(AppData.GAME_INTRO_KEY);
        }

        private void HandleCameraMovement()
        {
            float t = Math.Clamp(_counter / _duration, 0f, 1f);
            _cineCamGO.Transform.TranslateTo(Vector3.Lerp(_startPos, _endPos, t));
        }

        private void HandleCompletion()
        {
            _isActive = false;
            SceneController.SetActiveCamera(AppData.PLAYER_NAME);
            _playerEventChannel.OnGameStateChange.Raise(GameState.GameActive);
        }

        protected override void Update(float deltaTime)
        {
            if (!_isActive) return;

            if(_counter < _duration)
            {
                _counter += deltaTime;
                HandleCameraMovement();
            }
            else
            {
                HandleCompletion();
            }

            base.Update(deltaTime);
        }
        #endregion
    }
}
