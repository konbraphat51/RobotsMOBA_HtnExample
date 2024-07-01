using UnityEngine;

namespace Simulation.Utils
{
    /// <summary>
    /// Detects the collider is grounded or not
    /// </summary>
    public class GroundDetector : MonoBehaviour
    {
        public bool isGrounded { get; private set; } = false;

        public void OnTriggerEnter(Collider other)
        {
            //if is ground object...
            if (UtilityFunctions.IsMapObject(other.gameObject))
            {
                isGrounded = true;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            //if is ground object...
            if (UtilityFunctions.IsMapObject(other.gameObject))
            {
                isGrounded = false;
            }
        }
    }
}
