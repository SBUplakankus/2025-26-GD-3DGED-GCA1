using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering.UI;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    public class PauseMenu
    {
        #region Fields
        private GameObject _pauseGO;
        private UIMenuPanel _pausePanel;
        private bool _isVisible = false;
        private ContentDictionary<Texture2D> _textures;
        private SpriteFont _font;
        #endregion

        public PauseMenu(ContentDictionary<Texture2D> textures, SpriteFont font)
        {
            _textures = textures;
            _font = font;
            
            Initialise();
        }

        public void Initialise()
        {
            _pauseGO = new GameObject("Pause");
            SceneController.AddToCurrentScene(_pauseGO);

            _pausePanel = new UIMenuPanel();
            _pauseGO.AddComponent(_pausePanel);
            _pausePanel.AddButton("Potato", _textures.Get("button"), _font, HandleButtonClick);
            _pausePanel.PanelPosition = new Vector2(500, 500);

            _isVisible = false;
            _pausePanel.IsVisible = _isVisible;

            EventChannelManager.Instance.InputEvents.OnPauseToggle.Subscribe(HandlePauseToggle);
        }

        private void HandlePauseToggle()
        {
            _isVisible = !_isVisible;
            _pausePanel.IsVisible = _isVisible;
        }

        private void HandleButtonClick()
        {
            Debug.WriteLine("Click");
        }
    }
}
