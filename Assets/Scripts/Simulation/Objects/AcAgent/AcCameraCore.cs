using System.Collections.Generic;
using System.Linq;
using Simulation.UIs;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Domain model of camera of AcAgent
    ///
    /// To control camera, use AcAgentCore, NOT THIS!!
    /// </summary>
    public class AcCameraCore : MonoBehaviour
    {
        [SerializeField]
        private Transform cameraForcusPoint;

        [SerializeField]
        private RectTransform reticle;

        [SerializeField]
        private LockonCursor lockonCursor;

        private float reticleRadiusOnScreen
        {
            get { return reticle.rect.width / 2f; }
        }

        private float cameraFocusLength;

        private void Start()
        {
            InitializeCameraInformation();
        }

        /// <summary>
        /// Move camera
        /// </summary>
        public void MoveCamera(float cameraRotationHorizontal, float cameraRotationVertical)
        {
            //from camera POSITION should be opposite side from rotation
            cameraRotationHorizontal += 180f;
            cameraRotationVertical *= -1f;

            //set position
            float x =
                cameraFocusLength
                * Mathf.Cos(cameraRotationVertical * Mathf.Deg2Rad)
                * Mathf.Cos(cameraRotationHorizontal * Mathf.Deg2Rad);

            float y = cameraFocusLength * Mathf.Sin(-cameraRotationVertical * Mathf.Deg2Rad);

            float z =
                cameraFocusLength
                * Mathf.Cos(cameraRotationVertical * Mathf.Deg2Rad)
                * Mathf.Sin(cameraRotationHorizontal * Mathf.Deg2Rad);

            transform.position = new Vector3(x, y, z) + cameraForcusPoint.position;

            //set rotation
            transform.LookAt(cameraForcusPoint);
        }

        /// <summary>
        /// Get the most center-close agent
        /// </summary>
        /// <param name="me">AcAgentCore of who is doing this Lockon</param>
        /// <param name="availableDistance">max distance able to lock on</param>
        /// <returns>lockoned agent. null if none.</returns>
        public AttackableObject LockOn(AcAgentCore me, float availableDistance)
        {
            AttackableObject[] objectsVisible = UtilityFunctions.FindVisibleAttackableObjects(
                GetComponent<Camera>(),
                availableDistance,
                new AttackableObject[] { me }
            );

            //get nearest
            AttackableObject nearestAttackable = null;
            float nearestDistance = float.MaxValue;
            Vector3 reticlePosition = GetComponent<Camera>().WorldToScreenPoint(reticle.position);
            foreach (AttackableObject attackable in objectsVisible)
            {
                //if it's ally...
                if (attackable.team == me.team)
                {
                    //...skip
                    continue;
                }

                Vector3 cameraToTarget = attackable.centerPoint - transform.position;
                float distance = cameraToTarget.magnitude;

                //if it's not in front-side...
                if (Vector3.Dot(cameraToTarget, transform.forward) < 0)
                {
                    //...skip
                    continue;
                }

                //distance from center
                Vector3 targetOnScreen = GetComponent<Camera>()
                    .WorldToScreenPoint(attackable.centerPoint);
                //Vector3 -> Vector2 for on-screen distance
                float distanceOnScreen = Vector2.Distance(targetOnScreen, reticlePosition);

                //if it's not in reticle...
                if (distanceOnScreen > reticleRadiusOnScreen)
                {
                    //...skip
                    continue;
                }

                //if neareset...
                if (distance < nearestDistance)
                {
                    //...hold
                    nearestAttackable = attackable;
                    nearestDistance = distance;
                }
            }

            //if there is enemy in reticle...
            if (nearestAttackable != null)
            {
                //...show cursor
                lockonCursor.SetCursor(nearestAttackable.centerPoint);
            }
            //if there is no enemy in reticle...
            else
            {
                //...hide cursor
                lockonCursor.HideCursor();
            }

            return nearestAttackable;
        }

        private void InitializeCameraInformation()
        {
            //get camera focus length
            cameraFocusLength = Vector3.Distance(transform.position, cameraForcusPoint.position);
        }
    }
}
