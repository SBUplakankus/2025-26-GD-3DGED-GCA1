using System.Collections.Generic;

namespace GDGame.Scripts.Events.Channels
{
    /// <summary>
    /// Unused
    /// </summary>
    public class EventBaseList
    {
        private readonly List<EventBase> _events;

        public EventBaseList(List<EventBase> events)
        {
            _events = events;
        }

        public void ClearList()
        {
            foreach(var e in _events)
                e.UnsubscribeAll();

            _events.Clear();
        }
    }
}
