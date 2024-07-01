using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Domain model of gun of AcAgent
    /// </summary>
    public abstract class GunCore : MonoBehaviour
    {
        /// <summary>
        /// Shoot bullet toward target position
        /// </summary>
        /// <param name="targetPosition">target position</param>
        public abstract void Fire(AcAgentCore attacker, Vector3 targetPosition);
    }
}
