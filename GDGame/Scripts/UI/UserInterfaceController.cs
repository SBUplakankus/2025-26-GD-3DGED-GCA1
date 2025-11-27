using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Enums;
using GDEngine.Core.Systems.Base;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    public class UserInterfaceController : SystemBase
    {
        #region Fields
        private SpriteBatch _spriteBatch;
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        private CursorController _cursorController;
        private List<GameObject> _uiObjects;
        private List<TextDisplay> _textDisplays;
        #endregion

        #region Constructors
        public UserInterfaceController(SpriteBatch batch, 
            ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures) : 
            base(FrameLifecycle.PostRender, order: 10)
        {
            _spriteBatch = batch;
            _fonts = fonts;
            _interfaceTextures = textures;
            _textDisplays = new List<TextDisplay>();
        }
        #endregion

        #region Accessors
        public List<GameObject> UIObjects => _uiObjects;
        #endregion

        #region Methods
        private void InitCursor()
        {
            _cursorController = new CursorController(_interfaceTextures.Get(AppData.RETICLE_NAME));
            _uiObjects = [_cursorController.Reticle];
        }

        private void InitText()
        {
            var text1 = new TextDisplay(
                LocalisationController.Instance.Get("Play"), Color.Black, _fonts.Get("menufont"));

            _textDisplays.Add(text1);
        }

        private void DisplayText()
        {
            foreach (var text in _textDisplays)
                _spriteBatch.DrawString(text.Font, text.Text, text.Position, text.TextColour);
        }

        public void InitUserInterface()
        {
            InitCursor();
            InitText();
        }

        public override void Draw(float deltaTime)
        {
            _spriteBatch.Begin();
            DisplayText();
            _spriteBatch.End();
        }
        #endregion
    }
}
