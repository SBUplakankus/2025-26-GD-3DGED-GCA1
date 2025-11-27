using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    public class TextDisplay
    {
        #region Fields
        private string _textKey;
        private Color _textColour;
        private SpriteFont _font;
        private Vector2 _position;
        #endregion

        #region Constructors
        public TextDisplay(string textKey, Color textColour, SpriteFont font)
        {
            _textColour = textColour;
            _textKey = textKey;
            _font = font;
            _position = new Vector2(5, 5);
        }
        #endregion

        #region Accessors
        public string Text => _textKey;
        public Color TextColour => _textColour;
        public SpriteFont Font => _font;
        public Vector2 Position => _position;
        #endregion

        #region Methods
        public void InitText()
        {

        }
        #endregion
    }
}
