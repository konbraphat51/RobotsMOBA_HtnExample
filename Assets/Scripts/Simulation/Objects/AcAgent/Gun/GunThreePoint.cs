using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Three point gun for AcAgent
    /// </summary>
    public class GunThreePoint : GunCore
    {
        [SerializeField]
        private Transform muzzlePosition;

        [SerializeField]
        private Bullet bulletPrefab;

        [SerializeField]
        private float shortInterval = 0.1f;

        [SerializeField]
        private float longInterval = 0.5f;

        [SerializeField]
        private int burstMax = 3;

        [SerializeField]
        private float bulletspeed = 100f;

        [SerializeField]
        private float bulletDamage = 1f;

        [SerializeField]
        private float bulletLifeTime = 3f;

        private int bursted = 0;
        private float lastFireTime = float.MinValue;

        /// <summary>
        /// Shoot bullet toward target position
        /// </summary>
        /// <param name="targetPosition">target position</param>
        public override void Fire(AcAgentCore attacker, Vector3 targetPosition)
        {
            //this function does entire control

            //if all bursted...
            if (bursted >= burstMax)
            {
                //...if cooled...
                if (Time.time - lastFireTime > longInterval)
                {
                    //reset
                    bursted = 0;

                    ShootOneBullet(attacker, targetPosition);

                    bursted++;
                    lastFireTime = Time.time;
                }
            }
            //if not all...
            else
            {
                //...if cooled...
                if (Time.time - lastFireTime > shortInterval)
                {
                    ShootOneBullet(attacker, targetPosition);

                    bursted++;
                    lastFireTime = Time.time;
                }
            }
        }

        /// <summary>
        /// The logic of shooting one bullet
        /// </summary>
        /// <param name="targetPosition">target position</param>
        private void ShootOneBullet(AcAgentCore attacker, Vector3 targetPosition)
        {
            //generate bullet object
            Bullet bullet = Instantiate(
                    bulletPrefab.gameObject,
                    muzzlePosition.position,
                    muzzlePosition.rotation
                )
                .GetComponent<Bullet>();

            //delegate moving logic to Bullet class
            bullet.Initialize(
                attacker,
                muzzlePosition.position,
                (targetPosition - muzzlePosition.position).normalized * bulletspeed, //velocity
                bulletDamage,
                bulletLifeTime
            );
        }
    }
}
