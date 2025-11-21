using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.AccessControl;
using GDEngine.Core;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Debug;
using GDEngine.Core.Entities;
using GDEngine.Core.Events;
using GDEngine.Core.Extensions;
using GDEngine.Core.Factories;
using GDEngine.Core.Input.Data;
using GDEngine.Core.Input.Devices;
using GDEngine.Core.Orchestration;
using GDEngine.Core.Rendering;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Serialization;
using GDEngine.Core.Services;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using GDEngine.Core.Utilities;
using GDGame.Demos.Controllers;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.X3DAudio;
using Color = Microsoft.Xna.Framework.Color;

namespace GDGame
{
    public class Main : Game
    {
        #region Core Fields (Common to all games)     
        private GraphicsDeviceManager _graphics;
        private ContentDictionary<Model> _modelDictionary;
        private ContentDictionary<Effect> _effectsDictionary;
        private Scene _scene;
        private Camera _camera;
        private bool _disposed = false;
        private OrchestrationSystem _orchestrationSystem;
        private Material _matBasicUnlit, _matBasicLit, _matBasicUnlitGround;
        #endregion

        #region Systems
        AudioController _audioController;
        SceneController _sceneController;
        UserInterfaceController _uiController;
        SceneGenerator _sceneGenerator;
        ModelGenerator _modelGenerator;
        #endregion

        #region Game Fields
        private GameObject _cameraGO;
        private KeyboardState _newKBState, _oldKBState;
        #endregion

        #region Core Methods (Common to all games)     
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            #region Core

            // Give the game a name
            Window.Title = AppData.GAME_WINDOW_TITLE;

            // Set resolution and centering (by monitor index)
            InitializeGraphics(ScreenResolution.R_FHD_16_9_1920x1080);

            // Center and hide the mouse!
            InitializeMouse();

            // Shared data across entities
            InitializeContext();


            // Scene to hold game objects
            GenerateMaterials();

            InitializeScene();

            LoadAssetsFromJSON(AppData.ASSET_MANIFEST_PATH);

            // Camera, UI, Menu, Physics, Rendering etc.
            InitializeSystems();

            GenerateBaseScene();

            // All cameras we want in the game are loaded now and one set as active
            InitializeCameras();


            // Setup player
            InitializePlayer();
      
            #region Game
            DemoLoadFromJSON();
            #endregion

            // Setup renderers after all game objects added since ui text may use a gameobject as target
            InitializeUI();

            #endregion

            base.Initialize();
        }

        private void InitializePlayer()
        {
            GameObject player = _modelGenerator.GenerateModel(new Vector3(0, 5, 10),
                new Vector3(0, 0, 0),
                new Vector3(0.1f, 0.1f, 0.1f), "colormap", "ghost", AppData.PLAYER_NAME);

            var simpleDriveController = new SimpleDriveController();
            player.AddComponent(simpleDriveController);
        }

