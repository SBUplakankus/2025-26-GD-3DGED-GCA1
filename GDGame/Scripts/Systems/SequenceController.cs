using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Events;
using GDEngine.Core.Orchestration;
using GDEngine.Core.Systems;
using static GDEngine.Core.Orchestration.Orchestrator;

namespace GDGame.Scripts.Systems
{
    /// <summary>
    /// Unused
    /// </summary>
    public class SequenceController
    {
        #region Fields
        private OrchestrationSystem _sequenceSystem;
        private Orchestrator _sequencer;
        #endregion

        #region Constructors
        public SequenceController()
        {
            _sequenceSystem = new OrchestrationSystem();
            _sequencer = _sequenceSystem.Orchestrator;
        }
        #endregion

        #region Methods
        private void BuildCameraIntroSequence()
        {
           
        }

        public void Initialise()
        {
            SceneController.AddToCurrentScene(_sequenceSystem);
        }
        #endregion
    }
}
