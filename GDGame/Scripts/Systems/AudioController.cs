using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Systems;
using GDGame.Scripts.Audio;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Systems
{
    public class AudioController : Component
    {

        #region Fields
        private AudioSystem _audioSystem;
        private ContentDictionary<SoundEffect> _sounds;
        private List<GameObject> _3DsoundsList;
        private const float MUSIC_VOLUME = 0.5f;
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
        public void PlayMusic()
        {
            _audioSystem.PlayMusic(AppData.MAIN_MUSIC, MUSIC_VOLUME);
        }

        public void Generate3DAudio()
        {
            var obj1 = Generate3DAudioObject(AppData.FIRE_AUDIO_NAME, new Vector3(1000, 1, 1));
            _3DsoundsList.Add(obj1);

            var obj2 = Generate3DAudioObject(AppData.CAT_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj2);

            var obj3 = Generate3DAudioObject(AppData.DUNGEON_AUDIO_NAME, new Vector3(1, 1, 1));
            _3DsoundsList.Add(obj3);
        }

        private GameObject Generate3DAudioObject(string name, Vector3 position)
        {
            var soundGO = new GameObject(name);
            soundGO.Transform.TranslateTo(position);
            var audio = new _3DAudioController(_audioSystem.Listener, soundGO.Transform, _sounds.Get(name));
            soundGO.AddComponent(audio);
            return soundGO;
        }
        #endregion

    }
}
