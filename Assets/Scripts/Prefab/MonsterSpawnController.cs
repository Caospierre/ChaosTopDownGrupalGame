using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class MonsterSpawnController : MonoBehaviour
    {
        [Tooltip("Tiempo entre cada intento de spawn global")]
        public float spawnCheckInterval = 5f;

        [Tooltip("Total de monstruos generados en esta partida")]
        public int totalSpawnedMonsters = 0;

        [Tooltip("Cantidad máxima de monstruos que se pueden generar entre todos los puntos")]
        public int totalPossibleMonsters = 0;

        
        [Tooltip("Cantidad  de monstruos destruidos")]
        public int totalDeadMonsters = 0;

        private List<MonsterSpawnPoint> spawnPoints = new();

        void Start()
        {
            spawnPoints.AddRange(FindObjectsOfType<MonsterSpawnPoint>());

            foreach (var sp in spawnPoints)
            {
                sp.controller = this;
                totalPossibleMonsters += sp.GetMaxMonsters();
                GameSceneController.Instance.SetTotalMonster(totalPossibleMonsters);

            }

            StartCoroutine(SpawnLoop());
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnCheckInterval);

                if (spawnPoints.Count == 0) yield break;

                MonsterSpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                point.TrySpawn();
            }
        }

        public void RegisterSpawn()
        {
            totalSpawnedMonsters++;
        }

        public int GetTotalSpawnedMonsters()
        {
            return totalSpawnedMonsters;
        }
        
        public int GetMonsterForDestroy()
        {
            return totalSpawnedMonsters-totalDeadMonsters;
        }
        public void RegisterDeath()
        {
            totalDeadMonsters++;
        }
    }
}