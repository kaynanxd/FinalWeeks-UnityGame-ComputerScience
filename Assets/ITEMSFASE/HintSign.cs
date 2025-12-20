using UnityEngine;

public class HintSign : MonoBehaviour
{
    [Header("Configuração")]
    public HintData data;           // Arraste o arquivo da dica aqui
    public GameObject interactPrompt; // O balãozinho/botão que aparece em cima da placa
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        // Garante que o balão comece invisível
        if(interactPrompt) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Só verifica input se o jogador estiver perto e o jogo não estiver pausado
        if (playerInRange && Time.timeScale > 0)
        {
            if (Input.GetKeyDown(interactKey))
            {
                // Abre a dica
                HintManager.Instance.ShowHint(data);
                
                // Opcional: Esconder o prompt enquanto lê
                if(interactPrompt) interactPrompt.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Mostra o botão flutuante
            if(interactPrompt) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Esconde o botão flutuante
            if(interactPrompt) interactPrompt.SetActive(false);
        }
    }
}