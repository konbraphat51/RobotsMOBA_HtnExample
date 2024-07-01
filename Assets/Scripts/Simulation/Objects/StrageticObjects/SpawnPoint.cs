using Simulation.Utils;
using UnityEngine;

namespace Simulation.Objects.Stragetic
{
    /// <summary>
    /// Spawn point for ACs
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Team _team;

        [SerializeField]
        private float spawnRadius = 5f;

        [SerializeField]
        private Vector3 spawnDirection = Vector3.forward;

        public Team team => _team;

        /// <summary>
        /// Activate AC object and bring it to the spawn point
        /// </summary>
        /// <param name="acAgent">
        /// AcAgent to spawn. This should be already instantiated
        /// </param>
        public void Spawn(AcAgent.AcAgentCore acAgent)
        {
            //activate
            acAgent.gameObject.SetActive(true);

            //bring to spawn point
            acAgent.transform.position = ComputeSpawnPosition();

            //set direction
            acAgent.transform.right = spawnDirection;

            //reset the agent
            acAgent.Restart();
        }

        /// <summary>
        /// Change team of the spawn point
        /// </summary>
        /// <param name="newTeam">next team</param>
        public void ChangeTeam(Team newTeam)
        {
            _team = newTeam;
        }

        /// <summary>
        /// Randomly compute where to spawn
        /// </summary>
        /// <returns>World positino to spawn</returns>
        private Vector3 ComputeSpawnPosition()
        {
            float radius = Random.Range(0f, spawnRadius);
            float angle = Random.Range(0f, 360f);

            return transform.position
                + Vector3.forward * radius * Mathf.Cos(angle)
                + Vector3.right * radius * Mathf.Sin(angle);
        }
    }
}
