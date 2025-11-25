using GDEngine.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GDGame.Scripts.Audio
{
    public class _3DAudioController : Component
    {
        #region Fields
        private AudioEmitter _emitter;
        private AudioListener _listener;
        private SoundEffectInstance _soundInstance;
        private Transform _soundTransform;
        private bool _looped;
        private bool _active;
        private float _volume;
        #endregion


        #region Constructors
        public _3DAudioController(AudioListener listener, Transform soundTransform, 
            SoundEffect sound, bool looped = true, float volume = 1f) 
        {
            _soundInstance = sound.CreateInstance();
            _soundTransform = soundTransform;
            _emitter = new AudioEmitter();
            _listener = listener;
            _looped = looped;
            _volume = volume;
            _active = true;

            Init3DSound();
        }
        #endregion

        #region Methods
        private void Init3DSound()
        {
            _emitter.Position = _soundTransform.Position;
            _emitter.Forward = _soundTransform.Forward;
            _emitter.Up = _soundTransform.Up;
            _emitter.Velocity = Vector3.Zero;
            _emitter.DopplerScale = 0.02f;

            _soundInstance.IsLooped = _looped;
            _soundInstance.Volume = _volume;
            _soundInstance.Apply3D(_listener, _emitter);
            _soundInstance.Play();
        }

        public void Toggle3DSound()
        {
            if(_active)
                _soundInstance.Stop();
            else
                _soundInstance.Play();

            _active = !_active;
        }
        #endregion
    }
}
