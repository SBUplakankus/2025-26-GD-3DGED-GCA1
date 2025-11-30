using GDGame.Scripts.Systems;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Player Events in the game such as Winning, Losing and Game State Changes.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class PlayerEventChannel
    {
        public EventBase PlayerLose = new();
        public EventBase PlayerWin = new();
        public EventBase<GameState> GameStateChange = new();
        public EventBase<int> PlayerDamaged = new();
        public EventBase OrbCollected = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            PlayerLose.UnsubscribeAll();
            PlayerWin.UnsubscribeAll();
            GameStateChange.UnsubscribeAll();
            OrbCollected.UnsubscribeAll();
            PlayerDamaged.UnsubscribeAll();
        }
    }
}
