using Htn;
using Simulation.Objects.AcAgent;

namespace Ai.Htn.EasyExample
{
    /// <summary>
    /// lock on and attack
    /// </summary>
    public class TaskFindAndAttack : TaskCompound<AcAgentCore, WorldStateAc>
    {
        public override string name => "FindAndAttack";

        protected override void SetMethods()
        {
            methods = new Method[]
            {
                new Method()
                {
                    name = "Find+Attack",
                    conditions = new Condition<WorldStateAc>[] { new NotLockedOnCondition() },
                    subtasks = new Task<AcAgentCore, WorldStateAc>[]
                    {
                        new TaskLockonTurn(),
                        new TaskAttack()
                    }
                }
            };
        }

        public class NotLockedOnCondition : Condition<WorldStateAc>
        {
            protected override bool ValidateCondition(WorldStateAc worldState)
            {
                return worldState.LockedOn == WorldStateAc.LockedOnObject.None;
            }
        }
    }
}
