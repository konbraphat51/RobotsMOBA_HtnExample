using System.Collections.Generic;
using System.Linq;
using Simulation.Objects.AcAgent;
using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Simulation
{
    /// <summary>
    /// Integrate game information so that each AI can access it.
    /// </summary>
    [RequireComponent(typeof(GameManager))]
    public class InformationManager : MonoBehaviour
    {
        /// <summary>
        /// Agents existing in the game.
        /// </summary>
        private List<AcAgentCore> agents = new List<AcAgentCore>();

        /// <summary>
        /// Main bases existing in the game.
        /// </summary>
        private List<MainBase> mainBases = new List<MainBase>();

        /// <summary>
        /// Relay points existing in the game.
        /// </summary>
        private List<RelayPoint> relayPoints = new List<RelayPoint>();

        public AcAgentCore[] agentsAll => agents.ToArray();
        public MainBase[] mainBasesAll => mainBases.ToArray();
        public RelayPoint[] relayPointsAll => relayPoints.ToArray();

        public AcAgentCore[] GetAgentsInTeam(Team team)
        {
            return agents.Where(agent => agent.team == team).ToArray();
        }

        public MainBase GetMainBaseOfTeam(Team team)
        {
            return mainBases.Where(mainBase => mainBase.team == team).FirstOrDefault();
        }

        private void Start()
        {
            FindAllAgents();
            FindAllBases();
            FindAllRelayPoints();
        }

        private void FindAllAgents()
        {
            agents = new List<AcAgentCore>();
            foreach (AcAgentCore agent in GetComponentsInChildren<AcAgentCore>())
            {
                agents.Add(agent);
            }
        }

        private void FindAllBases()
        {
            mainBases = new List<MainBase>();
            foreach (MainBase mainBase in GetComponentsInChildren<MainBase>())
            {
                mainBases.Add(mainBase);
            }
        }

        private void FindAllRelayPoints()
        {
            relayPoints = new List<RelayPoint>();
            foreach (RelayPoint relayPoint in GetComponentsInChildren<RelayPoint>())
            {
                relayPoints.Add(relayPoint);
            }
        }
    }
}
