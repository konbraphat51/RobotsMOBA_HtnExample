using System.Collections.Generic;
using Simulation.Objects;
using Simulation.Objects.Stragetic;
using Simulation.Utils;
using UnityEngine;

namespace Simulation
{
    /// <summary>
    /// Controls the flow of the game
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public List<MainBase> mainBases { get; private set; } = new List<MainBase>();
        public bool gameSet { get; private set; } = false;

        private void Start()
        {
            RegisterBases();
        }

        private void Update()
        {
            DetectGameSet();
        }

        /// <summary>
        /// Get all bases in the children object, and register them with callbacks
        /// </summary>
        private void RegisterBases()
        {
            //reset
            ClearBasesData();

            //find
            MainBase[] bases = GetComponentsInChildren<MainBase>();

            //register callbacks
            foreach (MainBase mainBase in bases)
            {
                mainBase.ListenToDeath(OnBaseDestroyed);
            }

            mainBases.AddRange(bases);
        }

        /// <summary>
        /// Clear callbacks before clearing the list
        /// </summary>
        private void ClearBasesData()
        {
            foreach (MainBase mainBase in mainBases)
            {
                mainBase.StopListeningToDeath(OnBaseDestroyed);
            }

            mainBases.Clear();
        }

        /// <summary>
        /// Callback when base destroyed
        /// </summary>
        /// <param name="mainBaseObject">
        /// Destroyed base
        /// </param>
        private void OnBaseDestroyed(AttackableObject mainBaseObject)
        {
            MainBase mainBase = mainBaseObject as MainBase;

            mainBases.Remove(mainBase);
        }

        /// <summary>
        /// detect the game set rule has been met or not
        /// </summary>
        private void DetectGameSet()
        {
            //if already game set...
            if (gameSet)
            {
                //...skip
                return;
            }

            //if there is only one main base left...
            if (mainBases.Count == 1)
            {
                //...game set
                gameSet = true;
                FinishGame(mainBases[0].team);
            }
        }

        /// <summary>
        /// Finish the game
        /// </summary>
        /// <param name="winner"></param>
        private void FinishGame(Team winner)
        {
            //FIXME
            Debug.Log("Game set! Winner is " + winner);
        }
    }
}
