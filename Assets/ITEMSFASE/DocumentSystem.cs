using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DocumentSystem : MonoBehaviour
{
    public static DocumentSystem Instance;

    [Header("Configuração UI")]
    public GameObject documentPanel;   // O Painel inteiro (fundo + textos)
    public TextMeshProUGUI titleText;  // Lugar do título
    public TextMeshProUGUI bodyText;   // Lugar do texto longo
    public Image illustrationImage;    // (Opcional) Lugar da imagem
    
    [Header("Botões de Navegação")]
    public GameObject btnNext;         // Botão Próximo >
    public GameObject btnPrev;         // Botão Anterior <
    public GameObject btnClose;        // Botão Fechar/Continuar

    [Header("Teclas")]
    public KeyCode inventoryKey = KeyCode.I; // Tecla para abrir inventário

    // Lista interna do que o jogador já pegou
    private List<DocumentData> collectedDocuments = new List<DocumentData>();
    
    // Qual página estamos lendo agora?
    private int currentIndex = 0;
    private bool isReading = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Começa fechado
        if(documentPanel) documentPanel.SetActive(false);
    }

    void Update()
    {
        // Se apertar 'I' e não estiver lendo nada, abre o inventário
        if (Input.GetKeyDown(inventoryKey) && !isReading)
        {
            OpenInventory();
        }
        // Se apertar 'I' ou 'ESC' enquanto lê, fecha
        else if ((Input.GetKeyDown(inventoryKey) || Input.GetKeyDown(KeyCode.Escape)) && isReading)
        {
            CloseDocument();
        }
    }

    // --- FUNÇÕES DE COLETA ---

    // Chamado pelo objeto no chão quando o jogador pega
    public void CollectAndRead(DocumentData doc)
    {
        // 1. Adiciona à lista se ainda não tiver
        if (!collectedDocuments.Contains(doc))
        {
            collectedDocuments.Add(doc);
        }

        // 2. Abre a tela direto nesse documento
        currentIndex = collectedDocuments.IndexOf(doc);
        ShowReaderUI();
    }

    // --- FUNÇÕES DE UI E NAVEGAÇÃO ---

    public void OpenInventory()
    {
        if (collectedDocuments.Count == 0)
        {
            Debug.Log("Você não tem documentos ainda.");
            return; 
        }

        // Abre sempre no primeiro (ou no último lido, se preferir)
        currentIndex = 0;
        ShowReaderUI();
    }

    void ShowReaderUI()
    {
        isReading = true;
        documentPanel.SetActive(true);
        Time.timeScale = 0f; // Pausa o jogo

        UpdatePageContent();
    }

    public void CloseDocument()
    {
        isReading = false;
        documentPanel.SetActive(false);
        Time.timeScale = 1f; // Despausa o jogo
    }

    public void NextPage()
    {
        if (currentIndex < collectedDocuments.Count - 1)
        {
            currentIndex++;
            UpdatePageContent();
        }
    }

    public void PrevPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePageContent();
        }
    }

    // Atualiza os textos e controla se os botões aparecem ou somem
    void UpdatePageContent()
    {
        if (collectedDocuments.Count == 0) return;

        DocumentData doc = collectedDocuments[currentIndex];

        // Preenche textos
        titleText.text = doc.titulo;
        bodyText.text = doc.textoCompleto;

        // Imagem (se tiver)
        if (illustrationImage != null)
        {
            if (doc.imagemIlustrativa != null)
            {
                illustrationImage.sprite = doc.imagemIlustrativa;
                illustrationImage.gameObject.SetActive(true);
            }
            else
            {
                illustrationImage.gameObject.SetActive(false);
            }
        }

        // Lógica dos Botões: Só mostra "Próximo" se não for o último
        btnNext.SetActive(currentIndex < collectedDocuments.Count - 1);
        
        // Só mostra "Anterior" se não for o primeiro
        btnPrev.SetActive(currentIndex > 0);
    }
}