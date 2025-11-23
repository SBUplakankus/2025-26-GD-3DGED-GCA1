using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering;
using GDEngine.Core.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    public class CursorController
    {
        #region Fields
        private GameObject _reticleGO;
        private UIReticleRenderer _reticleRenderer;
        private Vector2 _reticleScale = new (0.1f, 0.1f);
        #endregion

        #region Constructors
        public CursorController (Texture2D reticleTexture)
        {
            _reticleGO = new GameObject(AppData.RETICLE_NAME);

            _reticleRenderer = new UIReticleRenderer(reticleTexture);
            _reticleRenderer.Origin = reticleTexture.GetCenter();
            _reticleRenderer.SourceRectangle = null;
            _reticleRenderer.Scale = _reticleScale;
            _reticleRenderer.LayerDepth = UILayer.Cursor;

            _reticleGO.AddComponent(_reticleRenderer);

        }
        #endregion

        #region Accessors
        public GameObject Reticle => _reticleGO;
        #endregion
    }
}
