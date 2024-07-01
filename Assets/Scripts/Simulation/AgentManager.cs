using System.Collections.Generic;
using System.Linq;
using Simulation.Objects;
using Simulation.Objects.AcAgent;
using Simulation.Objects.Stragetic;
using UnityEngine;

namespace Simulation
{
    /// <summary>
    /// Controls the data/generation of agents
    /// </summary>
    public class AgentManager : MonoBehaviour
    {
        [SerializeField]
        private SpawnPoint[] _spawnPoints;

        [SerializeField]
        [Tooltip("Seconds")]
        private float spawnInterval = 5f;

        /// <summary>
        /// Agents in the game
        /// </summary>
        public List<AcAgentCore> agents { get; private set; } = new List<AcAgentCore>();

        public SpawnPoint[] spawnPoints => _spawnPoints;

        private struct AgentSpawnData
        {
            public AcAgentCore agent;
            public float spawnTime;
        }

        private List<AgentSpawnData> spawningAgents = new List<AgentSpawnData>();

        private void Start()
        {
            RegisterAllAgents();
        }

        private void Update()
        {
            SpawnDeadAgents();
        }

        /// <summary>
        /// List up available spawn points for the agent
        ///
        /// "Available" rule is defined here
        /// </summary>
        /// <param name="acAgent">Who to spawn</param>
        /// <param name="spawnPoints">spawn points to look from</param>
        public SpawnPoint[] GetAvailableSpawnPoints(AcAgentCore acAgent)
        {
            return spawnPoints
                .Where(spawnPointData => spawnPointData.team == acAgent.team)
                .ToArray();
        }

        /// <summary>
        /// Get all agents in the children object
        ///  and give callbacks
        /// </summary>
        private void RegisterAllAgents()
        {
            //reset
            ClearAgentData();

            //find
            agents.AddRange(GetComponentsInChildren<AcAgentCore>());

            //register callback
            foreach (AcAgentCore agent in agents)
            {
                agent.ListenToDeath(OnAgentDestroyed);
            }
        }

        /// <summary>
        /// Erace all agents data including callbacks
        /// </summary>
        private void ClearAgentData()
        {
            //clear callback
            foreach (AcAgentCore agent in agents)
            {
                agent.StopListeningToDeath(OnAgentDestroyed);
            }

            //clear list
            agents.Clear();
        }

        /// <summary>
        /// Spawn killed agents if their time has come
        /// </summary>
        private void SpawnDeadAgents()
        {
            //per agent...
            for (int cnt = 0; cnt < spawningAgents.Count; cnt++)
            {
                AgentSpawnData spawnData = spawningAgents[cnt];
                AcAgentCore agent = spawnData.agent;

                //if able to spawn...
                if (Time.time >= spawnData.spawnTime)
                {
                    //...let agent select to spawn
                    SpawnPoint spawnPointSelected = agent.controller.SelectSpawnPoint(
                        GetAvailableSpawnPoints(agent)
                    );

                    //if spawn point selected...
                    if (spawnPointSelected != null)
                    {
                        //...spawn
                        spawnPointSelected.Spawn(agent);

                        //control loop sequence
                        spawningAgents.RemoveAt(cnt);
                        cnt--;
                    }
                }
            }
        }

        /// <summary>
        /// Callback when agent destroyed
        /// </summary>
        /// <param name="agentObject">
        /// destroyed agent
        /// </param>
        private void OnAgentDestroyed(AttackableObject agentObject)
        {
            AcAgentCore agent = agentObject as AcAgentCore;

            float spawnTime = Time.time + spawnInterval;
            spawningAgents.Add(new AgentSpawnData { agent = agent, spawnTime = spawnTime });
        }
    }
}
