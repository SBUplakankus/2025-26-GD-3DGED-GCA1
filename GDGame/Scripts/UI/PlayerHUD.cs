using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Entities;
using GDEngine.Core.Rendering.UI;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDGame.Scripts.UI
{
    /// <summary>
    /// Controls the Player HUD which displays stats and messages on the screen.
    /// Created in <see cref="UserInterfaceController"/>.
    /// </summary>
    public class PlayerHUD
    {
        #region Fields
        private PlayerStats _playerStats;
        private SpriteFont _hudFont;
        private Color _hudTextColour = Color.White;
        private readonly Dictionary<string, Vector2> _hudPositions = new()
        {
            ["top_left"] = new Vector2(200, 100),
            ["vert_increment"] = new Vector2(0, 50),
            ["hor_increment"] = new Vector2(285, 0),
            ["health"] = new Vector2(200, 230),
            ["orbs"] = new Vector2(200,260),
            ["objective"] = new Vector2(1500, 200),
            ["message"] = new Vector2(800,800)
        };
        #endregion

        #region Constructors
        public PlayerHUD(SpriteFont hudFont, PlayerStats stats)
        {
            _hudFont = hudFont;
            _playerStats = stats;
        }
        #endregion

        #region Methods
        private void CreateText(string key, Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: {key}");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => LocalisationController.Instance.Get(key),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        private void CreateOrbStat(Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: Orbs");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => _playerStats.OrbsCollected.ToString(),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        private void CreateHealthStat(Vector2 pos)
        {
            var textGO = new GameObject($"Text Object: Health");
            var uiText = new UIText
            {
                Color = _hudTextColour,
                Font = _hudFont,
                LayerDepth = UILayer.HUD,
                TextProvider = () => _playerStats.CurrentHealth.ToString(),
                PositionProvider = () => pos
            };

            textGO.AddComponent(uiText);
            SceneController.AddToCurrentScene(textGO);
        }

        private Vector2 GetPos(string key)
        {
            if (_hudPositions.TryGetValue(key, out var result))
                return result;
            else
                throw new Exception($"{key}: not found in HUD Position Dictionary");
        }

        private void InitHUDText()
        {
            var startPos = GetPos("top_left");
            var vertIncrement = GetPos("vert_increment");
            var horIncrement = GetPos("hor_increment");

            CreateText(AppData.LANG_TIME_KEY, startPos);
            CreateText(AppData.LANG_HEALTH_KEY, startPos += vertIncrement);
            CreateHealthStat(startPos + horIncrement);
            CreateText(AppData.LANG_ORB_KEY, startPos += vertIncrement);
            CreateOrbStat(startPos + horIncrement);
        }

        public void Initialise()
        {
            InitHUDText();
        }
        #endregion
    }
}
