using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation
{
    /// <summary>
    /// Controls entire system
    /// </summary>
    public abstract class EntireManager : MonoBehaviour
    {
        [SerializeField]
        private GameManager gamePrefab;

        private List<GameManager> games = new List<GameManager>();

        protected virtual void Start()
        {
            games = GetComponentsInChildren<GameManager>().ToList();
        }

        protected virtual void Update()
        {
            DetectGameSet();
        }

        private void DetectGameSet()
        {
            //get all games that are set
            List<GameManager> gamesToSet = new List<GameManager>();
            foreach (GameManager game in games)
            {
                if (game.gameSet)
                {
                    gamesToSet.Add(game);
                }
            }

            //process them
            foreach (GameManager game in gamesToSet)
            {
                OnGameSet(game);
            }
        }

        protected virtual void OnGameSet(GameManager game)
        {
            Vector3 position = game.transform.position;

            //erace
            Destroy(game.gameObject);
            games.Remove(game);

            //create new
            GameManager newGame = Instantiate(gamePrefab, position, Quaternion.identity, transform);
            games.Add(newGame);
        }
    }
}
