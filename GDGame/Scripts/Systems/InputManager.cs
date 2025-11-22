using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Entities;
using GDEngine.Core.Input.Data;
using GDEngine.Core.Input.Devices;
using GDEngine.Core.Systems;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Systems
{
    public class InputManager
    {
        #region Fields
        private InputSystem _inputSystem;
        private float _mouseSensitivity = 0.12f;
        private int _debounceMs = 60;
        private bool _enableKeyRepeat = true;
        private int _keyRepeatMs = 300;
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
        }
        #endregion

        #region Accessors
        public InputSystem Input => _inputSystem;
        #endregion
    }
}
