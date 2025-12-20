using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossProjectile : MonoBehaviour
{
    [Header("Configuração da Trajetória")]
    public float speed = 12f;      
    public float turnSpeed = 200f; 
    
    [Tooltip("Tempo (segundos) voando reto antes de perseguir.")]
    public float homingDelay = 0.5f; 

    [Tooltip("Se o projétil chegar mais perto que isso, ele para de perseguir e vai reto (Permite desviar).")]
    public float stopHomingDistance = 3f; // <--- NOVO: Distância para parar de guiar

    [Header("Correção Visual")]
    public float spriteRotationOffset = 0f; 

    [Header("Efeitos (Audio e Visual)")]
    public GameObject explosionPrefab; // <--- NOVO: Prefab da explosão
    public AudioClip launchSound;      // <--- NOVO: Som do disparo
    public AudioClip hitSound;         // <--- NOVO: Som do impacto

    public int damage = 1;
    public float lifetime = 6f;

    private Transform target;
    private Rigidbody2D rb;
    private float timeSinceSpawn = 0f;
    private bool stopTracking = false; // Controle interno

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Toca o som de lançamento se houver
        if (launchSound != null)
            AudioSource.PlayClipAtPoint(launchSound, transform.position);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            LaunchHorizontally();
        }
        else
        {
            rb.linearVelocity = Vector2.left * speed;
        }
        
        Destroy(gameObject, lifetime);
    }

    void LaunchHorizontally()
    {
        float directionX = (target.position.x < transform.position.x) ? -1 : 1;
        rb.linearVelocity = new Vector2(directionX, 0) * speed;
    }

    void FixedUpdate()
    {
        timeSinceSpawn += Time.fixedDeltaTime;
        UpdateSpriteRotation();

        if (target == null) return;

        // 1. Se já decidimos parar de seguir, apenas sai da função (mantém velocidade reta)
        if (stopTracking) return;

        // 2. Checa o Delay Inicial
        if (timeSinceSpawn < homingDelay) return;

        // 3. NOVO: Checa a Distância
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        
        if (distanceToPlayer < stopHomingDistance)
        {
            // Se chegou muito perto, para de virar o volante!
            stopTracking = true; 
            return;
        }

        // 4. Lógica de Perseguição
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        Vector3 newDirection = Vector3.RotateTowards(rb.linearVelocity.normalized, directionToTarget, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
        rb.linearVelocity = newDirection * speed;
    }

    void UpdateSpriteRotation()
    {
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + spriteRotationOffset, Vector3.forward);
        }
    }

    // void OnTriggerEnter2D(Collider2D other)
    //     {
    //         // Ignora o próprio chefe
    //         if (other.CompareTag("Boss")) return;

    //         // Verifica se bateu no jogador
    //         if (other.CompareTag("Player"))
    //         {
    //             // Tenta pegar o script PlayerHealth
    //             PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                
    //             if (playerHealth != null)
    //             {
    //                 playerHealth.TakeDamage(damage);
    //             }
                
    //             Impact(); // Explode o projétil
    //         }
    //         // Se bater no chão (Layer Ground) ou Parede, também explode
    //         else if (other.CompareTag("Ground")) 
    //         {
    //             Impact();
    //         }
    //     }


        void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora o próprio chefe
        if (other.CompareTag("Boss")) return;

        // Verifica se bateu no jogador
        if (other.CompareTag("Player"))
        {
            // --- MUDANÇA AQUI ---
            // Em vez de procurar PlayerHealth, procuramos o CharacterController2D
            // que é onde está a lógica de vida e de empurrão (knockback)
            CharacterController2D playerController = other.GetComponent<CharacterController2D>();
            
            if (playerController != null)
            {
                // Chamamos a função ApplyDamage passando:
                // 1. O dano
                // 2. A posição DESTE projétil (transform.position)
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                // Isso permite ao player calcular a direção contrária para ser empurrado
                playerController.ApplyDamage(damage, transform.position);
                playerHealth.TakeDamage(damage);
            }
            // --------------------
            
            Impact(); // Explode o projétil
        }
        // Se bater no chão (Layer Ground) ou Parede, também explode
        else if (other.CompareTag("Ground")) 
        {
            Impact();
        }
    }

    // Função para gerenciar a morte do projétil
    void Impact()
    {
        // 1. Cria a explosão visual
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. Toca o som de impacto
        // Usamos PlayClipAtPoint porque o objeto do tiro vai ser destruído agora,
        // então o AudioSource dele sumiria junto cortando o som.
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // 3. Tchau projétil
        Destroy(gameObject);
    }
}