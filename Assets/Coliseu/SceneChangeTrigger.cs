using UnityEngine;
using UnityEngine.SceneManagement; // ESSENCIAL para carregar cenas

public class SceneChangeTrigger : MonoBehaviour
{
    [Header("Configuração da Cena")]
    // O nome da cena que deve ser carregada
    public string targetSceneName = "NomeDaProximaFase"; 

    [Header("Interação")]
    public GameObject interactPrompt; // O balãozinho/botão que aparece (ex: "Aperte E")
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        // Garante que o prompt comece invisível
        if(interactPrompt) interactPrompt.SetActive(false);
        
        // Dica: Se o prompt for uma UI Canvas, o .SetActive(false) é mais eficaz.
    }

    void Update()
    {
        // Verifica o input apenas se o jogador estiver na área
        if (playerInRange)
        {
            if (Input.GetKeyDown(interactKey))
            {
                LoadNewScene();
            }
        }
    }

    // Chamado quando o jogador entra no Collider de Gatilho
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Mostra o botão flutuante
            if(interactPrompt) interactPrompt.SetActive(true);
        }
    }

    // Chamado quando o jogador sai do Collider de Gatilho
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Esconde o botão flutuante
            if(interactPrompt) interactPrompt.SetActive(false);
        }
    }
    
    // Função principal para carregar a cena
    void LoadNewScene()
    {
        // Verifica se o nome da cena foi preenchido
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("ERRO: O nome da cena de destino está vazio no SceneChangeTrigger!");
            return;
        }

        Debug.Log($"Carregando cena: {targetSceneName}");
        
        // Carrega a cena pelo nome
        SceneManager.LoadScene(targetSceneName);
        
        // Opcional: Para evitar que o prompt seja reexibido
        playerInRange = false;
        if(interactPrompt) interactPrompt.SetActive(false);
    }
}