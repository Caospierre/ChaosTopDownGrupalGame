using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }
    public Vector3 LastSavedPosition { get; private set; } = new Vector3(0.5f, 0.5f, 0);
    public Vector2 LastSavedRotation { get; private set; } = new Vector2(0, -1);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNewScene(int targetSceneIndex, Vector3 newPlayerPosition, Vector2 newPlayerRotation)
    {
        LastSavedPosition = newPlayerPosition;
        LastSavedRotation = newPlayerRotation;
        SceneManager.LoadScene(targetSceneIndex);
    }
}
