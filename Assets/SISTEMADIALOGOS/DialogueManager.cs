using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public bool IsDialogueActive 
{ 
    get { return dialoguePanel.activeSelf; } 
}

    private System.Action<string> onDialogueEndedCallback;
    
    [Header("UI Components")]
    public GameObject dialoguePanel;       // O painel geral
    public Image portraitImage;            // Foto do personagem
    public TextMeshProUGUI nameText;       // Nome do personagem
    public TextMeshProUGUI dialogueText;   // Texto da fala
    
    [Header("Área de Escolhas")]
    public Transform choiceContainer;      // Onde as opções vão aparecer
    public GameObject choiceButtonPrefab;  // O prefab (estilo texto)
    public GameObject choicesPanelBackground; // (Opcional) Se quiser um fundo só para as opções

    [Header("Audio Global")]
    public float typingSpeed = 0.04f;
    public AudioSource audioSource;
    public AudioClip globalTypingSound; // <--- ARRASTE SEU SOM DE BEEP AQUI

    // Variáveis internas
    private Queue<Sentence> sentences;
    private DialogueData currentDataWrapper;
    private DialogueNode currentNode;
    private bool isTyping = false;
    private string currentFullText = "";

    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        sentences = new Queue<Sentence>();
        
        // Garante que tudo começa fechado
        dialoguePanel.SetActive(false);
        if(choicesPanelBackground) choicesPanelBackground.SetActive(false);
    }

    public void StartDialogue(DialogueData data, System.Action<string> onEnd = null)
    {
        Time.timeScale = 0f;
        currentDataWrapper = data;
        onDialogueEndedCallback = onEnd; // <--- Guarda a função para chamar no final
        StartNode(data.startNodeID);
    }

    void StartNode(string nodeName)
    {
        DialogueNode node = currentDataWrapper.GetNode(nodeName);
        if (node == null)
        {
            Debug.LogError($"Nó '{nodeName}' não encontrado!");
            return;
        }

        currentNode = node;
        dialoguePanel.SetActive(true);
        
        // Esconde as opções enquanto fala
        choiceContainer.gameObject.SetActive(false);
        if(choicesPanelBackground) choicesPanelBackground.SetActive(false);

        // Limpa opções antigas
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);

        sentences.Clear();
        foreach (Sentence s in node.sentences)
        {
            sentences.Enqueue(s);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentFullText;
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            ShowChoicesOrClose();
            return;
        }

        Sentence sentence = sentences.Dequeue();
        currentFullText = sentence.text;

        nameText.text = sentence.characterName;
        portraitImage.sprite = sentence.portrait;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

IEnumerator TypeSentence(Sentence sentence)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char letter in sentence.text.ToCharArray())
        {
            dialogueText.text += letter;

            if (globalTypingSound != null && audioSource != null && !char.IsWhiteSpace(letter))
            {
                // Nota: Pitch de audio pode soar estranho com TimeScale 0 se não configurar o AudioSource,
                // mas geralmente funciona bem para sons curtos (UI).
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(globalTypingSound);
            }

            // --- A MUDANÇA IMPORTANTE ESTÁ AQUI EMBAIXO ---
            // Use Realtime para ignorar o Time.timeScale = 0
            yield return new WaitForSecondsRealtime(typingSpeed); 
        }

        isTyping = false;
    }

void ShowChoicesOrClose()
    {
        if (currentNode.choices.Count == 0)
            {
                // Antes de fechar, guarda o ID do nó final
                string finalNodeID = currentNode.nodeID;

                dialoguePanel.SetActive(false);
                Time.timeScale = 1f;

                // AVISO: Chama quem pediu o diálogo avisando: "Acabou neste nó!"
                onDialogueEndedCallback?.Invoke(finalNodeID);
                onDialogueEndedCallback = null; // Limpa para não dar erro depois

                return;
            }

        choiceContainer.gameObject.SetActive(true);
        if(choicesPanelBackground) choicesPanelBackground.SetActive(true);

        foreach (Choice choice in currentNode.choices)
        {
            // Instancia o objeto de texto
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceContainer);
            
            // Pega o nosso script novo
            TextoClicavel textoClicavel = btnObj.GetComponent<TextoClicavel>();

            // Configura o texto e o que acontece ao clicar
            textoClicavel.Configurar(choice.text, () => 
            {
                if (choice.externalDialogue != null)
                    StartDialogue(choice.externalDialogue);
                else
                    StartNode(choice.targetNodeID);
            });
        }
    }
    // Adicione isso dentro do DialogueManager.cs
    void Update()
    {
        // Só escuta inputs se o diálogo estiver aberto
        if (dialoguePanel.activeSelf)
        {
            // Se as opções estiverem na tela, não avance (deixe o mouse clicar)
            if (choiceContainer.gameObject.activeSelf) return;

            // Se apertar Enter, E ou Clicar
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                DisplayNextSentence();
            }
        }
    }
}