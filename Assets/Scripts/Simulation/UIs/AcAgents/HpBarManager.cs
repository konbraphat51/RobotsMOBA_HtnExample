using System.Collections.Generic;
using System.Linq;
using Simulation.Objects;
using Simulation.Objects.AcAgent;
using Simulation.Utils;
using UnityEngine;

namespace Simulation.UIs
{
    /// <summary>
    /// Controls generation/deletion of all the HP bars
    /// </summary>
    public class HpBarManager : MonoBehaviour
    {
        [SerializeField]
        private HpBar hpBarPrefab;

        [SerializeField]
        private Canvas canvasUsing;

        [SerializeField]
        private float rangeShowing = 30f;

        private Dictionary<AttackableObject, HpBar> hpBarsShowing =
            new Dictionary<AttackableObject, HpBar>();

        private void Update()
        {
            UpdateExistence();
        }

        /// <summary>
        /// See the addition/removal of HP bars and tell to update their values
        /// </summary>
        private void UpdateExistence()
        {
            Dictionary<AttackableObject, HpBar> hpBarsShowingNew =
                new Dictionary<AttackableObject, HpBar>();

            // find all attackable objects within range
            AttackableObject myAgent = GetComponentInParent<AcAgentCore>();
            HashSet<AttackableObject> attackableObjects = UtilityFunctions
                .FindVisibleAttackableObjects(
                    canvasUsing.worldCamera,
                    rangeShowing,
                    new AttackableObject[] { myAgent }
                )
                .ToHashSet();

            //update HP bars for this frame
            foreach (AttackableObject attackableObject in attackableObjects)
            {
                // if the object is not seen...
                if (!hpBarsShowing.ContainsKey(attackableObject))
                {
                    //...generate new HP bar
                    HpBar hpBar = Instantiate(hpBarPrefab, transform); //same parent
                    hpBar.Initailize(canvasUsing, attackableObject);

                    //register to dictionary
                    hpBarsShowingNew.Add(attackableObject, hpBar);
                }
                //continuely registered ones
                else
                {
                    hpBarsShowingNew.Add(attackableObject, hpBarsShowing[attackableObject]);
                }
            }

            //find HP bars no longer needed
            foreach (AttackableObject attackableObject in hpBarsShowing.Keys)
            {
                // if the object is no longer in range...
                if (!attackableObjects.Contains(attackableObject))
                {
                    //...delete HP bar
                    Destroy(hpBarsShowing[attackableObject].gameObject);
                }
            }

            //update list
            hpBarsShowing = hpBarsShowingNew;
        }
    }
}
