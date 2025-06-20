using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class MonsterSpawnController : MonoBehaviour
    {
        [Tooltip("Tiempo entre cada intento de spawn global")]
        public float spawnCheckInterval = 5f;

        private List<MonsterSpawnPoint> spawnPoints = new();

        void Start()
        {
            spawnPoints.AddRange(FindObjectsOfType<MonsterSpawnPoint>());
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
    }
}