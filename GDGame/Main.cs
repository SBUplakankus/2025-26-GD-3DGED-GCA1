using System.Collections.Generic;
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
using GDGame.Scripts.DemoGame;
using GDGame.Scripts.Player;
using GDGame.Scripts.Systems;
using GDGame.Scripts.Traps;
using GDGame.Scripts.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using GDGame.Scripts.Demo_Game;

namespace GDGame
{
    public class Main : Game
    {
        #region Fields

        // Core
        private readonly GraphicsDeviceManager _graphics;
        private PhysicsSystem _physicsSystem;
        private PhysicsDebugSystem _physicsDebugRenderer;
        private Scene _scene;
        private Vector2 _screenCentre;
        private bool _disposed = false;

        // Game Systems
        private AudioController _audioController;
        private SceneController _sceneController;
        private UserInterfaceController _uiController;
        private SceneGenerator _sceneGenerator;
        private ModelGenerator _modelGenerator;
        private MaterialGenerator _materialGenerator;
        private InputManager _inputManager;
        private readonly TrapManager _trapManager;
        private TimeController _timeController;
        private CinematicCamController _cineCamController;
        private GameStateManager _gameStateManager;
        private GameOverObject _gameOver;
        private GameWonObject _gameWon;

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
            _screenCentre = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
        }

        private void InitializeMouse()
        {
            Mouse.SetPosition((int)_screenCentre.X, (int) _screenCentre.Y);
            IsMouseVisible = true;
        }

        private void InitializeContext()
        {
            EngineContext.Initialize(GraphicsDevice, Content);
        }

        private static void InitLocalisation()
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

            // _inputEventChannel.OnPauseToggle.Subscribe(HandlePauseToggle);

            SceneController.AddToCurrentScene(new EventSystem(EngineContext.Instance.Events));
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
                }
            }

            _audioController = new AudioController(sounds);
            _uiController = new UserInterfaceController(EngineContext.Instance.SpriteBatch ,fonts, textures, _screenCentre, this);
            _sceneGenerator = new SceneGenerator(textures, _materialGenerator.MatBasicLit, _materialGenerator.MatBasicUnlit,
                _materialGenerator.MatBasicUnlitGround, _graphics);
            _modelGenerator = new ModelGenerator(textures, models, _materialGenerator.MatBasicUnlit, _graphics);
        }

        private void InitPhysicsSystem()
        {
            _physicsSystem = new PhysicsSystem()
            {
                Gravity = AppData.GRAVITY
            };

            InitPhysicsDebugSystem(true);

            _scene.AddSystem(_physicsDebugRenderer);
            _scene.AddSystem(_physicsSystem);
        }

        private void InitPhysicsDebugSystem(bool isEnabled)
        {
            _physicsDebugRenderer = new PhysicsDebugSystem()
            {
                Enabled = isEnabled,
                StaticColor = Color.Green,
                KinematicColor = Color.Blue,
                DynamicColor = Color.Yellow,
                TriggerColor = Color.Red
            };
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
            InitEvents();
            InitPhysicsSystem();
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
            SceneController.AddToCurrentScene(new UIEventSystem());
            _uiController.Initialise(_playerController.Stats);
        }
        private void InitPlayer()
        {
            var aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            _playerController = new PlayerController(aspectRatio, _audioController);
        }

        private static void InitTraps()
        {
            //_trapManager = new TrapManager();
            var trapManagerGO = new GameObject("TrapManagerGO");
            trapManagerGO.AddComponent<TrapManager>();
            SceneController.AddToCurrentScene(trapManagerGO);
        }

        private void InitTime()
        {
            _timeController = new TimeController(_physicsDebugRenderer, _physicsSystem);
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
        private void InitDemoGameEvents()
        {
            _gameOver = new GameOverObject();
            _gameWon = new GameWonObject();
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
            InitDemoGameEvents();
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
        private void HandlePauseToggle() => IsMouseVisible = !IsMouseVisible;

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
        #endregion

        #region Disposal

        /// <summary>
        /// Clear all of the event channels
        /// </summary>
        private static void UnsubscribeFromEvents()
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

                System.Diagnostics.Debug.WriteLine("Disposing UI");
                _uiController?.Dispose();
                _uiController = null;

                System.Diagnostics.Debug.WriteLine("Disposing Scene");
                _sceneController?.Dispose();
                _sceneController = null;

                _scene?.Dispose();
                _scene = null;

                System.Diagnostics.Debug.WriteLine("Disposing Graphics");
                MeshFilterFactory.ClearRegistry();
                
                _modelGenerator?.Dispose();
                _modelGenerator = null;

                _materialGenerator?.Dispose();
                _materialGenerator = null;

                _sceneGenerator?.Dispose();
                _sceneGenerator = null;

                System.Diagnostics.Debug.WriteLine("Disposing Physics");
                _physicsSystem?.Dispose();
                _physicsSystem = null;

                _physicsDebugRenderer = null;

                System.Diagnostics.Debug.WriteLine("Disposing Audio");
                _audioController?.Dispose();
                _audioController = null;

                System.Diagnostics.Debug.WriteLine("Disposing EngineContext");
                EngineContext.Instance?.Dispose();

                System.Diagnostics.Debug.WriteLine("Main disposal complete");
            }

            UnsubscribeFromEvents();
            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion
    }
}
