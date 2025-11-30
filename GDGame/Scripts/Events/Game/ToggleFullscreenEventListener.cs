using System;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Events;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Events.Game
{
    public class ToggleFullscreenEvent() { }
    public class ToggleFullscreenEventListener : Component
    {
        private Scene? _scene;
        private EventBus? _events;
        private GraphicsDeviceManager? _graphics;

        protected override void Start()
        {
            if (GameObject == null) throw new NullReferenceException(nameof(GameObject));

            _scene = GameObject.Scene;

            if (_scene == null) throw new NullReferenceException(nameof(_scene));

            _events = _scene.Context.Events;

            _events.On<ToggleFullscreenEvent>()
                .Do(HandleToggleFullscreen);
        }

        public void InitGraphics(GraphicsDeviceManager g) => _graphics = g;

        private void HandleToggleFullscreen(ToggleFullscreenEvent @event)
        {
            _graphics.ToggleFullScreen();
        }
    }
}
