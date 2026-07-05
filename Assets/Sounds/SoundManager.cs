using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Lista de audios")]
    public List<AudioClip> audios = new List<AudioClip>();

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float volumen = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volumen;
    }

    void Start()
    {
        ReproducirAudioAleatorio();
    }

    void ReproducirAudioAleatorio()
    {
        if (audios == null || audios.Count == 0)
        {
            Debug.LogWarning("SoundManager: no hay audios asignados en la lista.");
            return;
        }

        int indice = Random.Range(0, audios.Count);
        AudioClip clipElegido = audios[indice];

        audioSource.clip = clipElegido;
        audioSource.Play();
    }
}