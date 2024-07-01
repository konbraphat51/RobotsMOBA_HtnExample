using Simulation.Objects;
using Simulation.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UIs
{
    /// <summary>
    /// Show HP bar for Attackable Object
    /// </summary>
    public class HpBar : MonoBehaviour
    {
        [SerializeField]
        private Vector2 offsetOnScreen;

        private Canvas canvasUsing;
        private AttackableObject target;

        private void Update()
        {
            UpdatePosition();
            UpdateHp();
        }

        /// <summary>
        /// Call this when generating
        /// </summary>
        /// <param name="target">object to show on</param>
        public void Initailize(Canvas canvasUsing, AttackableObject target)
        {
            this.canvasUsing = canvasUsing;
            this.target = target;
        }

        private void UpdatePosition()
        {
            transform.position = UtilityFunctions.ComputeCanvasPosition(
                target.centerPoint,
                canvasUsing,
                offsetOnScreen
            );
        }

        /// <summary>
        /// update HP indication
        /// </summary>
        private void UpdateHp()
        {
            Image image = GetComponent<Image>();

            //get ratio of HP
            float ratio = Mathf.Clamp(target.hp / target.hpMax, 0f, 1f);

            //set viz
            image.fillAmount = ratio;
        }
    }
}
