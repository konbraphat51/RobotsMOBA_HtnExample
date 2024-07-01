using Simulation.Objects.Stragetic;
using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Controller that controls AcAgent
    /// </summary>
    public abstract class Controller : MonoBehaviour
    {
        [SerializeField]
        protected AcAgentCore targetControlling;

        public SpawnPoint spawnPointSelected { get; protected set; }

        protected virtual void Start()
        {
            TakeControl();
        }

        /// <summary>
        /// Select where to spawn
        /// </summary>
        /// <param name="spawnPointsAvailable">
        /// All the spawn points available
        /// </param>
        /// <returns>
        /// Selected spawn point. Null if not selected yet
        /// </returns>
        public abstract SpawnPoint SelectSpawnPoint(SpawnPoint[] spawnPointsAvailable);

        private void TakeControl()
        {
            targetControlling.TakeControl(this);
        }
    }
}
