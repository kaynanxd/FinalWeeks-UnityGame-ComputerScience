using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; 

public class GeradorPortas : MonoBehaviour
{
    public static string regraAtual;
    public static Sprite spriteAtual;

    [Header("Configurações de UI")]
    public Image displayDaPorta;
    public TextMeshProUGUI textoTempo;

    [Header("Configurações de Regras")]
    public Sprite[] portasDisponiveis;
    public float tempoParaTrocar = 10f;
    private float cronometroTroca;

    [Header("Configurações da Fase")]
    public float tempoTotalDaFase = 60f;
    public int idDestaFase = 1; 
    private bool jaFinalizou = false;

    [Header("--- Transição para Prova (Opcional) ---")]
    [Tooltip("Se vazio, o botão Continuar vai para o Lobby. Se preenchido, roda o diálogo antes.")]
    public DialogueData dialogoProfessor; 
    [Tooltip("Nome da cena da prova (Ex: CenaProva)")]
    public string nomeCenaProva;
    [Tooltip("O ID que a prova vai carregar (Geralmente o mesmo desta fase)")]
    public int idParaCarregarNaProva = 1;

    [Header("Efeitos Visuais")]
    public GameObject efeitoParticulaPrefab;
    public Transform localDoEfeito;

    [Header("--- Áudio ---")]
    public AudioSource sfxSource;
    public AudioClip somTrocaRegra;
    public AudioSource musicSource;
    public AudioClip musicaDaFase;
    private bool musicaDeveTocar = false;

    [Header("--- Diálogo Inicial ---")]
    public DialogueData dialogueInicial;

    void Start()
    {
        Time.timeScale = 1f;
        jaFinalizou = false;

        AudioSource[] todosOsAudios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in todosOsAudios)
        {
            if (audio != musicSource) audio.Stop();
        }

        MudarRegra();

        if (dialogueInicial != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueInicial, (string id) => { IniciarMusica(); });
        }
        else
        {
            IniciarMusica();
        }
    }

    void IniciarMusica()
    {
        if (musicSource != null && musicaDaFase != null)
        {
            musicSource.clip = musicaDaFase;
            musicSource.loop = true;
            musicSource.Play();
            musicaDeveTocar = true;
        }
    }

    void Update()
    {
        if (jaFinalizou) return; 

        if (musicSource != null && musicaDeveTocar)
        {
            if (Time.timeScale == 0 && musicSource.isPlaying) musicSource.Pause();
            else if (Time.timeScale > 0 && !musicSource.isPlaying) musicSource.UnPause();
        }

        cronometroTroca += Time.deltaTime;
        if (cronometroTroca >= tempoParaTrocar)
        {
            cronometroTroca = 0;
            MudarRegra();
        }

        if (tempoTotalDaFase > 0)
        {
            tempoTotalDaFase -= Time.deltaTime;
            if (textoTempo != null)
                textoTempo.text = "Tempo: " + Mathf.Max(0, tempoTotalDaFase).ToString("F0");
        }
        else
        {
            tempoTotalDaFase = 0;
            if (textoTempo != null) textoTempo.text = "Tempo: 0";
            
            jaFinalizou = true; 
            FinalizarFase(); 
        }
    }

    string ObterNomeBase(string nomeSujo)
        {
            if (string.IsNullOrEmpty(nomeSujo)) return "";
            // Pega só o que vem antes do underline ou espaço
            return nomeSujo.Split('_')[0].Split(' ')[0].Trim();
        }

    void MudarRegra()
    {
        if (portasDisponiveis.Length > 0)
        {
            int sorteio = 0;

            // Só faz a verificação se já existir uma regra atual (não for o Start) e tiver opções
            if (portasDisponiveis.Length > 1 && !string.IsNullOrEmpty(regraAtual))
            {
                string baseAtual = ObterNomeBase(regraAtual); // Ex: Transforma regra atual "NAND_1" em "NAND"
                string baseSorteada = "";
                int tentativas = 0; // Evita travar o jogo se só tiver regras iguais

                do
                {
                    sorteio = Random.Range(0, portasDisponiveis.Length);
                    baseSorteada = ObterNomeBase(portasDisponiveis[sorteio].name); // Ex: Transforma sorteio "NAND_2" em "NAND"
                    tentativas++;
                }
                // Repete enquanto o NOME BASE for igual (NAND == NAND)
                while (baseAtual == baseSorteada && tentativas < 100);
            }
            else
            {
                // Primeira vez ou lista pequena, pega aleatório simples
                sorteio = Random.Range(0, portasDisponiveis.Length);
            }

            // Aplica a regra
            Sprite imagemSorteada = portasDisponiveis[sorteio];
            displayDaPorta.sprite = imagemSorteada;
            displayDaPorta.preserveAspect = true;
            
            regraAtual = imagemSorteada.name; // O nome completo (com _1, _2) continua sendo usado para o sistema identificar
            spriteAtual = imagemSorteada;

            if (sfxSource != null && somTrocaRegra != null) sfxSource.PlayOneShot(somTrocaRegra);

            if (efeitoParticulaPrefab != null && localDoEfeito != null)
            {
                GameObject p = Instantiate(efeitoParticulaPrefab, localDoEfeito.position, Quaternion.identity);
                Destroy(p, 1.5f);
            }
        }
    }

    void FinalizarFase()
    {
        if (musicSource != null) musicSource.Stop();

        int nivelAserLiberado = idDestaFase + 1;
        int progressoAtual = PlayerPrefs.GetInt("NivelLiberado", 1);

        if (nivelAserLiberado > progressoAtual)
        {
            PlayerPrefs.SetInt("NivelLiberado", nivelAserLiberado);
            PlayerPrefs.Save();
        }

        if (Pontuação.instance != null)
        {
            Pontuação.instance.MostrarResultadoFinal();
        }
    }

    public void ExecutarSequenciaFinal()
    {
        // Se tiver diálogo configurado, roda a sequência da Prova
        if (dialogoProfessor != null)
        {
            StartCoroutine(SequenciaDeDialogoEProva());
        }
        // Se não tiver, vai para o Lobby normalmente
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("LobbyScene");
        }
    }

    IEnumerator SequenciaDeDialogoEProva()
    {
        // 1. Esconde o Painel de Pontos para limpar a tela
        if (Pontuação.instance != null && Pontuação.instance.painelFinal != null)
        {
            Pontuação.instance.painelFinal.SetActive(false);
        }

        // 2. Inicia o Diálogo
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogoProfessor);

            while (DialogueManager.Instance.dialoguePanel.activeSelf)
            {
                yield return null;
            }
        }

        // 3. Tenta carregar a prova
        GameSession.FaseAtualIndex = idParaCarregarNaProva;

        if (!string.IsNullOrEmpty(nomeCenaProva))
        {
            // Se tem nome, vai para a prova
            SceneManager.LoadScene(nomeCenaProva);
        }
        else
        {
            // --- MUDANÇA AQUI: FALLBACK PARA O LOBBY ---
            Debug.LogWarning("Nome da cena da prova não configurado. Voltando para o Lobby...");
            Time.timeScale = 1f;
            SceneManager.LoadScene("LobbyScene");
        }
    }
}