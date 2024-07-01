using System;
using Simulation.Objects.Stragetic;
using UnityEngine;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Control AcAgent by key & mouse
    /// </summary>
    public class KeyMouseController : Controller
    {
        public enum Actions
        {
            Forward,
            Backward,
            Left,
            Right,
            Jump,
            Attack,
            Die
        }

        [Serializable]
        public struct KeyMousePair
        {
            public KeyCode key;
            public Actions action;
        }

        [SerializeField]
        private KeyMousePair[] keyMousePairs;

        [SerializeField]
        private float mouseSensitivityX = 10f;

        [SerializeField]
        private float mouseSensitivityY = 5f;

        protected override void Start()
        {
            base.Start();

            LockCursor();
        }

        private void Update()
        {
            ReceiveKeys();
            ReceiveMouse();
        }

        /// <summary>
        /// Select where to spawn
        /// </summary>
        /// <param name="spawnPointsAvailable">
        /// All the spawn points available
        /// </param>
        /// <returns>
        /// Selected spawn point. Null if not selected yet
        /// </returns>
        public override SpawnPoint SelectSpawnPoint(SpawnPoint[] spawnPointsAvailable)
        {
            //TODO: better spawn point selection

            Debug.Assert(spawnPointsAvailable.Length > 0);
            Debug.Assert(spawnPointsAvailable.Length < 9);

            //get number key
            for (int cnt = 1; cnt < spawnPointsAvailable.Length + 1; cnt++)
            {
                if (Input.GetKeyDown(cnt.ToString()))
                {
                    //if first pressed...
                    if (spawnPointSelected != spawnPointsAvailable[cnt - 1])
                    {
                        //...show to UI
                        spawnPointSelected = spawnPointsAvailable[cnt - 1];
                        return null;
                    }
                    else
                    {
                        //...select
                        return spawnPointsAvailable[cnt - 1];
                    }
                }
            }

            return null;
        }

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ReceiveKeys()
        {
            //object movement direction vector
            //this don't have to be normalized
            Vector3 moveVector = Vector3.zero;

            foreach (KeyMousePair pair in keyMousePairs)
            {
                if (Input.GetKey(pair.key))
                {
                    switch (pair.action)
                    {
                        case Actions.Forward:
                            //forward
                            moveVector += new Vector3(1, 0, 0);
                            break;
                        case Actions.Backward:
                            //backward
                            moveVector += new Vector3(-1, 0, 0);
                            break;
                        case Actions.Left:
                            //left
                            moveVector += new Vector3(0, 0, 1);
                            break;
                        case Actions.Right:
                            //right
                            moveVector += new Vector3(0, 0, -1);
                            break;
                        case Actions.Jump:
                            //jump
                            targetControlling.JumpOrHover();
                            break;
                        case Actions.Attack:
                            //attack
                            targetControlling.Attack();
                            break;
                        case Actions.Die:
                            //die
                            targetControlling.Die();
                            break;
                    }
                }
            }

            //if should move...
            if (moveVector != Vector3.zero)
            {
                //...move object
                //full speed, direction from camera
                targetControlling.Move(moveVector, 1f, true);
            }
        }

        private void ReceiveMouse()
        {
            //camera rotation
            float cameraRotationHorizontal = -Input.GetAxis("Mouse X") * mouseSensitivityX;
            float cameraRotationVertical = Input.GetAxis("Mouse Y") * mouseSensitivityY;

            targetControlling.RotateCameraHorizontal(cameraRotationHorizontal);
            targetControlling.RotateCameraVertical(cameraRotationVertical);
        }
    }
}
