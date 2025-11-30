namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Controls Input Events in the game such as Toggling Pause, Language Swapping, Exit and Fullscreen.
    /// Uses <see cref="EventBase"/> to host events.
    /// </summary>
    public class InputEventChannel
    {
        public EventBase FullscreenToggle = new();
        public EventBase PauseToggle = new();
        public EventBase ApplicationExit = new();
        public EventBase<int> MovementInput = new();
        public EventBase LanguageSwap = new();

        /// <summary>
        /// Unsubscribe from all events in the Channel
        /// </summary>
        public void ClearEventChannel()
        {
            FullscreenToggle.UnsubscribeAll();
            PauseToggle.UnsubscribeAll();
            ApplicationExit.UnsubscribeAll();
            MovementInput.UnsubscribeAll();
            LanguageSwap.UnsubscribeAll();
        }
    }
}
