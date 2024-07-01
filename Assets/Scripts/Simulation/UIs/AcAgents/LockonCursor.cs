using Simulation.Utils;
using UnityEngine;

namespace Simulation.UIs
{
    /// <summary>
    /// Lockon cursor UI
    /// </summary>
    public class LockonCursor : MonoBehaviour
    {
        private bool initialized = false;
        private Camera cameraUsing;
        private Canvas canvasUsing;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Set cursor position
        /// </summary>
        public void SetCursor(Vector3 position)
        {
            if (!initialized)
            {
                Initialize();
            }

            gameObject.SetActive(true);

            SetPosition(position);
        }

        /// <summary>
        /// Hide cursor
        /// </summary>
        public void HideCursor()
        {
            gameObject.SetActive(false);
        }

        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            gameObject.SetActive(false);

            canvasUsing = GetComponentInParent<Canvas>();
            cameraUsing = canvasUsing.worldCamera;
        }

        private void SetPosition(Vector3 position)
        {
            transform.position = UtilityFunctions.ComputeCanvasPosition(
                position,
                canvasUsing,
                Vector2.zero
            );
        }
    }
}
