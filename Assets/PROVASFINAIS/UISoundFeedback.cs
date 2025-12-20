using UnityEngine;
using UnityEngine.EventSystems; // Necessário para detectar o mouse
using UnityEngine.UI;

public class UISoundFeedback : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [Header("Configurações de Áudio")]
    [Tooltip("Arraste o objeto que tem o AudioSource aqui. Se deixar vazio, ele tenta pegar do próprio objeto.")]
    public AudioSource audioSource;

    [Header("Clipes de Som")]
    public AudioClip hoverSound; // Som ao passar o mouse
    public AudioClip clickSound; // Som ao clicar

    private void Start()
    {
        // Se não definirmos um AudioSource manualmente, tenta pegar um do componente
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Detecta quando o mouse entra na área do botão/toggle
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    // Detecta quando o botão/toggle é pressionado
    public void OnPointerDown(PointerEventData eventData)
    {
        PlaySound(clickSound);
    }

    private void PlaySound(AudioClip clip)
    {
        // Só toca se houver um clip e um AudioSource atribuído
        if (clip != null && audioSource != null)
        {
            // PlayOneShot é ideal para UI pois permite sobreposição de sons sem cortar o anterior bruscamente
            audioSource.PlayOneShot(clip);
        }
    }
}