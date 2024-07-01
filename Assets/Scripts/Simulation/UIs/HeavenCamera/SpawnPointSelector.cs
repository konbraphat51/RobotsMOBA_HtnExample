using System.Collections.Generic;
using System.Linq;
using Simulation.Objects.AcAgent;
using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.UIs.HeavenCamera
{
    /// <summary>
    /// Show available spawn points and selecting one
    /// </summary>
    public class SpawnPointSelector : MonoBehaviour
    {
        [SerializeField]
        private AcAgentCore targetAgent;

        [SerializeField]
        private SpawnIndicator spawnIndicatorPrefab;

        [SerializeField]
        private SpawnCursor spawnCursorPrefab;

        private Dictionary<SpawnPoint, SpawnIndicator> spawnPointsAvailable =
            new Dictionary<SpawnPoint, SpawnIndicator>();
        private SpawnPoint spawnPointSelecting;
        private SpawnCursor spawnCursor;

        private AgentManager agentManager;
        private Canvas canvasUsing;

        private void Start()
        {
            agentManager = UtilityFunctions.GetGameManager(gameObject).GetComponent<AgentManager>();
            canvasUsing = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            ControlIndicators();
            ControlCursor();
        }

        /// <summary>
        /// Control generation/deletion of indicators
        /// </summary>
        private void ControlIndicators()
        {
            Dictionary<SpawnPoint, SpawnIndicator> spawnPointsAvailableNew =
                new Dictionary<SpawnPoint, SpawnIndicator>();

            //get available spawn points
            HashSet<SpawnPoint> availableSpawnPoints = agentManager
                .GetAvailableSpawnPoints(targetAgent)
                .ToHashSet();

            //find new spawn points
            foreach (SpawnPoint spawnPoint in availableSpawnPoints)
            {
                //if not in the dictionary...
                if (!spawnPointsAvailable.ContainsKey(spawnPoint))
                {
                    //...generate UI
                    SpawnIndicator spawnIndicator = Instantiate(spawnIndicatorPrefab, transform);
                    spawnIndicator.Initialize(canvasUsing, spawnPoint);

                    //register to dictionary
                    spawnPointsAvailableNew.Add(spawnPoint, spawnIndicator);
                }
                //if continuing to be available...
                else
                {
                    //...register to dictionary
                    spawnPointsAvailableNew.Add(spawnPoint, spawnPointsAvailable[spawnPoint]);
                }
            }

            //delete disappered spawn points
            foreach (SpawnPoint spawnPoint in spawnPointsAvailable.Keys)
            {
                if (!availableSpawnPoints.Contains(spawnPoint))
                {
                    Destroy(spawnPointsAvailable[spawnPoint].gameObject);
                }
            }

            //update list
            spawnPointsAvailable = spawnPointsAvailableNew;
        }

        /// <summary>
        /// Control cursor
        /// </summary>
        private void ControlCursor()
        {
            SpawnPoint spawnPointSelectedNow = targetAgent.controller.spawnPointSelected;
            //if it's not available...
            if (
                (spawnPointSelectedNow != null)
                && !spawnPointsAvailable.ContainsKey(spawnPointSelectedNow)
            )
            {
                //...reset
                spawnPointSelectedNow = null;
            }

            //if none selected...
            if (spawnPointSelectedNow == null)
            {
                //...if UI exists...
                if (spawnCursor != null)
                {
                    //...delete
                    Destroy(spawnCursor.gameObject);
                    spawnCursor = null;
                }
            }
            //if selected...
            else
            {
                //...if UI does not exist...
                if (spawnCursor == null)
                {
                    //...generate
                    spawnCursor = Instantiate(spawnCursorPrefab, transform);
                    spawnCursor.Initialize(canvasUsing, spawnPointSelectedNow);

                    spawnPointSelecting = spawnPointSelectedNow;
                }

                //if target changed...
                if (spawnPointSelectedNow != spawnPointSelecting)
                {
                    //...update
                    spawnCursor.SetTarget(spawnPointSelectedNow);
                    spawnPointSelecting = spawnPointSelectedNow;
                }
            }
        }
    }
}
