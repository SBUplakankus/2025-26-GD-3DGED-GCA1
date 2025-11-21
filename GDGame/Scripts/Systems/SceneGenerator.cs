using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Rendering;
using GDGame.Demos.Controllers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;

namespace GDGame.Scripts.Systems
{
    public class SceneGenerator
    {
        #region Fields
        private ContentDictionary<Texture2D> _textures;
        private GameObject _skyBoxParent;
        private GraphicsDeviceManager _graphics;
        private Material _matBasicUnlit, _matBasicLit, _matBasicUnlitGround;

        private const int SKY_SCALE = 500;
        private Vector3 _skyScaleVector = new(SKY_SCALE, SKY_SCALE, 1);

        private readonly Dictionary<string, Vector3> _skyPositions =
            new()
            {
                { "back",  new Vector3(0, 0, -SKY_SCALE / 2) },
                { "left",  new Vector3(-SKY_SCALE / 2, 0, 0) },
                { "right", new Vector3(SKY_SCALE / 2, 0, 0) },
                { "front", new Vector3(0, 0, SKY_SCALE / 2) },
                { "top",   new Vector3(0, SKY_SCALE / 2, 0) }
            };

        private readonly Dictionary<string, Vector3> _skyRotations =
            new()
            {
                { "back",  Vector3.Zero },
                { "left",  new Vector3(0, MathHelper.ToRadians(90), 0) },
                { "right", new Vector3(0, MathHelper.ToRadians(90), 0) },
                { "front", new Vector3(0, MathHelper.ToRadians(180), 0) },
                { "top",   new Vector3(MathHelper.ToRadians(90), 0, MathHelper.ToRadians(90)) }
            };
        #endregion

        #region Constructors
        public SceneGenerator(ContentDictionary<Texture2D> tex, GraphicsDeviceManager graphics)
        {
            _textures = tex;
            _graphics = graphics;
            GenerateMaterials();
        }
        #endregion

        #region Public Methods

        public void GenerateScene(Scene currentScene)
        {
            GenerateSkyBox(currentScene);
        }

        #endregion

        #region Private Methods
        private void GenerateSkyBox(Scene currentScene)
        {
            _skyBoxParent = new GameObject("SkyParent");
            var rot = _skyBoxParent.AddComponent<RotationController>();
            rot._rotationAxisNormalized = Vector3.Up;
            rot._rotationSpeedInRadiansPerSecond = MathHelper.ToRadians(2f);
            currentScene.Add(_skyBoxParent);

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_BACK_NAME, _skyRotations["back"],
                _skyPositions["back"], AppData.SKYBOX_BACK_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_LEFT_NAME, _skyRotations["left"],
                _skyPositions["left"], AppData.SKYBOX_LEFT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_RIGHT_NAME, _skyRotations["right"],
                _skyPositions["right"], AppData.SKYBOX_RIGHT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_FRONT_NAME, _skyRotations["front"],
                _skyPositions["front"], AppData.SKYBOX_FRONT_TEXTURE_KEY));

            currentScene.Add(CreateSkySegment(AppData.SKYBOX_SKY_NAME, _skyRotations["top"],
                _skyPositions["top"], AppData.SKYBOX_SKY_TEXTURE_KEY));
        }

        private GameObject CreateSkySegment(string name, Vector3 rotation, Vector3 position, string texture)
        {
            var skySegment = new GameObject(name);
            skySegment.Transform.ScaleTo(_skyScaleVector);
            skySegment.Transform.RotateEulerBy(rotation, true);
            skySegment.Transform.TranslateTo(position);

            var meshFilter = MeshFilterFactory.CreateQuadTexturedLit(_graphics.GraphicsDevice);
            skySegment.AddComponent(meshFilter);

            var meshRenderer = skySegment.AddComponent<MeshRenderer>();
            meshRenderer.Material = _matBasicUnlit;
            meshRenderer.Overrides.MainTexture = _textures.Get("skybox_sky");

            skySegment.Transform.SetParent(_skyBoxParent.Transform);

            return skySegment;
        }

        private void GenerateMaterials()
        {
            #region Unlit Textured BasicEffect 
            var unlitBasicEffect = new BasicEffect(_graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = false
            };

            _matBasicUnlit = new Material(unlitBasicEffect);
            _matBasicUnlit.StateBlock = RenderStates.Opaque3D();      // depth on, cull CCW
            _matBasicUnlit.SamplerState = SamplerState.LinearClamp;   // helps avoid texture seams on sky

            //ground texture where UVs above [0,0]-[1,1]
            _matBasicUnlitGround = new Material(unlitBasicEffect.Clone());
            _matBasicUnlitGround.StateBlock = RenderStates.Opaque3D();      // depth on, cull CCW
            _matBasicUnlitGround.SamplerState = SamplerState.AnisotropicWrap;   // wrap texture based on UV values

            #endregion

            #region Lit Textured BasicEffect 
            var litBasicEffect = new BasicEffect(_graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                LightingEnabled = true,
                PreferPerPixelLighting = true,
                VertexColorEnabled = false
            };
            litBasicEffect.EnableDefaultLighting();
            _matBasicLit = new Material(litBasicEffect);
            _matBasicLit.StateBlock = RenderStates.Opaque3D();
            #endregion
        }

        #endregion
    }
}