        private void InitializeGraphics(Integer2 resolution)
        {
            System.Windows.Forms.Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.PerMonitorV2);
            ScreenResolution.SetResolution(_graphics, resolution);
            WindowUtility.CenterOnMonitor(this, 1);
        }

        private void InitializeMouse()
        {
            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            _oldKBState = Keyboard.GetState();
        }

        private void InitializeContext()
        {
            EngineContext.Initialize(GraphicsDevice, Content);
        }

        /// <summary>
        /// New asset loading from JSON using AssetEntry and ContentDictionary::LoadFromManifest
        /// </summary>
        /// <param name="relativeFilePathAndName"></param>
        /// <see cref="AssetEntry"/>
        /// <see cref="ContentDictionary{T}"/>
        private void LoadAssetsFromJSON(string relativeFilePathAndName)
        {
            // Make dictionaries to store assets
            var textures = new ContentDictionary<Texture2D>();

            var models = new ContentDictionary<Model>();
            var fonts = new ContentDictionary<SpriteFont>();
            _effectsDictionary = new ContentDictionary<Effect>();
            var sounds = new ContentDictionary<SoundEffect>();


            var manifests = JSONSerializationUtility.LoadData<AssetManifest>(Content, relativeFilePathAndName); // single or array
            if (manifests.Count > 0)
            {
                foreach (var m in manifests)
                {
                    models.LoadFromManifest(m.Models, e => e.Name, e => e.ContentPath, overwrite: true);
                    textures.LoadFromManifest(m.Textures, e => e.Name, e => e.ContentPath, overwrite: true);
                    fonts.LoadFromManifest(m.Fonts, e => e.Name, e => e.ContentPath, overwrite: true);
                    sounds.LoadFromManifest(m.Sounds, e => e.Name, e => e.ContentPath, overwrite: true);
                    _effectsDictionary.LoadFromManifest(m.Effects, e => e.Name, e => e.ContentPath, overwrite: true);
                }
            }

            _audioController = new AudioController(sounds);
            _uiController = new UserInterfaceController(fonts, textures);
            _sceneGenerator = new SceneGenerator(textures, _matBasicLit, _matBasicUnlit, _matBasicUnlitGround, _graphics);
            _modelGenerator = new ModelGenerator(textures, models, _scene, _matBasicUnlit, _graphics);
        }

        private void InitializeScene()
        {
            _sceneController = new SceneController();
            _scene = _sceneController.CurrentScene;
        }

        private void InitializeSystems()
        {
            InitializePhysicsSystem();
            InitializePhysicsDebugSystem(true);
            InitializeCameraAndRenderSystems();
            InitializeAudioSystem();
        }

        private void InitializeAudioSystem()
        {
            if (_audioController == null) return;
        
            _audioController.PlayMusic();
           
        }

        private void GenerateBaseScene()
        {
            _sceneGenerator.GenerateScene(_scene);
        }

        private void InitializePhysicsDebugSystem(bool isEnabled)
        {
            var physicsDebugRenderer = _scene.AddSystem(new PhysicsDebugRenderer());

            // Toggle debug rendering on/off
            physicsDebugRenderer.Enabled = isEnabled; // or false to hide

            // Optional: Customize colors
            physicsDebugRenderer.StaticColor = Color.Green;      // Immovable objects
            physicsDebugRenderer.KinematicColor = Color.Blue;    // Animated objects
            physicsDebugRenderer.DynamicColor = Color.Yellow;    // Physics-driven objects
            physicsDebugRenderer.TriggerColor = Color.Red;       // Trigger volumes

        }

        private void InitializePhysicsSystem()
        {
            // 1. add physics
            var physicsSystem = _scene.AddSystem(new PhysicsSystem());
            physicsSystem.Gravity = AppData.GRAVITY;
        }

        private void InitializeCameraAndRenderSystems()
        {
            var cameraSystem = new CameraSystem(_graphics.GraphicsDevice, -100);
            _scene.Add(cameraSystem);

            var renderSystem = new RenderSystem(-100);
            _scene.Add(renderSystem);

            var uiRenderSystem = new UIRenderSystem(100);
            _scene.Add(uiRenderSystem); // draws in PostRender after RenderingSystem (order = -100)
        }

        private void InitializeInputSystem()
        {
            //set mouse, keyboard binding keys (e.g. WASD)
            var bindings = InputBindings.Default;
            // optional tuning
            bindings.MouseSensitivity = 0.12f;  // mouse look scale
            bindings.DebounceMs = 60;           // key/mouse debounce in ms
            bindings.EnableKeyRepeat = true;    // hold-to-repeat
            bindings.KeyRepeatMs = 300;         // repeat rate in ms

            // Create the input system 
            var inputSystem = new InputSystem();

            //register all the devices, you dont have to, but its for the demo
            inputSystem.Add(new GDKeyboardInput(bindings));
            inputSystem.Add(new GDMouseInput(bindings));
            inputSystem.Add(new GDGamepadInput(PlayerIndex.One, "Gamepad P1"));

            _scene.Add(inputSystem);
        }

        private void InitializeCameras()
        {

            #region First-person camera
            var position = new Vector3(0, 5, 25);

            //camera GO
            _cameraGO = new GameObject(AppData.CAMERA_NAME);
            //set position 
            _cameraGO.Transform.TranslateTo(position);
            //add camera component to the GO
            _camera = _cameraGO.AddComponent<Camera>();
            _camera.FarPlane = 1000;
            ////feed off whatever screen dimensions you set InitializeGraphics
            _camera.AspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _cameraGO.AddComponent<KeyboardWASDController>();
            _cameraGO.AddComponent<MouseYawPitchController>();

            // Add it to the scene
            _scene.Add(_cameraGO);
            #endregion

            var theCamera = _scene.Find(go => go.Name.Equals(AppData.CAMERA_NAME)).GetComponent<Camera>();
            _scene.SetActiveCamera(theCamera);
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
        private void InitializeUI()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            #region Core
            Time.Update(gameTime);

            _scene.Update(Time.DeltaTimeSecs);

            #endregion

            #region Game
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            _scene.Draw(Time.DeltaTimeSecs);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Override Dispose to clean up engine resources.
        /// MonoGame's Game class already implements IDisposable, so we override its Dispose method.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                base.Dispose(disposing);
                return;
            }

            if (disposing)
            {
                System.Diagnostics.Debug.WriteLine("Disposing Main...");

                System.Diagnostics.Debug.WriteLine("Disposing Scene");
                _scene?.Dispose();
                _scene = null;

                System.Diagnostics.Debug.WriteLine("Clearing MeshFilter Registry");
                MeshFilterFactory.ClearRegistry();

                _modelDictionary?.Dispose();
                _modelDictionary = null;

                System.Diagnostics.Debug.WriteLine("Disposing EngineContext");
                EngineContext.Instance?.Dispose();

                System.Diagnostics.Debug.WriteLine("Main disposal complete");
            }

            _disposed = true;

            // Always call base.Dispose
            base.Dispose(disposing);
        }

        #endregion    

        #region Game Methods

        private void DemoLoadFromJSON()
        {
            var relativeFilePathAndName = "assets/data/single_model_spawn.json";
            List<ModelSpawnData> mList = JSONSerializationUtility.LoadData<ModelSpawnData>(Content, relativeFilePathAndName);

            relativeFilePathAndName = "assets/data/multi_model_spawn.json";
            //load multiple models
            foreach (var d in JSONSerializationUtility.LoadData<ModelSpawnData>(Content, relativeFilePathAndName))
                _modelGenerator.GenerateModel(
                    d.Position, d.RotationDegrees, d.Scale, d.TextureName, d.ModelName, d.ObjectName);
        }
        #endregion

    }
}