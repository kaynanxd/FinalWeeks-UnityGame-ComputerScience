using UnityEngine;
using System.Collections;

public class SkillTriggerZone : MonoBehaviour
{
    [Header("Configuração do Diálogo")]
    public DialogueData quizDialogue; 
    public string successNodeID = "acertou"; 
    
    [Header("Qual Habilidade Ativar?")]
    public SkillType skillToUnlock; // O ID da skill (Ex: DoubleJump)

    [Header("Configurações da Zona")]
    public float retryCooldown = 10f; 
    public bool destroyAfterUnlock = true; 
    
    private bool inCooldown = false;
    private bool quizIsActive = false;
    private bool alreadyUnlocked = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !inCooldown && !quizIsActive && !alreadyUnlocked)
        {
            StartQuiz();
        }
    }

    void StartQuiz()
    {
        quizIsActive = true;
        DialogueManager.Instance.StartDialogue(quizDialogue, OnDialogueFinished);
    }

    void OnDialogueFinished(string finalNodeID)
    {
        quizIsActive = false; 
        
        // Verificação insensível a maiúsculas/minúsculas e espaços
        string idChegado = finalNodeID.Trim();
        string idEsperado = successNodeID.Trim();

        if (idChegado.Equals(idEsperado, System.StringComparison.OrdinalIgnoreCase))
        {
            PerformUnlock();
        }
        else
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    void PerformUnlock()
    {
        alreadyUnlocked = true;

        // 1. Desbloqueia a lógica
        if(PlayerSkills.Instance != null) 
            PlayerSkills.Instance.UnlockSkill(skillToUnlock);

        // 2. Chama a UI Específica baseada no TIPO da skill
        if(ItemPickupManager.Instance != null)
        {
            ItemPickupManager.Instance.ShowSkillPanel(skillToUnlock);
        }

        if (destroyAfterUnlock)
        {
            gameObject.SetActive(false); 
        }
    }

    IEnumerator CooldownRoutine()
    {
        inCooldown = true;
        yield return new WaitForSeconds(retryCooldown);
        inCooldown = false;
    }
}