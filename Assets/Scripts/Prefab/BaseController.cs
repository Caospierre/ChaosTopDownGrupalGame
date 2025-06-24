using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prefab
{
    public class BaseController : MonoBehaviour
    {
        [Header("Estado de la base (solo lectura)")]
        public int totalBlocks;
        public int aliveBlocks;
        public int destroyedBlocks;

        [SerializeField] private AreaDetector monsterDetector;
        [SerializeField] private AreaDetector playerDetector;
        [SerializeField] private AreaDetector safeAreaDetector;

        private List<Wall> walls = new();

        void Start()
        {
            // Suscripciones a detectores
            if (monsterDetector != null)
            {
                monsterDetector.OnEnter += OnMonsterEnter;
                monsterDetector.OnExit += OnMonsterExit;
            }

            if (playerDetector != null)
            {
                playerDetector.OnEnter += OnPlayerEnter;
                playerDetector.OnExit += OnPlayerExit;
            }

            if (safeAreaDetector != null)
            {
                safeAreaDetector.OnEnter += OnSafeAreaEnter;
                safeAreaDetector.OnExit += OnSafeAreaExit;
            }

            walls.AddRange(FindObjectsOfType<Wall>());
            StartCoroutine(DelayRecalcularTotalBlocks());
        }

        IEnumerator DelayRecalcularTotalBlocks()
        {
            yield return null;
            RecalcularTotalBlocks();
        }

        void Update()
        {
            int vivos = 0;

            foreach (Wall wall in walls)
            {
                if (wall == null) continue;

                foreach (var block in wall.GetBlocks())
                {
                    if (block != null && block.IsAlive())
                        vivos++;
                }
            }

            aliveBlocks = vivos;
            destroyedBlocks = totalBlocks - aliveBlocks;

            if (aliveBlocks == 0)
            {
                Debug.Log("💥 Todos los bloques de la base han sido destruidos.");
            }
        }

        private void RecalcularTotalBlocks()
        {
            totalBlocks = 0;

            foreach (Wall wall in walls)
            {
                if (wall == null) continue;
                totalBlocks += wall.GetBlocks().Count;
            }
        }

        private void OnMonsterEnter(GameObject monster)
        {
            Debug.Log($"🛑 Monstruo entró a la base: {monster.name}");
        }

        private void OnMonsterExit(GameObject monster)
        {
            Debug.Log($"✅ Monstruo salió de la base: {monster.name}");
        }

        private void OnPlayerEnter(GameObject player)
        {
            Debug.Log($"🟢 Jugador entró a la base: {player.name}");
        }

        private void OnPlayerExit(GameObject player)
        {
            Debug.Log($"🔵 Jugador salió de la base: {player.name}");
        }

        private void OnSafeAreaEnter(GameObject player)
        {
            Debug.Log($"❤️ Entró a zona segura: {player.name}");
        }

        private void OnSafeAreaExit(GameObject player)
        {
            Debug.Log($"💨 Salió de zona segura: {player.name}");
        }
    }
}
