using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.Systems
{
    public class UserInterfaceController : Component
    {
        #region Fields
        private ContentDictionary<SpriteFont> _fonts;
        private ContentDictionary<Texture2D> _interfaceTextures;
        #endregion

        #region Constructors
        public UserInterfaceController(ContentDictionary<SpriteFont> fonts, ContentDictionary<Texture2D> textures)
        {
            _fonts = fonts;
            _interfaceTextures = textures;
        }
        #endregion

        #region Methods

        #endregion
    }
}
