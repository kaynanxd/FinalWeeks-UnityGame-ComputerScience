using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement; // Necessário para carregar a cena do mapa

public class ExamManager : MonoBehaviour
{
    [Header("Cutscenes")]
    public GameObject panelCutscene;      // O painel preto
    public VideoPlayer videoPlayer;       // O tocador de vídeo
    public VideoClip cutsceneFinalClip;   // ARRASTE O VIDEO FINAL AQUI

    private bool _alunoFoiAprovado = false;
    
    [Header("Navegação")]
    public string nomeCenaMapa;           // Digite o nome exato da cena do mapa
    public Button botaoContinuar;         // O botão "Continuar" da tela de resultado

    // --- ADIÇÃO 1: Nome da cena final ---
    [Tooltip("Nome da cena para carregar se for a Fase 3 (Final do Jogo)")]
    public string nomeCenaFinalJogo; // <--- NOVO: Escreva o nome da cena final aqui
    // ------------------------------------

    [Header("Banco de Provas")]
    public TextMeshProUGUI textoTituloProva;
    public ExamData[] listaDeTodasAsProvas;

    [Header("Referências de Prefab")]
    public GameObject questionPrefab; 
    public Transform contentArea; 

    [Header("UI - Telas")]
    public GameObject telaProva;      
    public GameObject telaResultado;  

    [Header("UI - Elementos")]
    public TextMeshProUGUI textoResultado; 
    public Button botaoEntregar;

    private ExamData _provaSelecionada; 
    private List<QuestionUI> _questoesGeradas = new List<QuestionUI>();
    
    // Variável para saber qual vídeo está tocando (Intro ou Final)
    private bool _isIntroVideo = true; 

    void Start()
    {
        // 1. Configura Telas
        if(telaResultado != null) telaResultado.SetActive(false);
        
        // 2. Configura Botões
        if (botaoEntregar != null)
        {
            botaoEntregar.onClick.RemoveAllListeners(); 
            botaoEntregar.onClick.AddListener(CorrigirProva);
        }

        if (botaoContinuar != null)
        {
            botaoContinuar.onClick.RemoveAllListeners();
            botaoContinuar.onClick.AddListener(TocarCutsceneFinal);
        }

        // 3. Lógica da Cutscene Inicial
        if (panelCutscene != null && videoPlayer != null)
        {
            _isIntroVideo = true; // Marca que é a intro
            
            // Ativa a cutscene e esconde a prova
            panelCutscene.SetActive(true);
            telaProva.SetActive(false);

            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play(); // Toca o vídeo que já estiver no componente (Intro)
        }
        else
        {
            IniciarProva();
        }
    }

    void Update()
    {
        // Permite pular o vídeo com Espaço
        if (panelCutscene != null && panelCutscene.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            OnVideoFinished(videoPlayer); 
        }
    }

    // Chamado automaticamente quando QUALQUER vídeo acaba
    void OnVideoFinished(VideoPlayer vp)
    {
        panelCutscene.SetActive(false);
        
        if (_isIntroVideo)
        {
            // Se acabou a INTRO -> Começa a prova
            IniciarProva();
        }
        else
        {
            // Se acabou o FINAL -> Carrega o mapa
            CarregarMapa();
        }
    }

    // Chamado pelo Botão "Continuar" na tela de resultados
    void TocarCutsceneFinal()
    {
        if (panelCutscene != null && videoPlayer != null && cutsceneFinalClip != null)
        {
            _isIntroVideo = false; // Agora não é mais intro
            
            videoPlayer.clip = cutsceneFinalClip; // Troca o vídeo para o final
            panelCutscene.SetActive(true);        // Liga a tela preta de novo
            videoPlayer.Play();                   // Toca
        }
        else
        {
            // Se não tiver vídeo configurado, vai direto pro mapa
            CarregarMapa();
        }
    }

