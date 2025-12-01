using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Enums;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Systems.Base;
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
        private SpriteBatch _spriteBatch;
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        private CursorController _cursorController;
        private PlayerHUD _playerHUD;
        private PauseMenu _pauseMenu;
        private Vector2 _screenCentre;
        #endregion

        #region Constructors
        public UserInterfaceController(SpriteBatch batch, 
            ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures, Vector2 centre) : 
            base(FrameLifecycle.PostRender, order: 10)
        {
            _spriteBatch = batch;
            _fonts = fonts;
            _interfaceTextures = textures;
            _screenCentre = centre;
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

        public void Initialise(PlayerStats stats)
        {
            InitCursor();
            InitHUD(stats);
            InitPauseMenu();
        }

        public override void Draw(float deltaTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.End();
        }
        #endregion
    }
}
