using System.Collections.Generic;
using System.Windows.Forms;
using GDEngine.Core;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Input.Data;
using GDEngine.Core.Orchestration;
using GDEngine.Core.Rendering.UI;
using GDEngine.Core.Serialization;
using GDEngine.Core.Services;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using GDEngine.Core.Utilities;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using GDGame.Scripts.Traps;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace GDGame
{
    public class Main : Game
    {
        #region Fields

        // Core
        private GraphicsDeviceManager _graphics;
        private ContentDictionary<Model> _modelDictionary;
        private ContentDictionary<Effect> _effectsDictionary;
        private Scene _scene;
        private bool _disposed = false;
        private OrchestrationSystem _orchestrationSystem;

        // Game Systems
        private AudioController _audioController;
        private SceneController _sceneController;
        private UserInterfaceController _uiController;
        private SceneGenerator _sceneGenerator;
        private ModelGenerator _modelGenerator;
        private MaterialGenerator _materialGenerator;
        private InputManager _inputManager;
        private TrapManager _trapManager;
        private TimeController _timeController;
        private LocalisationController _localisationController;

        // Player
        private PlayerController _playerController;

        // Events
        private InputEventChannel _inputEventChannel;
        private PlayerEventChannel _playerEventChannel;

        #endregion

        #region Constructor

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = AppData.CONTENT_ROOT;
        }

        #endregion

        #region Initialisation

        protected override void Initialize()
        {
            Window.Title = AppData.GAME_WINDOW_TITLE;

            InitializeGraphics(ScreenResolution.R_FHD_16_9_1920x1080);
            InitializeMouse();
            InitializeContext();
            GenerateMaterials();
            InitializeScene();
            LoadAssetsFromJSON(AppData.ASSET_MANIFEST_PATH);
            InitializeSystems();
            InitGameSystems();

            base.Initialize();
        }

        private void InitializeGraphics(Integer2 resolution)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ScreenResolution.SetResolution(_graphics, resolution);
            WindowUtility.CenterOnMonitor(this, 1);
        }

        private void InitializeMouse()
        {
            Mouse.SetPosition(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            IsMouseVisible = false;
        }

        private void InitializeContext()
        {
            EngineContext.Initialize(GraphicsDevice, Content);
        }

        private void InitLocalisation()
        {
            _localisationController = new LocalisationController();
        }

        private void GenerateMaterials()
        {
            _materialGenerator = new MaterialGenerator(_graphics);
        }

        private void InitializeScene()
        {
            _sceneController = new SceneController();
            _scene = _sceneController.CurrentScene;
        }

        private void InitEvents()
        {
            EventChannelManager.Initialise();
            _inputEventChannel = EventChannelManager.Instance.InputEvents;
            _playerEventChannel = EventChannelManager.Instance.PlayerEvents;
        }

        /// <summary>
        /// Load all the assets in the 'asset.manifest' JSON into Content Dictionaries and pass them through
        /// to the game controls
        /// </summary>
        /// <param name="relativeFilePathAndName">Path to the JSON File</param>
        private void LoadAssetsFromJSON(string relativeFilePathAndName)
        {
            var textures = new ContentDictionary<Texture2D>();
            var models = new ContentDictionary<Model>();
            var fonts = new ContentDictionary<SpriteFont>();
            _effectsDictionary = new ContentDictionary<Effect>();
            var sounds = new ContentDictionary<SoundEffect>();

            var manifests = JSONSerializationUtility.LoadData<AssetManifest>(Content, relativeFilePathAndName);

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
            _sceneGenerator = new SceneGenerator(textures, _materialGenerator.MatBasicLit, _materialGenerator.MatBasicUnlit,
                _materialGenerator.MatBasicUnlitGround, _graphics);
            _modelGenerator = new ModelGenerator(textures, models, _materialGenerator.MatBasicUnlit, _graphics);
        }

        private void InitializeSystems()
        {
            InitEvents();
            InitPhysicsSystem();
            InitPhysicsDebugSystem(true);
            InitCameraAndRenderSystems();
            InitAudioSystem();
            InitInputSystem();
            InitLocalisation();
            GenerateBaseScene();
            InitializeUI();
        }

        private void InitGameSystems()
        {
            InitPlayer();
            InitTraps();
            InitTime();
            DemoLoadFromJSON();
            TestObjectLoad();
        }

        private void InitPhysicsSystem()
        {
            var physicsSystem = _scene.AddSystem(new PhysicsSystem());
            physicsSystem.Gravity = AppData.GRAVITY;
        }

        private void InitPhysicsDebugSystem(bool isEnabled)
        {
            var physicsDebugRenderer = _scene.AddSystem(new PhysicsDebugRenderer());
            physicsDebugRenderer.Enabled = isEnabled;

            physicsDebugRenderer.StaticColor = Color.Green;
            physicsDebugRenderer.KinematicColor = Color.Blue;
            physicsDebugRenderer.DynamicColor = Color.Yellow;
            physicsDebugRenderer.TriggerColor = Color.Red;
        }

        private void InitCameraAndRenderSystems()
        {
            var cameraSystem = new CameraSystem(_graphics.GraphicsDevice, -AppData.RENDER_ORDER);
            _scene.Add(cameraSystem);

            var renderSystem = new RenderSystem(-AppData.RENDER_ORDER);
            _scene.Add(renderSystem);

            var uiRenderSystem = new UIRenderSystem(AppData.RENDER_ORDER);
            _scene.Add(uiRenderSystem);
        }

        private void InitAudioSystem()
        {
            if (_audioController == null) return;

            _audioController.PlayMusic();
            _audioController.Generate3DAudio();

            foreach (var sound in _audioController.SoundsList)
                _scene.Add(sound);
        }

        private void InitInputSystem()
        {
            _inputManager = new InputManager();

            InitInputEvents();

            var inputGO = new GameObject(AppData.INPUT_NAME);
            inputGO.AddComponent(_inputManager);

            _scene.Add(inputGO);
            _scene.Add(_inputManager.Input);
        }

        private void InitInputEvents()
        {
            _inputEventChannel.FullscreenToggle.Subscribe(HandleFullscreenToggle);
            _inputEventChannel.ApplicationExit.Subscribe(HandleGameExit);
        }

        private void GenerateBaseScene()
        {
            _sceneGenerator.GenerateScene(_scene);
        }

        private void InitializeUI()
        {
            _uiController.InitUserInterface();
            foreach (var obj in _uiController.UIObjects)
                _scene.Add(obj);
        }

        #endregion

        #region Game Systems

        private void InitPlayer()
        {
            _playerController = new PlayerController(
                (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight);

            _playerController.PlayerCamGO.AddComponent(_audioController);

            _scene.Add(_playerController.PlayerCamGO);
            _scene.Add(_playerController.PlayerGO);
            _scene.SetActiveCamera(_playerController.PlayerCam);
        }

        private void InitTraps()
        {
            _trapManager = new TrapManager();
            foreach (var trap in _trapManager.TrapList)
                _scene.Add(trap.TrapGO);
        }

        private void InitTime()
        {
            _timeController = new TimeController();
            _inputEventChannel.PauseToggle.Subscribe(_timeController.TogglePause);
        }

        #endregion

        #region Game Loop

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            _scene.Update(Time.DeltaTimeSecs);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _scene.Draw(Time.DeltaTimeSecs);
            base.Draw(gameTime);
        }

        #endregion

        #region Event Handlers

        private void HandleFullscreenToggle() => _graphics.ToggleFullScreen();
        private void HandleGameExit() => Application.Exit();

        #endregion

        #region Demo Methods

        /// <summary>
        /// Load the objects from the single model and multi model spawn JSON files
        /// </summary>
        private void DemoLoadFromJSON()
        {
            List<ModelSpawnData> mList = JSONSerializationUtility.LoadData<ModelSpawnData>(Content, AppData.SINGLE_MODEL_SPAWN_PATH);

            foreach (var d in JSONSerializationUtility.LoadData<ModelSpawnData>(Content, AppData.MULTI_MODEL_SPAWN_PATH))
                _scene.Add(_modelGenerator.GenerateModel(
                    d.Position, d.RotationDegrees, d.Scale, d.TextureName, d.ModelName, d.ObjectName));
        }

        private void TestObjectLoad()
        {
            GameObject player = _modelGenerator.GenerateModel(
                new Vector3(0, 5, 10),
                new Vector3(0, 0, 0),
                new Vector3(0.1f, 0.1f, 0.1f),
                "colormap", "ghost", "test");
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Clear all of the event channels
        /// </summary>
        private void UnscubscribeFromEvents()
        {
            _inputEventChannel.ClearEventChannel();
        }

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

            UnscubscribeFromEvents();
            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion
    }
}