    void CarregarMapa() 
        {
            GameSession.DeveAbrirMapaDireto = true;

            if (_alunoFoiAprovado)
            {
                int proximoNivelDeDesbloqueio = GameSession.FaseAtualIndex + 2;

                if (proximoNivelDeDesbloqueio > GameSession.FasesDesbloqueadas)
                {
                    GameSession.FasesDesbloqueadas = proximoNivelDeDesbloqueio;
                    Debug.Log("Nova fase desbloqueada! Total liberado: " + GameSession.FasesDesbloqueadas);
                }
            }

            // --- VERIFICAÇÃO DO FINAL DO JOGO ---
            
            // Se a fase atual for a 3 (e o aluno passou, pois chegou aqui)...
            if (GameSession.FaseAtualIndex == 3)
            {
                Debug.Log("Fase 3 Concluída! Indo para o Final do Jogo.");
                
                if (!string.IsNullOrEmpty(nomeCenaFinalJogo))
                {
                    SceneManager.LoadScene(nomeCenaFinalJogo);
                    return; // Para o código aqui para não carregar o mapa abaixo
                }
                else
                {
                    Debug.LogWarning("ATENÇÃO: Você está na fase 3 mas esqueceu de colocar o nome da cena final no Inspector!");
                }
            }
            // -------------------------------------

            if (!string.IsNullOrEmpty(nomeCenaMapa))
            {
                SceneManager.LoadScene(nomeCenaMapa); 
            }
            else
            {
                Debug.LogError("Nome da cena do mapa/menu não foi configurado no Inspector!");
            }
        }

    void IniciarProva()
    {
        if(telaProva != null) telaProva.SetActive(true);
        CarregarProvaCorreta();
    }

    // --- O RESTO CONTINUA IGUAL ---
    
    void CarregarProvaCorreta()
        {
            // AJUSTE CRÍTICO: Converter ID da Fase (1,2,3) para Índice do Array (0,1,2)
            int idDaFase = GameSession.FaseAtualIndex;
            int indexDoArray = idDaFase - 1; 

            // Verificação de segurança (Impede que o jogo trave se o ID for 0 ou maior que a lista)
            if (indexDoArray >= 0 && indexDoArray < listaDeTodasAsProvas.Length)
            {
                _provaSelecionada = listaDeTodasAsProvas[indexDoArray]; // Usa o índice corrigido
                GerarProva(_provaSelecionada); 
                Debug.Log($"Carregando prova da Fase {idDaFase} (Item {indexDoArray} da lista).");
            }
            else 
            { 
                Debug.LogError($"ERRO GRAVE: O jogo pediu a prova da Fase {idDaFase}, o que seria o item {indexDoArray} da lista, mas sua lista só tem {listaDeTodasAsProvas.Length} provas configuradas no Inspector!"); 
            }
        }

    void GerarProva(ExamData dados)
    {
        if (textoTituloProva != null) textoTituloProva.text = dados.tituloDaProva;
        foreach (Transform child in contentArea)
        {
            if (child.GetComponent<QuestionUI>() != null) Destroy(child.gameObject);
        }
        _questoesGeradas.Clear();
        foreach (var q in dados.questoes)
        {
            GameObject obj = Instantiate(questionPrefab, contentArea);
            QuestionUI scriptUI = obj.GetComponent<QuestionUI>();
            scriptUI.Setup(q);
            _questoesGeradas.Add(scriptUI);
        }
        StartCoroutine(ForceLayoutUpdate());
    }

    IEnumerator ForceLayoutUpdate()
    {
        yield return new WaitForEndOfFrame();
        contentArea.gameObject.SetActive(false); 
        contentArea.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentArea.GetComponent<RectTransform>());
    }

    void CorrigirProva()
    {
        int acertos = 0;
        foreach (var qUI in _questoesGeradas)
        {
            if (qUI.IsCorrect()) acertos++;
        }
        ExibirResultado(acertos);
    }

    void ExibirResultado(int nota)
        {
            telaProva.SetActive(false);
            telaResultado.SetActive(true);

            int totalQuestoes = _provaSelecionada.questoes.Count;
            int notaMinima = _provaSelecionada.notaMinimaParaAprovar;

            if (nota >= notaMinima)
            {
                // APROVADO
                _alunoFoiAprovado = true; // <--- MARCA QUE PASSOU
                textoResultado.text = $"APROVADO!\nAcertos: {nota}/{totalQuestoes}";
                textoResultado.color = Color.green;
                if(botaoContinuar) botaoContinuar.interactable = true; 
            }
            else
            {
                // REPROVADO
                _alunoFoiAprovado = false; // <--- MARCA QUE REPROVOU
                textoResultado.text = $"REPROVADO.\nAcertos: {nota}/{totalQuestoes}";
                textoResultado.color = Color.red;
                // Opcional: Bloquear o botão continuar se quiser obrigar a refazer
            }
        }
}