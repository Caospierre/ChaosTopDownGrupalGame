using UnityEngine;

namespace Prefab
{
    public class WallBlock : MonoBehaviour
    {
        [Tooltip("Vida máxima del bloque")]
        public int maxHealth = 100;

        [Tooltip("Vida Actual")]
        public int currentHealth;

        private TextMesh warningText;
        private float warningTimer = 0f;

        void Start()
        {
            currentHealth = maxHealth;

            Transform warningObj = transform.Find("WarningText");
            if (warningObj != null)
            {
               // Debug.Log($"[Wall] get warning: {warningObj.name}");

                warningText = warningObj.GetComponent<TextMesh>();
                warningText.gameObject.SetActive(false);
            }
            else
            {
//                Debug.LogWarning($"[Wall] Error dont found warning");
            }
        }

        void Update()
        {
            if (warningTimer > 0f)
            {
                warningTimer -= Time.deltaTime;
                if (warningTimer <= 0f && currentHealth > 10 && warningText != null)
                {
                    warningText.gameObject.SetActive(false);
                }
            }
        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }

        public bool TakeDamage(int amount)
        {
            currentHealth -= amount;

            if (warningText != null)
            {
                //Debug.Log($"[Wall] current health:  {currentHealth}.");

                if (currentHealth <= 10 && currentHealth > 0)
                {
                    //Debug.Log($"[Wall] holi");

                    warningText.text = currentHealth <= 3
                        ? $"¡Colapsando! Vida: {currentHealth}"
                        : $"¡Dañado! Vida: {currentHealth}";

                    warningText.color = Color.red;
                    warningText.gameObject.SetActive(true);
                    warningTimer = 1f;

                }
                else
                {
                   // Debug.Log($"[Wall] boli");

                    warningText.text = $"¡Deterioro! -{amount}";
                    warningText.color = Color.red;
                    warningText.gameObject.SetActive(true);
                    warningTimer = 5f;
                }
            }

            if (currentHealth <= 0)
            {
                gameObject.SetActive(false); // No se destruye, se desactiva
                return true;
            }

            return false;
        }

        public void Rebuild()
        {
            currentHealth = maxHealth;
            gameObject.SetActive(true);

            if (warningText != null)
            {
                warningText.gameObject.SetActive(false);
            }

            //Debug.Log($"[Wall] Rebuilt with {currentHealth} health.");
        }
    }
}
