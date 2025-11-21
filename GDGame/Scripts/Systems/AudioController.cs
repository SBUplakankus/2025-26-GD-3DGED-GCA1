using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Collections;
using GDEngine.Core.Components;
using GDEngine.Core.Systems;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Systems
{
    public class AudioController : Component
    {

        #region Fields
        private ContentDictionary<SoundEffect> _soundDictionary;
        private AudioSystem _audioSystem;
        #endregion

        #region Constructors
        public AudioController(ContentDictionary<SoundEffect> sounds)
        {
            _soundDictionary = sounds;
            _audioSystem = new AudioSystem(_soundDictionary);
        }
        #endregion

        #region Methods
        public void PlayMusic()
        {
            _audioSystem.PlayMusic("test-music", 0.5f);
        }
        #endregion

    }
}
