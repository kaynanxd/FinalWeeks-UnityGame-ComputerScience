using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType { Interaction, AutoStartZone }

    [Header("Configuração")]
    public TriggerType triggerType;
    public DialogueData dialogueData;
    
    [Header("Apenas para Zonas")]
    public bool playOnce = true; // Se for true, a cutscene só acontece uma vez
    private bool hasPlayed = false;

    [Header("Visual (Apenas Interação)")]
    public GameObject interactIcon; // Ex: Tecla "E" flutuando

    private bool playerInRange = false;

    void Start()
    {
        if(interactIcon) interactIcon.SetActive(false);
    }

    void Update()
        {
            // Se não for interação por botão, sai fora
            if (triggerType != TriggerType.Interaction) return;

            // Se o jogador está perto
            if (playerInRange)
            {
                // Se apertar o botão de interação
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
                {
                    // AQUI ESTÁ A CORREÇÃO:
                    // O Gatilho só age se o diálogo estiver FECHADO.
                    // Se estiver aberto, o Update do Manager (que criamos acima) cuida do resto.
                    
                    if (!DialogueManager.Instance.dialoguePanel.activeSelf)
                    {
                        DialogueManager.Instance.StartDialogue(dialogueData);
                    }
                    
                    // (Opcional) Esconde o ícone
                    if(interactIcon) interactIcon.SetActive(false);
                }
            }
            
            // --- APAGUEI O BLOCO 'ELSE IF' QUE EXISTIA AQUI ---
            // Aquele bloco era o culpado por duplicar os comandos.
        }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerType == TriggerType.AutoStartZone)
            {
                if (playOnce && hasPlayed) return; // Já tocou, não toca mais
                
                DialogueManager.Instance.StartDialogue(dialogueData);
                hasPlayed = true;
            }
            else
            {
                playerInRange = true;
                if(interactIcon) interactIcon.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if(interactIcon) interactIcon.SetActive(false);
        }
    }
}