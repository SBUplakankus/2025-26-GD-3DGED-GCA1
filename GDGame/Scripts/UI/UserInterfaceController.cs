using System.Windows.Forms;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Enums;
using GDEngine.Core.Managers;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    public class UserInterfaceController : SystemBase
    {
        #region Fields

        // Core
        private SpriteBatch _spriteBatch;
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        private CursorController _cursorController;
        private MenuController _menuController;
        private PlayerHUD _playerHUD;
        private PauseMenu _pauseMenu;
        private Vector2 _screenCentre;
        private Game _game;

        // Event Channels
        private GameEventChannel _gameEvents;

        #endregion

        #region Constructors
        public UserInterfaceController(SpriteBatch batch, 
            ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures, Vector2 centre, Game game) : 
            base(FrameLifecycle.PostRender, order: 10)
        {
            _spriteBatch = batch;
            _fonts = fonts;
            _interfaceTextures = textures;
            _screenCentre = centre;
            _game = game;
            _gameEvents = EventChannelManager.Instance.GameEvents;
        }
        #endregion

        #region Methods
        private void InitCursor()
        {
            _cursorController = new CursorController(_interfaceTextures.Get(AppData.RETICLE_NAME));
            SceneController.AddToCurrentScene(_cursorController.Reticle);
        }

        private void InitHUD(PlayerStats stats)
        {
            _playerHUD = new PlayerHUD(_fonts.Get("gamefont"), stats);
            _playerHUD.Initialise();
        }

        private void InitPauseMenu()
        {
            _pauseMenu = new PauseMenu(_interfaceTextures, _fonts.Get("gamefont"), _screenCentre);
        }

        private void InitMenuController()
        {
            _menuController = new MenuController(_game);
            _game.Components.Add(_menuController);

            Texture2D btnTex = _interfaceTextures.Get("button2");
            Texture2D trackTex = _interfaceTextures.Get("hyphon");
            Texture2D handleTex = _interfaceTextures.Get("toggle");
            Texture2D controlsTx = _interfaceTextures.Get("toggle");
            SpriteFont uiFont = _fonts.Get("gamefont");

            // Wire UIManager to the menu scene
            _menuController.Initialize(SceneController.GetCurrentScene,
                btnTex, trackTex, handleTex, controlsTx, uiFont,
                _interfaceTextures.Get("bg_1"),
                 _interfaceTextures.Get("bg_2"),
                  _interfaceTextures.Get("bg_3"));

            // Subscribe to high-level events
            _menuController.PlayRequested += () =>
            {
                _menuController.HideMenus();
                //fade out menu sound
            };

            _menuController.ExitRequested += () =>
            {
                Application.Exit();
            };

            _menuController.MusicVolumeChanged += v =>
            {
                // Forward to audio manager
                System.Diagnostics.Debug.WriteLine("MusicVolumeChanged");

                //raise event to set sound
                // EngineContext.Instance.Events.Publish(new PlaySfxEvent)
            };

            _menuController.SfxVolumeChanged += v =>
            {
                // Forward to audio manager
                System.Diagnostics.Debug.WriteLine("SfxVolumeChanged");

                //raise event to set sound
            };
        }

        public void Initialise(PlayerStats stats)
        {
            InitCursor();
            InitHUD(stats);
            // InitPauseMenu();
            InitMenuController();
        }

        public override void Draw(float deltaTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.End();
        }
        #endregion
    }
}
