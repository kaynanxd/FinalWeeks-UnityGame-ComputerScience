using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações")]
    public int maxHealth = 5; // Você pediu 5 hits
    private int currentHealth;
    
    [Header("Invencibilidade (Cooldown)")]
    public float iframeDuration = 1f; // Tempo invencível após ser atingido
    private bool isInvulnerable = false;
    private SpriteRenderer spriteRenderer;

    [Header("Referências")]
    public CameraFollow cameraScript;
    private PlayerRespawn respawnScript;
    private PlayerMovement movementScript; // Referência ao seu script de input
    private CharacterController2D controllerScript; // Referência ao script de física

    void Start()
    {
        currentHealth = maxHealth;
        
        // Pega as referências automaticamente
        respawnScript = GetComponent<PlayerRespawn>();
        movementScript = GetComponent<PlayerMovement>();
        controllerScript = GetComponent<CharacterController2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Tenta achar a câmera se não tiver sido arrastada
        if (cameraScript == null) cameraScript = FindObjectOfType<CameraFollow>();
    }

    public void TakeDamage(int damageAmount)
    {
        // Se estiver invulnerável ou morto, ignora
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damageAmount;
        Debug.Log($"Vida Restante: {currentHealth}");

        // 1. Treme a câmera
        if (cameraScript != null) cameraScript.ShakeCamera();

        // 2. Feedback Visual (Piscar)
        StartCoroutine(FlashSprite());

        // 3. Verifica Morte
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Morreu!");

        // 1. Trava a movimentação para o jogador não andar durante o Fade
        if (movementScript != null) movementScript.enabled = false;
        if (controllerScript != null) controllerScript.Move(0, false, false); // Para o boneco

        // 2. Chama o Respawn (que faz o FadeOut -> Teleporte -> FadeIn)
        if (respawnScript != null)
        {
            respawnScript.Respawn();
        }

        // 3. Reseta a vida e devolve o movimento (aguarda um pouco para sincronizar com o fade)
        StartCoroutine(ResetPlayerAfterRespawn());
    }

    IEnumerator ResetPlayerAfterRespawn()
    {
        // Espera o tempo do Fade Out (aproximadamente 1 segundo, ajuste conforme seu FadeController)
        yield return new WaitForSeconds(1f);

        currentHealth = maxHealth;
        
        // Devolve o controle
        if (movementScript != null) movementScript.enabled = true;
    }

    // Corrotina para ficar piscando quando toma dano
    IEnumerator FlashSprite()
    {
        isInvulnerable = true;
        
        // Pisca por um tempo
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }
}