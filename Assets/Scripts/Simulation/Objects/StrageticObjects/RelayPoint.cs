using System;
using System.Collections.Generic;
using System.Linq;
using Simulation.Objects.AcAgent;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.Objects.Stragetic
{
    /// <summary>
    /// Relay point for ACs to conquer
    /// </summary>
    public class RelayPoint : MonoBehaviour
    {
        [SerializeField]
        private SpawnPoint spawnPoint;

        [Serializable]
        public struct TeamAndMaterial
        {
            public Team team;
            public Material material;
        }

        [SerializeField]
        private TeamAndMaterial[] teamMaterials;

        [SerializeField]
        private Material materialDefault;

        [SerializeField]
        private float conquestMax = 1f;

        [SerializeField]
        private float conquestSpeedPerAgent = 0.25f;

        /// <summary>
        /// Team that currently owns/superior-in this relay point
        /// </summary>
        public Team teamSuperior { get; private set; } = Team.Undefined;
        public bool conquered => conquestProgress >= conquestMax;
        public float conquestProgress { get; private set; } = 0f;

        private HashSet<AcAgentCore> agentsInside = new HashSet<AcAgentCore>();

        private void Update()
        {
            ComputeConquest();
        }

        public void OnTriggerEnter(Collider other)
        {
            //if the object is an agent...
            if (other.TryGetComponent(out AcAgentCore agent))
            {
                //...add to the list
                agentsInside.Add(agent);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            //if the object is an agent...
            if (other.TryGetComponent(out AcAgentCore agent))
            {
                //...remove from the list
                agentsInside.Remove(agent);
            }
        }

        private void ComputeConquest()
        {
            //if nobody inside...
            if (agentsInside.Count == 0)
            {
                //...skip
                return;
            }

            //count the number of agents in the relay point
            Dictionary<Team, int> teamCount = new Dictionary<Team, int>();
            foreach (AcAgentCore agent in agentsInside)
            {
                if (teamCount.ContainsKey(agent.team))
                {
                    teamCount[agent.team]++;
                }
                else
                {
                    teamCount[agent.team] = 1;
                }
            }

            //sort the dictionary by the number of agents
            List<KeyValuePair<Team, int>> sortedTeamCount = teamCount.ToList();
            sortedTeamCount.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            //compute conquest points
            Team teamSuperiorNow = sortedTeamCount[0].Key;
            int agentsDiff;
            if (sortedTeamCount.Count == 1)
            {
                agentsDiff = sortedTeamCount[0].Value;
            }
            else
            {
                agentsDiff = sortedTeamCount[0].Value - sortedTeamCount[1].Value;
            }
            float pointsChange = conquestSpeedPerAgent * agentsDiff * Time.deltaTime;

            //if the conquered team still superior...
            if ((teamSuperiorNow == teamSuperior) && conquered)
            {
                //...skip
                return;
            }
            //if the team is the current sperior...
            else if (teamSuperiorNow == teamSuperior)
            {
                //...add points
                conquestProgress += pointsChange;

                //if successfully conqured...
                if (conquered)
                {
                    //erace exceeded points
                    conquestProgress = conquestMax;

                    //call the event logic
                    BeConqured(teamSuperior);
                }
            }
            //if the team is different...
            else
            {
                //...subtract points

                //if it was conquered...
                if (conquered)
                {
                    //...call the event logic
                    BeNotConqured();
                }

                conquestProgress -= pointsChange;

                //if exceeded 0...
                if (conquestProgress < 0)
                {
                    //...change the team
                    teamSuperior = teamSuperiorNow;
                    conquestProgress = -conquestProgress;
                }
            }
        }

        /// <summary>
        /// Logic when the relay point is conquered
        /// </summary>
        private void BeConqured(Team team)
        {
            //change material
            Material material = teamMaterials.First(pair => pair.team == team).material;
            GetComponent<MeshRenderer>().material = material;

            //set spawner
            spawnPoint.ChangeTeam(team);
        }

        private void BeNotConqured()
        {
            //change material
            GetComponent<MeshRenderer>().material = materialDefault;

            //set spawner
            spawnPoint.ChangeTeam(Team.Undefined);
        }
    }
}
