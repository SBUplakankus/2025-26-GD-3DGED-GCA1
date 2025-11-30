using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDGame.Scripts.Player
{
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
        public void Initialise()
        {
            _currentHealth = _startHealth;
            _orbsCollected = 0;
        }
        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth > 0) return;

            Debug.WriteLine("gg");
        }

        public void HandleOrbCollection()
        {
            _orbsCollected++;
        }
        #endregion
    }
}
