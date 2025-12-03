using System.Numerics;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Utilities;
using GDGame.Scripts.Events.Channels;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// Controls the cursor reticle in the centre of the screen.
    /// Created in <see cref="UserInterfaceController"/>.
    /// </summary>
    public class CursorController
    {
        #region Fields
        private GameObject _reticleGO;
        private UIReticle _reticleRenderer;
        private Vector2 _reticleScale = new (0.1f, 0.1f);
        private bool _reticleVisible = false;
        #endregion

        #region Constructors
        public CursorController (Texture2D reticleTexture)
        {
            _reticleGO = new GameObject(AppData.RETICLE_NAME);

            _reticleRenderer = new UIReticle(reticleTexture);
            _reticleRenderer.Origin = reticleTexture.GetCenter();
            _reticleRenderer.SourceRectangle = null;
            _reticleRenderer.Scale = _reticleScale;
            _reticleRenderer.LayerDepth = UILayer.HUD;

            _reticleGO.AddComponent(_reticleRenderer);
        }
        #endregion

        #region Accessors
        public GameObject Reticle => _reticleGO;
        #endregion
    }
}
