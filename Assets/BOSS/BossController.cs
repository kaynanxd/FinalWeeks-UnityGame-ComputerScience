using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Alvo e Movimento")]
    public Transform player;      
    public float orbitSpeed = 1f; 
    public float orbitRadius = 5f;
    public float moveSmoothness = 5f;

    [Header("Configuração do Arco (Novo)")]
    [Range(0, 180)] 
    public float arcAngle = 120f; // 120 graus de abertura total

    [Header("Combate")]
    public GameObject projectilePrefab; 
    public Transform firePoint;         
    public float fireRate = 3f;         

    private float timeCounter; // Contador de tempo para o balanço
    private float nextFireTime;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        HandleFacing();
        HandleShooting();
    }

    void FixedUpdate()
    {
        if (player == null) return;
        MoveAroundPlayerPhysics();
    }

    void MoveAroundPlayerPhysics()
    {
        // 1. Incrementa o contador de tempo baseado na velocidade
        timeCounter += orbitSpeed * Time.fixedDeltaTime;

        // 2. Calcula o balanço (Oscilação)
        // Mathf.Sin gera um número que vai suavemente de -1 a 1 e volta.
        float sineWave = Mathf.Sin(timeCounter);

        // 3. Converte para Graus
        // 90 graus é o topo (Céu). 
        // (arcAngle / 2) é quanto ele abre para cada lado (60 pra esq, 60 pra dir).
        float angleInDegrees = 90 + (sineWave * (arcAngle / 2));

        // 4. Converte Graus para Radianos (Unity precisa disso para calcular posição)
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // 5. Calcula a posição
        Vector2 offset = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians)) * orbitRadius;
        Vector2 desiredPosition = (Vector2)player.position + offset;

        // 6. Move suavemente com física
        Vector2 smoothedPosition = Vector2.Lerp(rb.position, desiredPosition, Time.fixedDeltaTime * moveSmoothness);
        
        rb.MovePosition(smoothedPosition);
    }

    void HandleFacing()
    {
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
            firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, 0);
        }
        else
        {
            spriteRenderer.flipX = false;
            firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, 0);
        }
    }

    void HandleShooting()
        {
            if (Time.time > nextFireTime)
            {
                // 1. Toca animação
                if(animator) animator.SetTrigger("Atacar");

                // 2. Cria o projétil e guarda numa variável temporária
                GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                
                // 3. O TRUQUE DE MESTRE: Ignorar Colisão
                // Pegamos o colisor do tiro que acabou de nascer
                Collider2D projectileCollider = newProjectile.GetComponent<Collider2D>();
                // Pegamos o colisor do próprio chefe
                Collider2D bossCollider = GetComponent<Collider2D>();

                // Se os dois existirem, mandamos a física ignorar o contato entre eles
                if (projectileCollider != null && bossCollider != null)
                {
                    Physics2D.IgnoreCollision(projectileCollider, bossCollider);
                }

                // 4. Configura o próximo tempo de tiro
                nextFireTime = Time.time + fireRate;
            }
        }
    
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // Desenha o raio total
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Vermelho transparente
            Gizmos.DrawWireSphere(player.position, orbitRadius);

            // Desenha linhas para mostrar onde começa e termina o arco (Visualização)
            Gizmos.color = Color.yellow;
            
            // Limite Esquerdo
            float leftRad = (90 + (arcAngle/2)) * Mathf.Deg2Rad;
            Vector3 leftPos = player.position + new Vector3(Mathf.Cos(leftRad), Mathf.Sin(leftRad), 0) * orbitRadius;
            Gizmos.DrawLine(player.position, leftPos);

            // Limite Direito
            float rightRad = (90 - (arcAngle/2)) * Mathf.Deg2Rad;
            Vector3 rightPos = player.position + new Vector3(Mathf.Cos(rightRad), Mathf.Sin(rightRad), 0) * orbitRadius;
            Gizmos.DrawLine(player.position, rightPos);
        }
    }
}