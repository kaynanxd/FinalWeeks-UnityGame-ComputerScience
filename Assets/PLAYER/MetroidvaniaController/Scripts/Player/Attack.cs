using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float dmgValue = 4;
	public GameObject throwableObject;
	public Transform attackCheck;
	private Rigidbody2D m_Rigidbody2D;
	public Animator animator;
	public bool canAttack = true;
	public bool isTimeToCheck = false;

	public GameObject cam;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
	void Update()
		{
			// --- MUDANÇA 1: TRAVA DE DIÁLOGO ---
			// Se estiver conversando, não lê nenhum input de ataque
			//if (DialogueManager.IsTalking) return; 

			// --- MUDANÇA 2: TRAVA DE HABILIDADE (ATAQUE CORPO A CORPO) ---
			// Adicionamos && PlayerSkills.Instance.canAttack
			if (Input.GetKeyDown(KeyCode.Q) && canAttack && PlayerSkills.Instance.canAttack)
				{
					canAttack = false;
					animator.SetBool("IsAttacking", true);
					
					// ADICIONE ESTA LINHA AQUI:
					CheckMeleeAttack(); 
					
					StartCoroutine(AttackCooldown());
				}
			// --- MUDANÇA 3 (OPCIONAL): TRAVA PARA O ARREMESSO (V) ---
			// Se você quiser que o arremesso TAMBÉM precise ser desbloqueado, 
			// adicione a verificação aqui também. Se for uma habilidade separada, ignore.
			if (Input.GetKeyDown(KeyCode.V)) // && PlayerSkills.Instance.canAttack (Se quiser bloquear tbm)
			{
				GameObject throwableWeapon = Instantiate(throwableObject, transform.position + new Vector3(transform.localScale.x * 0.5f,-0.2f), Quaternion.identity) as GameObject; 
				Vector2 direction = new Vector2(transform.localScale.x, 0);
				throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction; 
				throwableWeapon.name = "ThrowableWeapon";
			}
		}

	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(0.5f);
		canAttack = true;
	}

	public void DoDashDamage()
	{
		dmgValue = Mathf.Abs(dmgValue);
		Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
		for (int i = 0; i < collidersEnemies.Length; i++)
		{
			if (collidersEnemies[i].gameObject.tag == "Enemy")
			{
				if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
				{
					dmgValue = -dmgValue;
				}
				collidersEnemies[i].gameObject.SendMessage("ApplyDamage", dmgValue);
				cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}

		public void CheckMeleeAttack()
	{
		// Cria um círculo na frente do player (attackCheck) para ver se pegou no chefe
		Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f); // 0.9f é o raio do ataque

		foreach (Collider2D enemy in hitEnemies)
		{
			// Se o objeto tiver a tag "Boss" (Lembre de colocar a Tag no Chefe!)
			if (enemy.CompareTag("Boss") || enemy.CompareTag("Enemy"))
			{
				// Manda o dano para o script do chefe
				enemy.SendMessage("ApplyDamage", dmgValue, SendMessageOptions.DontRequireReceiver);
				
				// Treme a câmera (se tiver referência)
				if(cam != null) cam.GetComponent<CameraFollow>().ShakeCamera();
			}
		}
	}
	// Adicione isso no final do Attack.cs, antes do último fecha-chaves "}"
	void OnDrawGizmosSelected()
	{
		if (attackCheck != null)
		{
			Gizmos.color = Color.red;
			// 0.9f é o tamanho do raio que usamos na lógica do CheckMeleeAttack
			Gizmos.DrawWireSphere(attackCheck.position, 0.9f);
		}
	}
}
