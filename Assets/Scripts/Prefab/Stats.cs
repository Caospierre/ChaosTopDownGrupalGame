using System.Collections.Generic;
using UnityEngine;

public class HeartScalerUI : MonoBehaviour
{
    [SerializeField] private Transform heartFill; // El corazón lleno (superpuesto al vacío)
    [SerializeField] private float maxLife = 100f;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private int previousLives = -1;

    void Update()
    {
        float currentHealth = Prefab.GameSceneController.GetHealth();
        float scale = Mathf.Clamp01(currentHealth / maxLife);

        heartFill.localScale = new Vector3(scale, scale, 1f);
        int currentLives = Prefab.GameSceneController.GetLives();

        if (currentLives != previousLives)
        {
            UpdateLivesDisplay(currentLives);
            previousLives = currentLives;
        }
    }

    void UpdateLivesDisplay(int lives)
    {
        // Eliminar corazones viejos
        foreach (GameObject heart in spawnedHearts)
        {
            Destroy(heart);
        }
        spawnedHearts.Clear();

        // Instanciar corazones nuevos
        for (int i = 0; i < lives; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            spawnedHearts.Add(newHeart);
        }
    }

}

