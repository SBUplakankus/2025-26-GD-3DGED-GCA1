namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Audio Events in the game such as Playing Music and Sound Effects.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class AudioEventChannel
    {
        public EventBase<string> PlayMusic = new();
        public EventBase<string> PlaySFX = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            PlayMusic.UnsubscribeAll();
            PlaySFX.UnsubscribeAll();
        }
    }
}
