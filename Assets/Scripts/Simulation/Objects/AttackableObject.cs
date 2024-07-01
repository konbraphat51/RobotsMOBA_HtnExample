using Simulation.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Simulation.Objects
{
    /// <summary>
    /// Object that can
    /// - be attacked
    /// - be locked on
    /// </summary>
    public interface AttackableObject
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }

        /// <summary>
        /// Where to aim at
        /// </summary>
        public Vector3 centerPoint { get; }

        public Team team { get; }

        public float hp { get; }
        public float hpMax { get; }

        public void TakeDamage(float damage);
        public void ListenToDeath(UnityAction<AttackableObject> action);
        public void StopListeningToDeath(UnityAction<AttackableObject> action);
    }
}
