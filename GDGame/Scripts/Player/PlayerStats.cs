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
        #endregion

        #region Accessors
        public int CurrentHealth => _currentHealth;
        public int OrbsCollected => _orbsCollected;
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
        }

        /// <summary>
        /// Remove health from the player by a given amount
        /// </summary>
        /// <param name="amount">Health to remove from players current health</param>
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth > 0) return;

            Debug.WriteLine("gg");
        }

        /// <summary>
        /// Increment the players orb collected count by 1
        /// </summary>
        public void HandleOrbCollection()
        {
            _orbsCollected++;
        }

        #endregion
    }
}
