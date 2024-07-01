using Htn;
using Simulation.Objects.AcAgent;
using UnityEngine;

namespace Ai.Htn.EasyExample
{
    /// <summary>
    /// Task that can attack for 3 seconds if locked on
    /// </summary>
    public class TaskAttack : TaskPrimitive<AcAgentCore, WorldStateAc>
    {
        public override string name => "Attack";

        public class LockedOnCondition : Condition<WorldStateAc>
        {
            protected override bool ValidateCondition(WorldStateAc worldState)
            {
                return worldState.LockedOn != WorldStateAc.LockedOnObject.None;
            }
        }

        private float timeStarted;

        public override OperationResult Operate(AcAgentCore agentHtn)
        {
            agentHtn.Attack();

            if (Time.time - timeStarted > 3.0f)
            {
                return OperationResult.Success;
            }

            return OperationResult.Running;
        }

        public override void Initialize(AcAgentCore agentHtn)
        {
            timeStarted = Time.time;
        }

        public override void OnInterrupted()
        {
            //do nothing
        }

        protected override void SetConditions()
        {
            LockedOnCondition lockedOnCondition = new LockedOnCondition();
            conditions = new Condition<WorldStateAc>[] { lockedOnCondition };
        }

        protected override void SetEffects()
        {
            effects = new Effect<WorldStateAc>[0];
        }
    }
}
