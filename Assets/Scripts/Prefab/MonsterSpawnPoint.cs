using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class MonsterSpawnPoint : MonoBehaviour
    {
        [Tooltip("Prefab del monstruo a instanciar")]
        public GameObject monsterPrefab;

        [Tooltip("Máximo de monstruos activos en este punto")]
        public int maxMonsters = 10;

        [Tooltip("Intervalo de tiempo (segundos) entre spawns")]
        public float spawnCooldown = 10f;

        [HideInInspector]
        public MonsterSpawnController controller;

        private List<GameObject> activeMonsters = new();
        private float lastSpawnTime = -Mathf.Infinity;

        public bool TrySpawn()
        {
            Cleanup();

            if (activeMonsters.Count >= maxMonsters)
                return false;

            if (Time.time - lastSpawnTime < spawnCooldown)
                return false;

            GameObject monster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            activeMonsters.Add(monster);
            lastSpawnTime = Time.time;

            controller?.RegisterSpawn();

            return true;
        }

        private void Cleanup()
        {
            for (int i = activeMonsters.Count - 1; i >= 0; i--)
            {
                if (activeMonsters[i] == null)
                {
                    activeMonsters.RemoveAt(i);
                    controller?.RegisterDeath();
                }
            }
        }
        
        public int GetMaxMonsters()
        {
            return maxMonsters;
        }
    }
}