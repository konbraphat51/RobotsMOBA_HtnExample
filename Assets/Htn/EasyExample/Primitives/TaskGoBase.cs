using Htn;
using Simulation;
using Simulation.Objects.AcAgent;
using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Ai.Htn.EasyExample
{
    public class TaskGoBase : TaskPrimitive<AcAgentCore, WorldStateAc>
    {
        public override string name => "GoBase";

        private MainBase baseToGo;

        public override OperationResult Operate(AcAgentCore agentHtn)
        {
            //head to base
            Vector3 direction = baseToGo.transform.position - agentHtn.transform.position;
            agentHtn.Move(direction.normalized, 1f, false);

            return OperationResult.Running;
        }

        public override void Initialize(AcAgentCore agentHtn)
        {
            InformationManager informationManager = UtilityFunctions.GetInformationManager(
                agentHtn.gameObject
            );
            baseToGo = informationManager.GetMainBaseOfTeam(agentHtn.team);
        }

        public override void OnInterrupted()
        {
            //do nothing
        }

        protected override void SetConditions()
        {
            conditions = new Condition<WorldStateAc>[0];
        }

        protected override void SetEffects()
        {
            effects = new Effect<WorldStateAc>[0];
        }
    }
}
