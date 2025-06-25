using UnityEngine;

namespace Prefab
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Clips")]
        public AudioClip backgroundClip;
        public AudioClip effectClip;

        [Header("Sources")]
        public AudioSource musicSource;
        public AudioSource effectSource;

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

            if (musicSource != null)
            {
                musicSource.loop = true;
                musicSource.clip = backgroundClip;
                musicSource.volume = PlayerPrefs.GetFloat("volumenSave", 0.5f); // Cargar volumen guardado
            }

            if (effectSource != null)
            {
                effectSource.clip = effectClip;
                effectSource.volume = PlayerPrefs.GetFloat("sonidoSave", 0.5f); // Cargar volumen guardado
            }
        }

        void Start()
        {
            PlayBackground();
        }

        public void PlayBackground()
        {
            if (musicSource != null && backgroundClip != null)
                musicSource.Play();
        }

        public void PauseBackground() => musicSource?.Pause();
        public void ResumeBackground() => musicSource?.UnPause();
        public void StopBackground() => musicSource?.Stop();

        public void PlayEffect(AudioClip clip)
        {
            if (effectSource != null && clip != null)
                effectSource.PlayOneShot(clip);
        }

        public float BackgroundVolume
        {
            get => musicSource != null ? musicSource.volume : 0f;
            set
            {
                if (musicSource != null)
                {
                    musicSource.volume = Mathf.Clamp01(value);
                    PlayerPrefs.SetFloat("volumenSave", musicSource.volume);
                }
            }
        }

        public float EffectVolume
        {
            get => effectSource != null ? effectSource.volume : 0f;
            set
            {
                if (effectSource != null)
                {
                    effectSource.volume = Mathf.Clamp01(value);
                    PlayerPrefs.SetFloat("sonidoSave", effectSource.volume);
                }
            }
        }
    }
}