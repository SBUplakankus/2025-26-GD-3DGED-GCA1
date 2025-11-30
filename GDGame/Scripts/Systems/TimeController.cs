using GDEngine.Core.Timing;
using GDGame.Scripts.Events.Channels;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Controls the time of the game using <see cref="Time.TimeScale"/>.
    /// </summary>
    public class TimeController
    {
        #region Fields
        private bool _isPlaying;
        #endregion

        #region Constructors
        public TimeController() 
        {
            _isPlaying = true;
            Time.TimeScale = 1.0f;
            EventChannelManager.Instance.InputEvents.OnPauseToggle.Subscribe(TogglePause);
        }
        #endregion

        #region Methods
        public void TogglePause()
        {
            if (_isPlaying)
                Time.TimeScale = 0;
            else
                Time.TimeScale = 1;

            _isPlaying = !_isPlaying;
        }
        #endregion
    }
}
