using System.Collections.Generic;
using System.Linq;
using Simulation.Objects;
using UnityEngine;

namespace Simulation.Utils
{
    /// <summary>
    /// static helper functions
    /// </summary>
    public static class UtilityFunctions
    {
        const string tagMapObject = "MapObject";

        /// <summary>
        /// Get GameManager that responsible for the object
        /// </summary>
        /// <param name="self">whose GameManager you want</param>
        /// <returns>reference to the responsible GameManager</returns>
        public static GameManager GetGameManager(GameObject self)
        {
            return self.GetComponentInParent<GameManager>();
        }

        /// <summary>
        /// Get InformationManager that responsible for the object
        /// </summary>
        /// <param name="self">
        /// Whose InformationManager you want
        /// </param>
        /// <returns>
        /// Reference to the responsible InformationManager
        /// </returns>
        public static InformationManager GetInformationManager(GameObject self)
        {
            return self.GetComponentInParent<InformationManager>();
        }

        /// <summary>
        /// Get Team of the object
        ///
        /// blue -> red
        /// red -> blue
        /// </summary>
        public static Team GetEnemyTeam(Team team)
        {
            return team == Team.Blue ? Team.Red : Team.Blue;
        }

        /// <summary>
        /// The specified obj is map object.
        /// </summary>
        public static bool IsMapObject(GameObject obj)
        {
            return obj.CompareTag(tagMapObject);
        }

        /// <summary>
        /// Make UI on The position seen in Canvas
        ///
        /// world -> screen -> z = 0 -> world
        /// </summary>
        /// <param name="positionWorld">Position in the scene world</param>
        /// <param name="canvasUsing">Canvas that using</param>
        /// <param name="OffsetOnScreen">Offset appplied by on-screen position</param>
        /// <returns>Vector3 position that should be into the transform.position</returns>
        public static Vector3 ComputeCanvasPosition(
            Vector3 positionWorld,
            Canvas canvasUsing,
            Vector2 offsetOnScreen
        )
        {
            Camera cameraUsing = canvasUsing.worldCamera;

            //position ON canvas
            Vector3 screenPosition = cameraUsing.WorldToScreenPoint(positionWorld);
            screenPosition.z = canvasUsing.planeDistance;

            // give offset
            screenPosition += new Vector3(offsetOnScreen.x, offsetOnScreen.y, 0f);

            //set to its word position
            return cameraUsing.ScreenToWorldPoint(screenPosition);
        }

        /// <summary>
        /// Get all visible in camera objects
        /// </summary>
        /// <param name="range">range from camera to count in</param>
        /// <param name="cameraUsing">camera that viewing from</param>
        /// <param name="excludings">objects to exclude</param>
        /// <typeparam name="T">component that this function want to get</typeparam>
        /// <returns>All visible objects</returns>
        public static T[] FindVisibleObjects<T>(
            Camera cameraUsing,
            float range,
            T[] excludings = null
        )
        {
            //in range
            T[] componentsInRange = GetAllObjectsInRange<T>(cameraUsing.transform.position, range);
            GameObject[] objectsInRange = componentsInRange
                .Where(component => !excludings.Contains(component))
                .Select(component => component as GameObject)
                .ToArray();

            // check obstacles...
            List<T> result = new List<T>();
            foreach (GameObject target in objectsInRange)
            {
                bool visible = HasObstacleBetween(
                    target.transform.position,
                    cameraUsing.transform.position
                );

                bool onFrontSide =
                    Vector3.Dot(
                        target.transform.position - cameraUsing.transform.position,
                        cameraUsing.transform.forward
                    ) > 0;

                if (visible && onFrontSide)
                {
                    result.Add(target.GetComponent<T>());
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get all visible AttackableObjects in camera
        /// </summary>
        /// <param name="cameraUsing">
        /// camera that viewing from
        /// </param>
        /// <param name="range">
        /// range from camera to count in
        /// </param>
        /// <param name="excludings">
        /// objects to exclude
        /// </param>
        /// <returns>
        /// All visible AttackableObjects
        /// </returns>
        public static AttackableObject[] FindVisibleAttackableObjects(
            Camera cameraUsing,
            float range,
            AttackableObject[] excludings = null
        )
        {
            //seperate from FindVisibleObjects() because using `centerPoint` instead of `transform.position`

            //in range
            AttackableObject[] componentsInRange = GetAllObjectsInRange<AttackableObject>(
                    cameraUsing.transform.position,
                    range
                )
                .Where(component => !excludings.Contains(component))
                .ToArray();

            // check obstacles...
            List<AttackableObject> result = new List<AttackableObject>();
            foreach (AttackableObject attackableObject in componentsInRange)
            {
                bool visible = HasObstacleBetween(
                    attackableObject.centerPoint,
                    cameraUsing.transform.position
                );

                bool onFrontSide =
                    Vector3.Dot(
                        attackableObject.centerPoint - cameraUsing.transform.position,
                        cameraUsing.transform.forward
                    ) > 0;

                if (visible && onFrontSide)
                {
                    result.Add(attackableObject);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Get all objects has T component in range
        /// </summary>
        /// <typeparam name="T">component you want</typeparam>
        /// <param name="centerPoint">range from whom</param>
        /// <param name="range">world coordinate</param>
        /// <returns>
        /// array of objects
        /// </returns>
        public static T[] GetAllObjectsInRange<T>(Vector3 centerPoint, float range)
        {
            // find ALL objects within range
            RaycastHit[] hits = Physics.SphereCastAll(centerPoint, range, Vector3.up, 0);

            // get only objects component T
            List<T> objectsInRange = new List<T>();
            foreach (RaycastHit hit in hits)
            {
                //if has T component...
                T targetComponent = hit.collider.GetComponent<T>();
                if (targetComponent != null)
                {
                    objectsInRange.Add(targetComponent);
                }
            }

            return objectsInRange.ToArray();
        }

        /// <summary>
        /// Check if there is an obstacle (map object) between the camera and the target.
        /// </summary>
        /// <returns>True if has obstacle</returns>
        public static bool HasObstacleBetween(Vector3 position0, Vector3 position1)
        {
            Vector3 vectorDiff = position1 - position0;

            RaycastHit[] hitsWithRay = Physics.RaycastAll(
                position0,
                vectorDiff,
                vectorDiff.magnitude
            );

            //find obstacle
            bool hasObstacle = false;
            foreach (RaycastHit hit in hitsWithRay)
            {
                if (IsMapObject(hit.collider.gameObject))
                {
                    hasObstacle = true;
                    break;
                }
            }

            //if didn't had obstacle...
            if (!hasObstacle)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
