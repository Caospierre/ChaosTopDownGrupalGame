using Prefab;
using UnityEngine;
using UnityEngine.UI;

public class ControlVolumen : MonoBehaviour
{
    [SerializeField] private Slider controlVolumen;
    [SerializeField] private Slider controlSonido;

    void Start()
    {
        // Cargar valores iniciales
        controlVolumen.value = PlayerPrefs.GetFloat("volumenSave", 0.5f);
        controlSonido.value = PlayerPrefs.GetFloat("sonidoSave", 0.5f);

        // Asignar listeners
        controlVolumen.onValueChanged.AddListener(OnBackgroundVolumeChanged);
        controlSonido.onValueChanged.AddListener(OnEffectVolumeChanged);
    }

    void OnBackgroundVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.BackgroundVolume = value;
    }

    void OnEffectVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.EffectVolume = value;
        }
    }
}
