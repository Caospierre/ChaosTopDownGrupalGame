using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class Wall : MonoBehaviour
    {
        public int tam = 1;
        public GameObject blockPrefab;
        public bool vertical = false;

        [Header("Configuración de daño")]
        public int minDamage = 21;
        public int maxDamage = 30;

        [Header("Intervalo de daño (en segundos)")]
        public float minInterval = 3f;
        public float maxInterval = 5f;

        private List<WallBlock> blocks = new();
        private AreaDetector areaDetector;

        void Start()
        {
            for (int i = 0; i < tam; i++)
            {
                Vector3 offset = vertical ? new Vector3(0f, -i, 0f) : new Vector3(i, 0f, 0f);
                Vector3 pos = transform.position + offset;
                GameObject blockObj = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                blockObj.name = $"Block_{i}";

                WallBlock wb = blockObj.GetComponent<WallBlock>();
                if (wb != null)
                    blocks.Add(wb);
            }

            areaDetector = GetComponentInChildren<AreaDetector>();
            if (areaDetector != null)
            {
                Debug.Log($"🧱 Registrando eventos de detección para pared: {gameObject.name}");

                areaDetector.OnEnter += HandlePlayerEnter;
                areaDetector.OnExit += HandlePlayerExit;
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
                    block.Rebuild();
            }
        }

        public List<WallBlock> GetBlocks() => blocks;

        private void HandlePlayerEnter(GameObject other)
        {
            Debug.Log($"🧍‍♂️ {other.name} entró en el detector de {gameObject.name}");

            var weaponGO = GameSceneController.Instance.currentWeapon;
            var itemHolder = weaponGO?.GetComponent<ItemHolder>();
            var item = itemHolder?.item;

            if (item == null || !item.isWall)
                return;

            bool hayBloquesRotos = blocks.Exists(b => b != null && !b.IsAlive());
            if (hayBloquesRotos)
            {
                Debug.Log($"🧱 Reparando muro en {gameObject.name} con herramienta 'Wall'");
                RebuildAll();
            }
        }

        private void HandlePlayerExit(GameObject other)
        {
            Debug.Log($"🚪 {other.name} salió del detector de {gameObject.name}");
        }
    }
}
