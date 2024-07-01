using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.UIs.HeavenCamera
{
    /// <summary>
    /// Show an available spawn point
    /// </summary>
    public class SpawnIndicator : MonoBehaviour
    {
        private SpawnPoint targetSpawnPoint;
        private Canvas canvasUsing;

        private void Update()
        {
            SetPosition();
        }

        public void Initialize(Canvas canvasUsing, SpawnPoint spawnPoint)
        {
            targetSpawnPoint = spawnPoint;
            this.canvasUsing = canvasUsing;
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
