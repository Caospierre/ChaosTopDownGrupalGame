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
        public GameObject currentWeapon;

        [Header("Estado del Jugador")]
        [SerializeField] private int health = 100;
        [SerializeField] private int lives = 3;

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

        public static void ApplyDamage(int amount)
        {
            if (Instance == null) return;

            Instance.health -= amount;

            if (Instance.health <= 0)
            {
                Instance.lives--;

                if (Instance.lives > 0)
                {
                    Instance.health = 100;
                }
                else
                {
                    Instance.health = 0;
                    Instance.GameOver();
                }
            }
        }

        public void GameOver()
        {
            Debug.Log("Game Over");
            PauseGame();
        }

        public void ReducirDurabilidadActualWeapon(int cantidad)
        {
            if (currentWeapon == null)
            {
                Debug.LogWarning("⛔ currentWeapon es null.");
                return;
            }

            var itemHolder = currentWeapon.GetComponent<ItemHolder>();
            if (itemHolder == null || itemHolder.item == null)
            {
                Debug.LogWarning("⛔ itemHolder o item nulo en currentWeapon.");
                return;
            }

            float weaponHealth = itemHolder.HealthValue;
            if (weaponHealth <= 0f)
            {
                Debug.Log("🔁 Arma ya está rota, no se reduce más.");
                return;
            }

            weaponHealth = Mathf.Max(0f, weaponHealth - cantidad);
            itemHolder.item.health = weaponHealth.ToString("0");

            Debug.Log($"🔻 Durabilidad del arma reducida en {cantidad}, nueva salud: {weaponHealth}");

            Transform healthTransform = currentWeapon.transform.Find("Health");
            if (healthTransform != null)
            {
                var healthTMP = healthTransform.GetComponent<TextMeshProUGUI>();
                if (healthTMP != null)
                {
                    healthTMP.text = itemHolder.item.health;
                    Debug.Log("🔄 Texto de durabilidad actualizado.");
                }

                if (weaponHealth <= 0f)
                {
                    healthTransform.gameObject.SetActive(false);
                    Debug.Log("⚠️ Texto de salud ocultado por durabilidad 0.");
                }
            }

            if (weaponHealth <= 0f)
            {
                var weaponImage = currentWeapon.GetComponent<Image>();
                if (weaponImage != null)
                    weaponImage.sprite = null;

                Debug.Log("⚔️ El arma se rompió y fue visualmente ocultada.");
            }
        }
    }
}
