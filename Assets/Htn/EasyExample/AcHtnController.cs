using Htn;
using Simulation.Objects.AcAgent;
using UnityEngine;

namespace Ai.Htn.EasyExample
{
    /// <summary>
    /// control AcAgent according to HTN
    /// </summary>
    [RequireComponent(typeof(AcAgentCore))]
    [RequireComponent(typeof(SensorAc))]
    public class AcHtnController : MonoBehaviour
    {
        private HtnPlanner<AcAgentCore, WorldStateAc> htnPlanner =
            new HtnPlanner<AcAgentCore, WorldStateAc>();

        private AcAgentCore agent;
        private SensorAc sensor;

        private void Start()
        {
            agent = GetComponent<AcAgentCore>();
            sensor = GetComponent<SensorAc>();
        }

        private void Update()
        {
            if (!htnPlanner.isRunningPlan)
            {
                htnPlanner.StartRootTask(agent, new TaskRoot(), sensor.worldState);
            }

            htnPlanner.ActOnPlan(agent);
        }
    }
}
