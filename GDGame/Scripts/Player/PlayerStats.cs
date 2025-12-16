using GDEngine.Core.Timing;
using GDGame.Scripts.Events.Channels;
using System.Diagnostics;

namespace GDGame.Scripts.Player
{
    /// <summary>
    /// Controls the player stats in the game that are displayed in the Player HUD.
    /// Created and stored in <see cref="PlayerController"/>.
    /// </summary>
    public class PlayerStats
    {
        #region Fields
        private readonly int _startHealth = 100;
        private int _currentHealth;
        private int _orbsCollected;
        private float _timeRemaining;
        private bool _timerStarted = false;
        private bool _timerEnded = false;
        #endregion

        #region Accessors
        public int CurrentHealth => _currentHealth;
        public int OrbsCollected => _orbsCollected;
        public string TimeLeft
        {
            get
            {
                if (!_timerStarted && _timeRemaining == 0)

                    return "Time Remaining: 10:00";

                int minutes = (int)(_timeRemaining / 60);
                int seconds = (int)(_timeRemaining % 60);
                return $"Time Remaining: {minutes:D2}:{seconds:D2}";
            }
        }
        public bool IsTimeUp => _timeRemaining <=  0;
        #endregion

        #region Constructors
        public PlayerStats() { }
        #endregion

        #region Methods

        /// <summary>
        /// Set the health to the starter health amount and set orbs collected to 0
        /// </summary>
        public void Initialise()
        {
            _currentHealth = _startHealth;
            _orbsCollected = 0;
            _timeRemaining = 600f; //starts from 10 minutes 
            _timerStarted = false;
        }

        /// <summary>
        /// Remove health from the player by a given amount
        /// </summary>
        /// <param name="amount">Health to remove from players current health</param>
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth > 0) return;

            EventChannelManager.Instance.PlayerEvents.OnPlayerLose.Raise();
            Debug.WriteLine("gg");
        }

        /// <summary>
        /// Increment the players orb collected count by 1
        /// </summary>
        public void HandleOrbCollection()
        {
            _orbsCollected++;
        }

        public void StartTimer()
        {
            _timeRemaining = 600f;  //10 minutes
            _timerStarted = true;
        }

        public void HandleTimeCountdown()
        {
            if (!_timerStarted || _timeRemaining <= 0) 
                return;
            _timeRemaining -= Time.DeltaTimeSecs;

            if (_timeRemaining <= 0)
                _timeRemaining = 0;
        }

        public void AddTime(float seconds)
        {
            _timeRemaining += seconds;
        }

        public void StopTimer()
        {
            _timerStarted = false;
        }
        #endregion
    }
}
