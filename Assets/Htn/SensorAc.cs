using System;
using System.Linq;
using Simulation;
using Simulation.Objects;
using Simulation.Objects.AcAgent;
using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Ai.Htn
{
    /// <summary>
    /// Sensor for the AC's world state and make WorldState data
    /// </summary>
    [RequireComponent(typeof(AcAgentCore))]
    public class SensorAc : MonoBehaviour
    {
        [Header("DistanceThreadholds")]
        private float distanceFar = 20f;
        private float distanceMedium = 10f;
        private float distanceClose = 5f;

        /// <summary>
        /// World state of the agent
        /// </summary>
        public WorldStateAc worldState
        {
            get
            {
                if (!hasSensedOnce)
                {
                    Sense();
                }

                return _worldState;
            }
        }

        private WorldStateAc _worldState;
        private AcAgentCore acAgentCore;
        private InformationManager informationManager;
        private bool hasSensedOnce = false;

        private void Start()
        {
            acAgentCore = GetComponent<AcAgentCore>();
            informationManager = UtilityFunctions.GetInformationManager(gameObject);
        }

        private void Update()
        {
            Sense();
        }

        /// <summary>
        /// Sense the world state and pass it to HtnPlanner
        /// </summary>
        private void Sense()
        {
            hasSensedOnce = true;

            MainBase myBase = informationManager.GetMainBaseOfTeam(acAgentCore.team);
            MainBase enemyBase = informationManager.GetMainBaseOfTeam(
                UtilityFunctions.GetEnemyTeam(acAgentCore.team)
            );
            RelayPoint[] relayPoints = informationManager.relayPointsAll;

            WorldStateAc.Health myBaseHealth = SenseHealth(myBase);
            WorldStateAc.Health enemyBaseHealth = SenseHealth(enemyBase);

            WorldStateAc.Team[] relays = new WorldStateAc.Team[relayPoints.Length];
            for (int cnt = 0; cnt < relayPoints.Length; cnt++)
            {
                RelayPoint relayPoint = relayPoints[cnt];
                relays[cnt] = SenseTeam(relayPoint, acAgentCore);
            }

            AcAgentCore[] teamMates = informationManager
                .GetAgentsInTeam(acAgentCore.team)
                .Where(agent => agent != acAgentCore)
                .ToArray();
            AcAgentCore[] enemies = informationManager.GetAgentsInTeam(
                UtilityFunctions.GetEnemyTeam(acAgentCore.team)
            );

            WorldStateAc.Health myHealth = SenseHealth(acAgentCore);
            WorldStateAc.Health[] teamMateHealths = new WorldStateAc.Health[teamMates.Length];
            for (int cnt = 0; cnt < teamMates.Length; cnt++)
            {
                AcAgentCore teamMate = teamMates[cnt];
                teamMateHealths[cnt] = SenseHealth(teamMate);
            }
            WorldStateAc.Health[] enemyHealths = new WorldStateAc.Health[enemies.Length];
            for (int cnt = 0; cnt < enemies.Length; cnt++)
            {
                AcAgentCore enemy = enemies[cnt];
                enemyHealths[cnt] = SenseHealth(enemy);
            }
            WorldStateAc.Distance myBaseDistance = SenseDistance(
                acAgentCore.transform,
                myBase.transform
            );
            WorldStateAc.Distance enemyBaseDistance = SenseDistance(
                acAgentCore.transform,
                enemyBase.transform
            );
            WorldStateAc.Distance[] relaysDistances = new WorldStateAc.Distance[relayPoints.Length];
            for (int cnt = 0; cnt < relayPoints.Length; cnt++)
            {
                RelayPoint relayPoint = relayPoints[cnt];
                relaysDistances[cnt] = SenseDistance(acAgentCore.transform, relayPoint.transform);
            }
            WorldStateAc.Distance[] teamMateDistances = new WorldStateAc.Distance[teamMates.Length];
            for (int cnt = 0; cnt < teamMates.Length; cnt++)
            {
                AcAgentCore teamMate = teamMates[cnt];
                teamMateDistances[cnt] = SenseDistance(acAgentCore.transform, teamMate.transform);
            }
            WorldStateAc.Distance[] enemyDistances = new WorldStateAc.Distance[enemies.Length];
            for (int cnt = 0; cnt < enemies.Length; cnt++)
            {
                AcAgentCore enemy = enemies[cnt];
                enemyDistances[cnt] = SenseDistance(acAgentCore.transform, enemy.transform);
            }
            WorldStateAc.Boolean[] coveredFromEnemies = new WorldStateAc.Boolean[enemies.Length];
            for (int cnt = 0; cnt < enemies.Length; cnt++)
            {
                AcAgentCore enemy = enemies[cnt];
                coveredFromEnemies[cnt] = SenseCovered(acAgentCore.gameObject, enemy.gameObject);
            }
            WorldStateAc.LockedOnObject lockedOn = SenseLockedOnObject(
                acAgentCore,
                enemies,
                enemyBase
            );

            _worldState = new WorldStateAc(
                myBaseHealth,
                enemyBaseHealth,
                relays[0],
                relays[1],
                myHealth,
                teamMateHealths[0],
                teamMateHealths[1],
                enemyHealths[0],
                enemyHealths[1],
                enemyHealths[2],
                myBaseDistance,
                enemyBaseDistance,
                relaysDistances[0],
                relaysDistances[1],
                teamMateDistances[0],
                teamMateDistances[1],
                enemyDistances[0],
                enemyDistances[1],
                enemyDistances[2],
                coveredFromEnemies[0],
                coveredFromEnemies[1],
                coveredFromEnemies[2],
                lockedOn
            );
        }

        /// <summary>
        /// Sense the health of the Attackable object
        /// </summary>
        /// <param name="obj">
        /// Attackable object to get health
        /// </param>
        /// <returns>
        /// helath enum
        /// </returns>
        private WorldStateAc.Health SenseHealth(AttackableObject obj)
        {
            float health = obj.hp;
            float healthMax = obj.hpMax;
            if (health >= healthMax * 0.75f)
            {
                return WorldStateAc.Health.Sufficient;
            }
            else if (health >= healthMax * 0.5f)
            {
                return WorldStateAc.Health.SlightlyDamaged;
            }
            else if (health >= healthMax * 0.25f)
            {
                return WorldStateAc.Health.HighlyDamaged;
            }
            else
            {
                return WorldStateAc.Health.Critical;
            }
        }

        private WorldStateAc.Team SenseTeam(RelayPoint relayPoint, AcAgentCore acAgentCore)
        {
            //if not conquered yet...
            if (!relayPoint.conquered)
            {
                return WorldStateAc.Team.Undefined;
            }
            // if this agent's team conquered it...
            else if (relayPoint.teamSuperior == acAgentCore.team)
            {
                return WorldStateAc.Team.Mine;
            }
            // if enemy team conquered it...
            else
            {
                return WorldStateAc.Team.Enemy;
            }
        }

        private WorldStateAc.Distance SenseDistance(Transform obj0, Transform obj1)
        {
            float distance = Vector3.Distance(obj0.position, obj1.position);
            if (distance > distanceFar)
            {
                return WorldStateAc.Distance.Far;
            }
            else if (distance > distanceMedium)
            {
                return WorldStateAc.Distance.Medium;
            }
            else if (distance > distanceClose)
            {
                return WorldStateAc.Distance.Close;
            }
            else
            {
                return WorldStateAc.Distance.Melee;
            }
        }

        private WorldStateAc.Boolean SenseCovered(GameObject obj0, GameObject obj1)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                obj0.transform.position,
                obj1.transform.position - obj0.transform.position,
                Vector3.Distance(obj0.transform.position, obj1.transform.position)
            );
            foreach (RaycastHit hit in hits)
            {
                if (UtilityFunctions.IsMapObject(hit.collider.gameObject))
                {
                    return WorldStateAc.Boolean.False;
                }
            }
            return WorldStateAc.Boolean.True;
        }

        private WorldStateAc.LockedOnObject SenseLockedOnObject(
            AcAgentCore acAgentCore,
            AcAgentCore[] enemies,
            MainBase enemyBase
        )
        {
            AttackableObject lockedOn = acAgentCore.lockonTarget;
            if (lockedOn == null)
            {
                return WorldStateAc.LockedOnObject.None;
            }

            //cannot use `switch` because it's not constant
            GameObject lockedOnObject = lockedOn.gameObject;

            if (lockedOnObject == enemyBase.gameObject)
            {
                return WorldStateAc.LockedOnObject.EnemyBase;
            }
            else if (lockedOnObject == enemies[0].gameObject)
            {
                return WorldStateAc.LockedOnObject.Enemy0;
            }
            else if (lockedOnObject == enemies[1].gameObject)
            {
                return WorldStateAc.LockedOnObject.Enemy1;
            }
            else if (lockedOnObject == enemies[2].gameObject)
            {
                return WorldStateAc.LockedOnObject.Enemy2;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
