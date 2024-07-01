using Simulation.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Simulation.Objects.Stragetic
{
    /// <summary>
    /// MainBase that determines the winner
    /// </summary>
    public class MainBase : MonoBehaviour, AttackableObject
    {
        [SerializeField]
        private Transform _centerPoint;

        [SerializeField]
        private Team _team;

        [SerializeField]
        private float _hpMax = 1000f;

        public float hpMax
        {
            get => _hpMax;
            private set => _hpMax = value;
        }
        public float hp { get; private set; }
        public Vector3 centerPoint => _centerPoint.position;
        public Team team => _team;

        private UnityEvent<AttackableObject> onDeath = new UnityEvent<AttackableObject>();

        private void Awake()
        {
            hp = hpMax;
        }

        public void TakeDamage(float damage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                Die();
            }
        }

        public void ListenToDeath(UnityAction<AttackableObject> action)
        {
            onDeath.AddListener(action);
        }

        public void StopListeningToDeath(UnityAction<AttackableObject> action)
        {
            onDeath.RemoveListener(action);
        }

        private void Die()
        {
            //call callbacks
            onDeath.Invoke(this);

            Destroy(gameObject);
        }
    }
}
