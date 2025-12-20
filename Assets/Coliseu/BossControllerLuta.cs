using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena
using System.Collections; // Necessário para Corrotinas

public class BossControllerLuta : MonoBehaviour
{
    [Header("Status")]
    public float maxHealth = 100f;
    private float currentHealth;
    public BossHealthUI healthBar; 
    
    public AudioClip teleportSoundClip; 
    
    [Header("Movimento (Flutuar)")]
    public float floatSpeed = 2f;
    public float floatDistance = 1.5f;
    private Vector3 startPosition;

    [Header("Teleporte e Fases")]
    public Transform secondPhasePoint;
    public Transform thirdPhasePoint; 
    public float teleportThreshold2 = 0.7f; 
    public float teleportThreshold3 = 0.4f; 
    public GameObject teleportEffect;
    
    private int currentPhase = 1; 

    [Header("Combate")]
    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f; 
    private float nextFireTime;

    [Header("Dificuldade Manual")]
    public float fireRatePhase2 = 0.75f; 
    public float floatSpeedPhase2 = 3.0f; 
    public float fireRatePhase3 = 0.5f; 
    public float floatSpeedPhase3 = 4.0f;

    // --- NOVO: SEÇÃO DE FIM DE JOGO ---
    [Header("Fim de Jogo / Vitória")]

    [Tooltip("0 = Fase 1, 1 = Fase 2, 2 = Fase 3, 3 = Fase 4")]
    public int idDaFaseDesteChefe = 2;
    public DialogueData victoryDialogue; // Arraste o arquivo de diálogo aqui
    public string sceneToLoad; // Digite o nome exato da próxima cena (ex: "Menu" ou "Fase2")

    private SpriteRenderer spriteRenderer;
    private Animator animator; 
    private bool isDead = false; // Trava para impedir que ele morra duas vezes

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetMaxHealth(maxHealth);
        startPosition = transform.position;
    }

    void Update()
    {
        // Se o chefe morreu, paramos toda a lógica de ataque e movimento
        if (player == null || isDead) return;

        HandleFloating();
        HandleFacing();
        HandleShooting();
    }

    void HandleFloating()
    {
        float newX = startPosition.x + Mathf.Sin(Time.time * floatSpeed) * floatDistance;
        transform.position = new Vector3(newX, startPosition.y, transform.position.z);
    }

    void HandleFacing()
    {
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true; 
            if(firePoint != null)
            {
                 if(firePoint.localPosition.x > 0) 
                    firePoint.localPosition = new Vector3(-firePoint.localPosition.x, firePoint.localPosition.y, 0);
            }
        }
        else
        {
            spriteRenderer.flipX = false; 
            if(firePoint != null)
            {
                if(firePoint.localPosition.x < 0) 
                    firePoint.localPosition = new Vector3(-Mathf.Abs(firePoint.localPosition.x), firePoint.localPosition.y, 0);
            }
        }
    }

    void HandleShooting()
    {
        if (Time.time > nextFireTime)
        {
            if (animator) animator.SetTrigger("Atacar");

            if (projectilePrefab != null && firePoint != null)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Vector2 direction = (player.position - firePoint.position).normalized;
                Rigidbody2D projRb = newProjectile.GetComponent<Rigidbody2D>();
                if(projRb) projRb.linearVelocity = direction * 5f; 
                
                Collider2D myCol = GetComponent<Collider2D>();
                Collider2D projCol = newProjectile.GetComponent<Collider2D>();
                if(myCol && projCol) Physics2D.IgnoreCollision(myCol, projCol);
            }

            nextFireTime = Time.time + fireRate;
        }
    }

    public void ApplyDamage(float damage)
    {
        if (isDead) return; // Não toma dano se já estiver morto

        currentHealth -= damage;
        
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        StartCoroutine(FlashRed());

        if (currentPhase == 1 && currentHealth <= maxHealth * teleportThreshold2)
        {
            TeleportToPhase(2);
        }
        else if (currentPhase == 2 && currentHealth <= maxHealth * teleportThreshold3)
        {
            TeleportToPhase(3);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void TeleportToPhase(int phase)
    {
        Transform targetPoint = null;
        float newFireRate = fireRate;
        float newFloatSpeed = floatSpeed;
        
        if (phase == 2)
        {
            targetPoint = secondPhasePoint;
            newFireRate = fireRatePhase2;
            newFloatSpeed = floatSpeedPhase2;
        }
        else if (phase == 3)
        {
            targetPoint = thirdPhasePoint;
            newFireRate = fireRatePhase3;
            newFloatSpeed = floatSpeedPhase3;
        }

        if (targetPoint == null)
        {
            Debug.LogError($"Ponto de teleporte para Fase {phase} não configurado!");
            return;
        }

        if (teleportSoundClip != null)
        {
            GameObject soundObject = new GameObject("TempTeleportSound");
            soundObject.AddComponent<TemporarySoundPlayer>().soundClip = teleportSoundClip;
        }

        if (teleportEffect) 
            Instantiate(teleportEffect, transform.position, Quaternion.identity);

        currentPhase = phase; 
        
        fireRate = newFireRate;
        floatSpeed = newFloatSpeed;
        
        transform.position = targetPoint.position;
        startPosition = targetPoint.position; 
        
        if (teleportEffect) 
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Toca animação
        if (animator) animator.SetTrigger("Morreu");
        
        // 2. Desativa colisão para não interagir mais
        GetComponent<Collider2D>().enabled = false; 
        this.enabled = false; // Desativa o Update deste script (mas as Corrotinas continuam rodando)

        // 3. Esconde a barra de vida
        if (healthBar != null && healthBar.border != null)
        {
            healthBar.border.SetActive(false); 
        }

        // 4. Inicia a sequência de fim de jogo
        StartCoroutine(VictorySequence());
    }

    // --- NOVA LÓGICA DE SEQUÊNCIA DE VITÓRIA ---
    IEnumerator VictorySequence()
        {
            yield return new WaitForSeconds(0f);

            if (spriteRenderer) spriteRenderer.enabled = false;

            if (victoryDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(victoryDialogue);
                while (DialogueManager.Instance.dialoguePanel.activeSelf)
                {
                    yield return null; 
                }
            }

            // --- MUDANÇA AQUI: GRAVAR O ID ANTES DE CARREGAR ---
            
            // Avisa ao sistema qual prova carregar na próxima cena
            GameSession.FaseAtualIndex = idDaFaseDesteChefe; 
            
            // ---------------------------------------------------

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("Nome da cena para carregar está vazio!");
            }

            Destroy(gameObject);
        }

    System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}