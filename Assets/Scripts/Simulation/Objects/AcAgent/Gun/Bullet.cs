using Simulation.Utils;
using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Bullet shooted from AcAgent's gun
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        public AttackableObject attacker { get; private set; }
        public float damage { get; private set; }

        public void Initialize(
            AttackableObject attacker,
            Vector3 position,
            Vector3 velocity,
            float damage,
            float lifeTime
        )
        {
            this.attacker = attacker;
            this.damage = damage;

            //move to position
            transform.position = position;

            //move
            GetComponent<Rigidbody>().velocity = velocity;

            //disappear after lifeTime
            Destroy(gameObject, lifeTime);
        }

        public void OnCollisionEnter(Collision collision)
        {
            //if is attackable object...
            if (collision.gameObject.TryGetComponent(out AttackableObject attackableObject))
            {
                //if is not attacker itself...
                if (attackableObject != attacker)
                {
                    //give damage
                    attackableObject.TakeDamage(damage);

                    //disappear
                    Destroy(gameObject);
                }
            }
            //if map object...
            else if (UtilityFunctions.IsMapObject(collision.gameObject))
            {
                //disappear
                Destroy(gameObject);
            }
        }
    }
}
