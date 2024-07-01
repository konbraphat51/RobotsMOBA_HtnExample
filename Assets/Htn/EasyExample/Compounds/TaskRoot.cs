using Htn;
using Simulation.Objects.AcAgent;

namespace Ai.Htn.EasyExample
{
    public class TaskRoot : TaskCompound<AcAgentCore, WorldStateAc>
    {
        public override string name => "Root";

        protected override void SetMethods()
        {
            methods = new Method[]
            {
                new Method()
                {
                    name = "go to base",
                    conditions = new Condition<WorldStateAc>[] { new DamagedCondition() },
                    subtasks = new Task<AcAgentCore, WorldStateAc>[] { new TaskGoBase() }
                },
                new Method()
                {
                    name = "FindAndAttack",
                    conditions = new Condition<WorldStateAc>[0],
                    subtasks = new Task<AcAgentCore, WorldStateAc>[] { new TaskFindAndAttack() }
                },
                new Method()
                {
                    name = "simple attack",
                    conditions = new Condition<WorldStateAc>[]
                    {
                        new TaskAttack.LockedOnCondition()
                    },
                    subtasks = new Task<AcAgentCore, WorldStateAc>[] { new TaskAttack() }
                },
            };
        }

        public class DamagedCondition : Condition<WorldStateAc>
        {
            protected override bool ValidateCondition(WorldStateAc worldState)
            {
                return (worldState.MyHealth == WorldStateAc.Health.HighlyDamaged)
                    || (worldState.MyHealth == WorldStateAc.Health.Critical);
            }
        }
    }
}
