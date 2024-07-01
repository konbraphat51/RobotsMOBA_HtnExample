namespace Ai.Htn
{
    /// <summary>
    /// World state for AC
    /// </summary>
    public struct WorldStateAc
    {
        /// <summary>
        /// Team
        /// </summary>
        public enum Team
        {
            Undefined,
            Mine,
            Enemy
        }

        /// <summary>
        /// Health of the agent.
        /// </summary>
        public enum Health
        {
            Sufficient, //75-100%
            SlightlyDamaged, //50-75%
            HighlyDamaged, //25-50%
            Critical, //0-25%
            Dead //0%
        }

        /// <summary>
        /// Distance
        /// </summary>
        public enum Distance
        {
            Melee,
            Close,
            Medium,
            Far,
            Unknown
        }

        public enum Boolean
        {
            True,
            False
        }

        public enum LockedOnObject
        {
            None,
            Enemy0,
            Enemy1,
            Enemy2,
            EnemyBase
        }

        /// <summary>
        /// Health of the agent.
        /// </summary>
        public Health MyBaseHealth;

        /// <summary>
        /// Health of the enemy base.
        /// </summary>
        public Health EnemyBaseHealth;

        /// <summary>
        /// Team of Relay0.
        /// </summary>
        public Team Relay0;

        /// <summary>
        /// Team of Relay1.
        /// </summary>
        public Team Relay1;

        /// <summary>
        /// Health of the agent.
        /// </summary>
        public Health MyHealth;

        /// <summary>
        /// Health of TeamMate0.
        /// </summary>
        public Health TeamMate0Health;

        /// <summary>
        /// Health of TeamMate1.
        /// </summary>
        public Health TeamMate1Health;

        /// <summary>
        /// Health of Enemy0.
        /// </summary>
        public Health Enemy0Health;

        /// <summary>
        /// Health of Enemy1.
        /// </summary>
        public Health Enemy1Health;

        /// <summary>
        /// Health of Enemy2.
        /// </summary>
        public Health Enemy2Health;

        /// <summary>
        /// Distance to MyBase.
        /// </summary>
        public Distance MyBaseDistance;

        /// <summary>
        /// Distance to EnemyBase.
        /// </summary>
        public Distance EnemyBaseDistance;

        /// <summary>
        /// Distance to Relay0.
        /// </summary>
        public Distance Relay0Distance;

        /// <summary>
        /// Distance to Relay1.
        /// </summary>
        public Distance Relay1Distance;

        /// <summary>
        /// Distance to TeamMate0.
        /// </summary>
        public Distance TeamMate0Distance;

        /// <summary>
        /// Distance to TeamMate1.
        /// </summary>
        public Distance TeamMate1Distance;

        /// <summary>
        /// Distance to Enemy0.
        /// </summary>
        public Distance Enemy0Distance;

        /// <summary>
        /// Distance to Enemy1.
        /// </summary>
        public Distance Enemy1Distance;

        /// <summary>
        /// Distance to Enemy2.
        /// </summary>
        public Distance Enemy2Distance;

        /// <summary>
        /// Indicates if the agent is covered from Enemy0.
        /// </summary>
        public Boolean CoveredFromEnemy0;

        /// <summary>
        /// Indicates if the agent is covered from Enemy1.
        /// </summary>
        public Boolean CoveredFromEnemy1;

        /// <summary>
        /// Indicates if the agent is covered from Enemy2.
        /// </summary>
        public Boolean CoveredFromEnemy2;

        /// <summary>
        /// Object that the agent is locked on.
        /// </summary>
        public LockedOnObject LockedOn;

        /// <summary>
        /// Constructor to fill every data.
        /// </summary>
        public WorldStateAc(
            Health MyBaseHealth,
            Health EnemyBaseHealth,
            Team Relay0,
            Team Relay1,
            Health MyHealth,
            Health TeamMate0Health,
            Health TeamMate1Health,
            Health Enemy0Health,
            Health Enemy1Health,
            Health Enemy2Health,
            Distance MyBaseDistance,
            Distance EnemyBaseDistance,
            Distance Relay0Distance,
            Distance Relay1Distance,
            Distance TeamMate0Distance,
            Distance TeamMate1Distance,
            Distance Enemy0Distance,
            Distance Enemy1Distance,
            Distance Enemy2Distance,
            Boolean CoveredFromEnemy0,
            Boolean CoveredFromEnemy1,
            Boolean CoveredFromEnemy2,
            LockedOnObject LockedOn
        )
        {
            this.MyBaseHealth = MyBaseHealth;
            this.EnemyBaseHealth = EnemyBaseHealth;
            this.Relay0 = Relay0;
            this.Relay1 = Relay1;
            this.MyHealth = MyHealth;
            this.TeamMate0Health = TeamMate0Health;
            this.TeamMate1Health = TeamMate1Health;
            this.Enemy0Health = Enemy0Health;
            this.Enemy1Health = Enemy1Health;
            this.Enemy2Health = Enemy2Health;
            this.MyBaseDistance = MyBaseDistance;
            this.EnemyBaseDistance = EnemyBaseDistance;
            this.Relay0Distance = Relay0Distance;
            this.Relay1Distance = Relay1Distance;
            this.TeamMate0Distance = TeamMate0Distance;
            this.TeamMate1Distance = TeamMate1Distance;
            this.Enemy0Distance = Enemy0Distance;
            this.Enemy1Distance = Enemy1Distance;
            this.Enemy2Distance = Enemy2Distance;
            this.CoveredFromEnemy0 = CoveredFromEnemy0;
            this.CoveredFromEnemy1 = CoveredFromEnemy1;
            this.CoveredFromEnemy2 = CoveredFromEnemy2;
            this.LockedOn = LockedOn;
        }
    }
}
