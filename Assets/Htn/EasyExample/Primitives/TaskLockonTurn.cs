using Htn;
using Simulation.Objects.AcAgent;

namespace Ai.Htn.EasyExample
{
    /// <summary>
    /// Task that turning around until locked on something
    /// </summary>
    public class TaskLockonTurn : TaskPrimitive<AcAgentCore, WorldStateAc>
    {
        public override string name => "LockonTurn";

        public override OperationResult Operate(AcAgentCore agentHtn)
        {
            //turn around
            agentHtn.RotateCameraHorizontal(0.1f);

            //if locked on...
            if (agentHtn.lockonTarget != null)
            {
                return OperationResult.Success;
            }

            return OperationResult.Running;
        }

        public override void Initialize(AcAgentCore agentHtn)
        {
            //do nothing
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
            LockedOnEffect lockedOnEffect = new LockedOnEffect();
            effects = new Effect<WorldStateAc>[] { lockedOnEffect };
        }

        public class LockedOnEffect : Effect<WorldStateAc>
        {
            protected override WorldStateAc ApplyEffect(WorldStateAc worldState)
            {
                worldState.LockedOn = WorldStateAc.LockedOnObject.Enemy0;
                return worldState;
            }
        }
    }
}
