using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace Prefab
{
    public class GameSceneController : MonoBehaviour
    {
        [Header("Referencias")]
        public GameObject baseZone;
        public GameObject monsterController;
        public GameObject player;
        public GameObject start;
        public GameObject currentWeapon;

        [Header("Configuración de Nivel")]
        [Tooltip("Índice del siguiente nivel")]
        [SerializeField] private int nextLevel = 0;

        [Tooltip("¿Este nivel es el fin del juego?")]
        [SerializeField] private bool finishGame = false;

        [Tooltip("¿Verificar destrucción de la base para Game Over?")]
        [SerializeField] private bool checkBaseDestruction = true;

        [Header("Estado del Jugador")]
        [SerializeField] private int health = 100;
        [SerializeField] private int lives = 3;
        [SerializeField] private int totalMonster = 120;
        [SerializeField] private int destroyedMonster = 0;

        [Header("Estado de la Base (solo lectura)")]
        [SerializeField] private int totalBlocks;
        [SerializeField] private int aliveBlocks;
        [SerializeField] private int destroyedBlocks;

        private bool baseDestroyedHandled = false;
        private AudioManager audioManager;

        public static GameSceneController Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            audioManager = AudioManager.Instance;
        }

        void Update()
        {
            if (checkBaseDestruction && !baseDestroyedHandled && baseZone != null)
            {
                var baseController = baseZone.GetComponent<BaseController>();

                totalBlocks = baseController.totalBlocks;
                aliveBlocks = baseController.aliveBlocks;
                destroyedBlocks = baseController.destroyedBlocks;
                
                if (aliveBlocks == 0)
                {
                    baseDestroyedHandled = true;
                    Debug.Log("💣 ¡Base destruida! Fin del juego.");
                    GameOver();
                }
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            audioManager?.PauseBackground();
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            audioManager?.ResumeBackground();
        }

        public void RespawnStartPlayer()
        {
            if (player != null && start != null)
            {
                player.transform.position = start.transform.position;
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void SetHealth(int value)
        {
            if (Instance != null)
                Instance.health = Mathf.Clamp(value, 0, 100);
        }

        public static int GetHealth()
        {
            return Instance != null ? Instance.health : 0;
        }

        public static void SetLives(int value)
        {
            if (Instance != null)
                Instance.lives = Mathf.Max(0, value);
        }

        public static int GetLives()
        {
            return Instance != null ? Instance.lives : 0;
        }

        public void SetTotalMonster(int value)
        {
            if (Instance != null)
                Instance.totalMonster = value;
        }

        public void SetDestroyedMonster()
        {
            destroyedMonster++;
            Debug.Log($"💀 Monstruo destruido. Total muertos: {destroyedMonster}");

            if (destroyedMonster >= totalMonster)
            {
                Debug.Log("🎉 Todos los monstruos han sido destruidos. ¡Ganaste!");

                if (finishGame)
                    SceneManager.LoadScene("WinGame");
                else
                    SceneManager.LoadScene(nextLevel);
            }
        }

        public void GameOver()
        {
            SceneManager.LoadScene("GameOver");
        }

        public void ReducirDurabilidadActualWeapon(int cantidad)
        {
            if (currentWeapon == null) return;

            var itemHolder = currentWeapon.GetComponent<ItemHolder>();
            if (itemHolder == null || itemHolder.item == null) return;

            float weaponHealth = itemHolder.HealthValue;
            if (weaponHealth <= 0f) return;

            weaponHealth = Mathf.Max(0f, weaponHealth - cantidad);
            itemHolder.item.health = weaponHealth.ToString("0");

            var healthTransform = currentWeapon.transform.Find("Health");
            if (healthTransform != null)
            {
                var healthTMP = healthTransform.GetComponent<TextMeshProUGUI>();
                if (healthTMP != null)
                    healthTMP.text = itemHolder.item.health;

                if (weaponHealth <= 0f)
                    healthTransform.gameObject.SetActive(false);
            }

            if (weaponHealth <= 0f)
            {
                var weaponImage = currentWeapon.GetComponent<Image>();
                if (weaponImage != null)
                    weaponImage.sprite = null;
            }
        }
    }
}
