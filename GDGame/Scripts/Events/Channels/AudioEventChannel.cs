using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDGame.Scripts.Events.Channels
{
    public class AudioEventChannel
    {
        public EventBase<string> PlayMusic;
        public EventBase<string> PlaySFX;

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
