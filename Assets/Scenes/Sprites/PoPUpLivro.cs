using UnityEngine;

public class PoPUpLivro : MonoBehaviour
{
    [Header("UI")]
    public GameObject balaoFala; // O elemento que será ativado/desativado
    public Canvas textoDialogo;  // Se tiver outro elemento que deseja mostrar

    private void Start()
    {
        // Garantir que os elementos começam invisíveis
        balaoFala.SetActive(false);
        textoDialogo.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Ativar elementos quando o player entra no trigger
            balaoFala.SetActive(true);
            textoDialogo.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Desativar elementos quando o player sai do trigger
            balaoFala.SetActive(false);
            textoDialogo.gameObject.SetActive(false);
        }
    }
}
