using GDEngine.Core.Components;
using GDEngine.Core.Input.Data;
using GDEngine.Core.Input.Devices;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDGame.Scripts.Systems
{
    public class InputManager : Component
    {
        #region Fields
        private InputEventChannel _inputEventChannel;
        private InputSystem _inputSystem;
        private KeyboardState _newKBState, _oldKBState;
        private readonly float _mouseSensitivity = 0.12f;
        private readonly int _debounceMs = 60;
        private readonly bool _enableKeyRepeat = true;
        private readonly int _keyRepeatMs = 300;
        #endregion

        #region Input Keys
        private readonly Keys _pauseKey = Keys.Escape;
        private readonly Keys _fullscreenKey = Keys.F11;
        private readonly Keys _exitKey = Keys.E;
        private readonly Keys _forwardKey = Keys.W;
        private readonly Keys _backwardKey = Keys.S;
        private readonly Keys _leftKey = Keys.A;
        private readonly Keys _rightKey = Keys.D;
        #endregion

        #region Constructors
        public InputManager() 
        {
            var bindings = InputBindings.Default;

            // Set Inititial Parameters
            bindings.MouseSensitivity = _mouseSensitivity;  
            bindings.DebounceMs = _debounceMs;          
            bindings.EnableKeyRepeat = _enableKeyRepeat;   
            bindings.KeyRepeatMs = _keyRepeatMs;       

            _inputSystem = new InputSystem();

            // Register Devices
            _inputSystem.Add(new GDKeyboardInput(bindings));
            _inputSystem.Add(new GDMouseInput(bindings));
            _inputSystem.Add(new GDGamepadInput(PlayerIndex.One, AppData.GAMEPAD_P1_NAME));

            _inputEventChannel = EventChannelManager.Instance.InputEvents;
        }
        #endregion

        #region Accessors
        public InputSystem Input => _inputSystem;
        #endregion

        #region Input Methods
        private void CheckForPause()
        {
            bool isPressed = _newKBState.IsKeyDown(_pauseKey) && !_oldKBState.IsKeyDown(_pauseKey);
            if (!isPressed) return;

            _inputEventChannel.PauseToggle.Raise();    
        }

        private void CheckForFullscreen()
        {
            bool isPressed = _newKBState.IsKeyDown(_fullscreenKey) && !_oldKBState.IsKeyDown(_fullscreenKey);
            if (!isPressed) return;

            _inputEventChannel.FullscreenToggle.Raise();
        }

        private void CheckForExit()
        {
            bool isPressed = _newKBState.IsKeyDown(_exitKey) && !_oldKBState.IsKeyDown(_exitKey);
            if (!isPressed) return;

            _inputEventChannel.ApplicationExit.Raise();
        }

        private void CheckForMovement()
        {
            if (_newKBState.IsKeyDown(_forwardKey))
                _inputEventChannel.MovementInput.Raise(AppData.FORWARD_MOVE_NUM);

            if (_newKBState.IsKeyDown(_backwardKey))
                _inputEventChannel.MovementInput.Raise(AppData.BACKWARD_MOVE_NUM);

            if (_newKBState.IsKeyDown(_leftKey))
                _inputEventChannel.MovementInput.Raise(AppData.LEFT_MOVE_NUM);

            if (_newKBState.IsKeyDown(_rightKey))
                _inputEventChannel.MovementInput.Raise(AppData.RIGHT_MOVE_NUM);
        }
        
        private void CheckForInputs()
        {
            CheckForPause();
            CheckForFullscreen();
            CheckForExit();
            CheckForMovement();
        }
        #endregion

        #region Engine Methods
        protected override void Update(float deltaTime)
        {
            _newKBState = Keyboard.GetState();
            CheckForInputs();
            _oldKBState = _newKBState;

            base.Update(deltaTime);
        }
        #endregion
    }
}
