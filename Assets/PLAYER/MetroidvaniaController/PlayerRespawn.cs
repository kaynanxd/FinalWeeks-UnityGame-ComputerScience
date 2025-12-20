using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Configuração")]
    private Vector3 currentCheckpoint; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentCheckpoint = transform.position;
        
        // 1. Assina o evento do FadeController. Quando o Fade Out acabar, a função TeleportAndFadeIn será chamada.
        FadeController.OnFadeOutComplete += TeleportAndFadeIn; 
    }
    
    // Certifique-se de remover o listener quando o objeto for destruído!
    void OnDestroy()
    {
        FadeController.OnFadeOutComplete -= TeleportAndFadeIn;
    }

    // Chamado quando o jogador cai ou morre (AGORA SÓ CHAMA O FADE)
    public void Respawn()
    {
        // Se o FadeController existir na cena, INICIA o processo de Fade Out
        if (FadeController.Instance != null)
        {
            // Começa o escurecimento. O teleport vai acontecer depois que escurecer
            FadeController.Instance.StartRespawnFade();
        }
        else
        {
            // Caso não encontre o FadeController, faz o respawn instantâneo (fallback)
            ExecuteRespawnLogic();
        }
    }
    
    // Esta função será chamada pelo FadeController DEPOIS que a tela estiver preta
    private void TeleportAndFadeIn()
    {
        ExecuteRespawnLogic(); // Faz o teleport e zera a velocidade
        
        // Começa o clareamento
        if (FadeController.Instance != null)
        {
            FadeController.Instance.StartFadeIn();
        }
    }
    
    // Lógica principal de Respawn
    private void ExecuteRespawnLogic()
    {
        // 1. Teleporta para o último checkpoint salvo
        transform.position = currentCheckpoint;

        // 2. Zera a velocidade 
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.angularVelocity = 0f; // Zera rotação, se for o caso
        }
        
        // Opcional: Reativar o jogador se ele foi desativado/escondido na morte
        gameObject.SetActive(true);

        Debug.Log("Jogador Renasceu e Teleportou!");
    }

    // Chamado quando encosta numa bandeira/checkpoint
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        currentCheckpoint = newPosition;
        Debug.Log("Checkpoint Salvo!");
    }
}