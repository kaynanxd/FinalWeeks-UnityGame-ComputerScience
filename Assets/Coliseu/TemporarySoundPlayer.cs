using UnityEngine;

public class TemporarySoundPlayer : MonoBehaviour
{
    public AudioClip soundClip;
    public float destroyTime = 2f; // Destroi o objeto depois de 2 segundos
    
    void Start()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.clip = soundClip;
        audioSource.playOnAwake = false; // Garante que o som só toque quando mandarmos
        audioSource.spatialBlend = 0f; // 2D Sound (na tela)
        
        audioSource.Play();
        
        // Destrói o objeto depois que o som terminar ou após o tempo definido
        Destroy(gameObject, destroyTime); 
    }
}