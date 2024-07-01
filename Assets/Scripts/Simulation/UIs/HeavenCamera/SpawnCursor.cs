using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.UIs.HeavenCamera
{
    /// <summary>
    /// Show the selecting spawn point
    /// </summary>
    public class SpawnCursor : MonoBehaviour
    {
        private Canvas canvasUsing;
        private SpawnPoint targetSpawnPoint;

        private void Update()
        {
            SetPosition();
        }

        public void Initialize(Canvas canvasUsing, SpawnPoint spawnPoint)
        {
            this.canvasUsing = canvasUsing;
            SetTarget(spawnPoint);
        }

        public void SetTarget(SpawnPoint spawnPoint)
        {
            targetSpawnPoint = spawnPoint;
        }

        private void SetPosition()
        {
            transform.position = UtilityFunctions.ComputeCanvasPosition(
                targetSpawnPoint.transform.position,
                canvasUsing,
                Vector2.zero
            );
        }
    }
}
