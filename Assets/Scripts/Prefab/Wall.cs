using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class Wall : MonoBehaviour
    {
        public int tam = 1;
        public GameObject blockPrefab;

        [Tooltip("Si está activado, la muralla se construye en vertical")]
        public bool vertical = false;

        [Header("Configuración de daño")]
        [Tooltip("Daño mínimo que puede recibir un bloque")]
        public int minDamage = 21;

        [Tooltip("Daño máximo que puede recibir un bloque")]
        public int maxDamage = 30;

        [Header("Intervalo de daño (en segundos)")]
        [Tooltip("Tiempo mínimo entre daños")]
        public float minInterval = 3f;

        [Tooltip("Tiempo máximo entre daños")]
        public float maxInterval = 5f;

        private List<WallBlock> blocks = new();

        void Start()
        {
            for (int i = 0; i < tam; i++)
            {
                Vector3 offset = vertical
                    ? new Vector3(0f, -i, 0f)
                    : new Vector3(i, 0f, 0f);

                Vector3 pos = transform.position + offset;
                GameObject blockObj = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blockObj.name = $"Block_{i}";

                WallBlock wb = blockObj.GetComponent<WallBlock>();
                if (wb != null)
                {
                    blocks.Add(wb);
                }
            }

            StartCoroutine(DamageLoop());
        }

        IEnumerator DamageLoop()
        {
            while (true)
            {
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                List<WallBlock> aliveBlocks = blocks.FindAll(b => b != null && b.IsAlive());
                if (aliveBlocks.Count == 0) yield break;

                WallBlock target = aliveBlocks[Random.Range(0, aliveBlocks.Count)];
                if (target != null)
                {
                    int damage = Random.Range(minDamage, maxDamage + 1);
                    target.TakeDamage(damage);
                }
            }
        }

        public void RebuildAll()
        {
            foreach (WallBlock block in blocks)
            {
                if (block != null && !block.IsAlive())
                {
                    block.Rebuild();
                }
            }
        }

        public List<WallBlock> GetBlocks()
        {
            return blocks;
        }
    }
}
