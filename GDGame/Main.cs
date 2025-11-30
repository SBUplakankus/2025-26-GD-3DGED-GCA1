using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using GDEngine.Core;
using GDEngine.Core.Collections;
using GDEngine.Core.Entities;
using GDEngine.Core.Factories;
using GDEngine.Core.Screen;
using GDEngine.Core.Serialization;
using GDEngine.Core.Services;
using GDEngine.Core.Systems;
using GDEngine.Core.Timing;
using GDEngine.Core.Utilities;
using GDGame.Scripts.Audio;
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
        private CinematicCamController _cineCamController;
        private GameStateManager _gameStateManager;

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
            InitializeContext();
            InitEvents();
            InitializeSystems();
            LoadAssetsFromJSON(AppData.ASSET_MANIFEST_PATH);
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
            LocalisationController.Initialise();
        }

        private void GenerateMaterials()
        {
            _materialGenerator = new MaterialGenerator(_graphics);
        }

        private void InitScene()
        {
            _sceneController = new SceneController(this);
            _sceneController.Initialise();
            _scene = _sceneController.CurrentScene;
            Components.Add(_sceneController);
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
            _uiController = new UserInterfaceController(EngineContext.Instance.SpriteBatch ,fonts, textures);
            _sceneGenerator = new SceneGenerator(textures, _materialGenerator.MatBasicLit, _materialGenerator.MatBasicUnlit,
                _materialGenerator.MatBasicUnlitGround, _graphics);
            _modelGenerator = new ModelGenerator(textures, models, _materialGenerator.MatBasicUnlit, _graphics);
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

        private void InitializeSystems()
        {
            InitializeMouse();
            GenerateMaterials();
            InitScene();
            InitPhysicsSystem();
            InitPhysicsDebugSystem(true);
            InitCameraAndRenderSystems();
        }

        #endregion

        #region Game Systems
        private void InitAudioSystem()
        {
            _audioController.Initialise();
        }
        private void InitInputSystem()
        {
            _inputManager = new InputManager();
            _inputManager.Initialise();
            _inputEventChannel.OnFullscreenToggle.Subscribe(HandleFullscreenToggle);
            _inputEventChannel.OnApplicationExit.Subscribe(HandleGameExit);
        }

        private void GenerateBaseScene()
        {
            _sceneGenerator.GenerateScene(_scene);
        }

        private void InitializeUI()
        {
            _uiController.Initialise(_playerController.Stats);
        }
        private void InitPlayer()
        {
            var aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _playerController = new PlayerController(aspectRatio, _audioController);
        }

        private void InitTraps()
        {
            _trapManager = new TrapManager();
        }

        private void InitTime()
        {
            _timeController = new TimeController();
        }

        private void InitCineCam()
        {
            var aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _cineCamController = new CinematicCamController(aspectRatio);
            _cineCamController.Initialise();
        }

        private void InitGameStateManager()
        {
            _gameStateManager = new GameStateManager();
        }
        private void InitGameSystems()
        {
            GenerateBaseScene();
            InitInputSystem();
            InitLocalisation();
            InitPlayer();
            InitializeUI();
            InitAudioSystem();
            InitCineCam();
            InitTraps();
            InitTime();
            InitGameStateManager();
            DemoLoadFromJSON();
            TestObjectLoad();
        }

        #endregion

        #region Game Loop
        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
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
            EventChannelManager.Instance.ClearEventChannels();
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
