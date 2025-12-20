using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false; // Para não salvar o mesmo checkpoint mil vezes
    
    // Opcional: Sprite para quando ativar (bandeira levantada)
    public Sprite activeSprite; 
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            PlayerRespawn player = other.GetComponent<PlayerRespawn>();
            
            if (player != null)
            {
                // Salva a posição ATUAL desse objeto como o novo ponto de nascimento
                player.UpdateCheckpoint(transform.position);
                
                ActivateCheckpoint();
            }
        }
    }

    void ActivateCheckpoint()
    {
        activated = true;
        
        // Troca o sprite para parecer ativado
        if (activeSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = activeSprite;
        }
        
        // Opcional: Tocar um som aqui
    }
}