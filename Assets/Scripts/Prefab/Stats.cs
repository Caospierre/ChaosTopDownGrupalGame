using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private Transform heartFill;
    [SerializeField] private float maxLife = 100f;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;
    private List<GameObject> spawnedHearts = new List<GameObject>();
    private int previousLives = -1;
    [SerializeField] private TextMeshProUGUI texto1;
    [SerializeField] private TextMeshProUGUI texto2;

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

        // Actualizar bloques en pantalla
        int bloquesVivos = Prefab.GameSceneController.GetAliveBlocks();
        texto1.text = $"Bloques: {bloquesVivos}";

        //Actualizar enemigos en pantalla
        int total = Prefab.GameSceneController.GetTotalMonsters();
        int muertos = Prefab.GameSceneController.GetDestroyedMonsters();
        int restantes = total - muertos;
        texto2.text = $"Enemigos restantes: {restantes}";
    }

    void UpdateLivesDisplay(int lives)
    {
        foreach (GameObject heart in spawnedHearts)
        {
            Destroy(heart);
        }
        spawnedHearts.Clear();

        for (int i = 0; i < lives; i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartContainer);
            spawnedHearts.Add(newHeart);
        }
    }
}