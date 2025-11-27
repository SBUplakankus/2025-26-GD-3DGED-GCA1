using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    public class UserInterfaceController : Component
    {
        #region Fields
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        private CursorController _cursorController;
        private List<GameObject> _uiObjects;
        #endregion

        #region Constructors
        public UserInterfaceController(ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures)
        {
            _fonts = fonts;
            _interfaceTextures = textures;
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
        public void InitUserInterface()
        {
            InitCursor();
        }
        #endregion
    }
}
