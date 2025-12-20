using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool dash = false;

	//bool dashAxis = false;
	
	// Update is called once per frame
	void Update () {

			// --- CORREÇÃO AQUI ---
			// Se o Diálogo estiver ativo OU o Jogo estiver Pausado pelo Menu
			if ((DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) || PauseMenu.GameIsPaused)
			{
				// Zera o movimento para garantir que ele pare instantaneamente
				horizontalMove = 0f;
				// Força a animação a voltar para Idle (Parado) para cortar o som de passos
				animator.SetFloat("Speed", 0f);
				
				// Sai da função Update, impedindo que qualquer input seja lido
				return; 
			}
			// ---------------------

			horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

			animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

			if (Input.GetKeyDown(KeyCode.Space))
			{
				jump = true;
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				dash = true;
			}
		}

	public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
		jump = false;
		dash = false;
	}
}
