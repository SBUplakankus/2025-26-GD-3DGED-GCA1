using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using GDEngine.Core.Systems;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Systems
{
    public class AudioController : Component
    {

        #region Fields
        private AudioSystem _audioSystem;
        private const float MUSIC_VOLUME = 0.5f;
        #endregion

        #region Constructors
        public AudioController(ContentDictionary<SoundEffect> sounds)
        {
            _audioSystem = new AudioSystem(sounds);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Play the games default base music
        /// </summary>
        public void PlayMusic()
        {
            _audioSystem.PlayMusic(AppData.MAIN_MUSIC, MUSIC_VOLUME);
        }

        private GameObject Generate3DAudioObject(SoundEffect sound, string name)
        {
            var soundGO = new GameObject(name);


            return soundGO;
        }
        #endregion

    }
}
