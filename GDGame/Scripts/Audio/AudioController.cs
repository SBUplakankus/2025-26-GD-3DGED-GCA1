using System.Collections.Generic;
using System.Numerics;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Systems;
using GDGame.Scripts.Events.Channels;
using GDGame.Scripts.Systems;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Audio
{
    /// <summary>
    /// Controls all Audio in the game and stores the Sound files
    /// </summary>
    public class AudioController : Component
    {

        #region Fields
        private AudioSystem _audioSystem;
        private ContentDictionary<SoundEffect> _sounds;
        private List<GameObject> _3DsoundsList;
        private AudioEventChannel _audioEventChannel;
        private const float MUSIC_VOLUME = 0.25f;
        private const float SFX_VOLUME = 0.8f;
        #endregion

        #region Constructors
        public AudioController(ContentDictionary<SoundEffect> sounds)
        {
            _sounds = sounds;
            _3DsoundsList = new();
            _audioSystem = new AudioSystem(_sounds);
        }
        #endregion

        #region Accessors
        public List<GameObject> SoundsList => _3DsoundsList;
        #endregion

        #region Methods
        /// <summary>
        /// Play the games default base music
        /// </summary>
        private void PlayMusic(string key)
        {
            _audioSystem.PlayMusic(key, MUSIC_VOLUME);
        }

        private void PlaySFX(string key)
        {
            _audioSystem.PlayOneShot(key);
        }

        /// <summary>
        /// Generate the 3D Audio Objects for the scene
        /// </summary>
        private void Generate3DAudio()
        {
            var obj1 = Generate3DAudioObject(AppData.RATTLE_KEY, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj1);

            var obj2 = Generate3DAudioObject(AppData.CAT_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj2);

            var obj3 = Generate3DAudioObject(AppData.DUNGEON_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj3);
        }

        private void Add3DAudioToScene()
        {
            foreach(var obj in _3DsoundsList)
                SceneController.AddToCurrentScene(obj);
        }

        private GameObject Generate3DAudioObject(string name, Vector3 position)
        {
            var soundGO = new GameObject(name);
            soundGO.Transform.TranslateTo(position);
            var audio = new _3DAudioController(_audioSystem.Listener, soundGO.Transform, _sounds.Get(name));
            soundGO.AddComponent(audio);
            return soundGO;
        }

        private void InitEventHandlers()
        {
            _audioEventChannel.OnMusicRequested.Subscribe(PlayMusic);
            _audioEventChannel.OnSFXRequested.Subscribe(PlaySFX);
        }
        public void Initialise()
        {
            _audioEventChannel = EventChannelManager.Instance.AudioEvents;
            PlayMusic(AppData.MAIN_MUSIC);
            Generate3DAudio();
            Add3DAudioToScene();
            InitEventHandlers();
        }
        #endregion

    }
}
