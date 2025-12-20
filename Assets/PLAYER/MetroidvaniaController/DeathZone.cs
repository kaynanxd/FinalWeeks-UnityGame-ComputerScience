using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Busca o script de Respawn no jogador e manda renascer
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            
            if (respawnScript != null)
            {
                respawnScript.Respawn();
            }
        }
        // Opcional: Se um inimigo cair, destrói ele
        else if (other.CompareTag("Inimigo"))
        {
            Destroy(other.gameObject);
        }
    }
}