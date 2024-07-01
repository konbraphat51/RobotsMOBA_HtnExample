using Simulation.Objects.AcAgent;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UIs
{
    /// <summary>
    /// Bar UI that shows thruster left
    /// </summary>
    public class ThrusterBar : MonoBehaviour
    {
        [SerializeField]
        private AcAgentCore target;

        private void Update()
        {
            Image image = GetComponent<Image>();

            image.fillAmount = target.thruster / target.thrusterMax;
        }
    }
}
