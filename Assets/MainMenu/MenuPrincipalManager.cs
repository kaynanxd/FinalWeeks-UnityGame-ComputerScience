using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI; // Necessário para mexer na Imagem do botão

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelmenuinicial;
    [SerializeField] private GameObject painelopcoes;
    [SerializeField] private GameObject painelMapa;

    [Header("Avisos de Bloqueio")]
    [SerializeField] private GameObject painelAvisoBloqueado;

    [Header("Ferramentas de Debug (Cheat)")]
    [Tooltip("Aperte esta tecla no menu para liberar tudo.")]
    [SerializeField] private KeyCode teclaDebugCheat = KeyCode.F1;

    [Header("Configuração Visual do Mapa")]
    [Tooltip("Arraste os botões na ordem: Fase 1, Fase 2, etc.")]
    [SerializeField] private Button[] botoesDasFases; 
    
    [Tooltip("Imagem do Cadeado (Bloqueado)")]
    [SerializeField] private Sprite spriteCadeado; 

    [Tooltip("Imagens originais das fases (Ícone 1, Ícone 2...) na mesma ordem dos botões")]
    [SerializeField] private List<Sprite> spritesDasFases;

    [Header("Configuração da Cutscene")]
    [SerializeField] private GameObject painelCutscene;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip introCutsceneClip;
    [SerializeField] private KeyCode teclaParaPular = KeyCode.Escape;

    [Header("Configuração das Fases")]
    [SerializeField] private List<VideoClip> cutscenesDasFases;
    [SerializeField] private List<string> nomesDasCenasDasFases;
    [SerializeField] private AudioClip musicaDoMenu;

    private bool cutsceneTocando = false;
    private string cenaParaCarregar; 

    private void Start()
    {
        Debug.Log("Fases Desbloqueadas: " + GameSession.FasesDesbloqueadas);

        // 1. Lógica de Troca de Tela
        if (GameSession.DeveAbrirMapaDireto)
        {
            if(painelmenuinicial) painelmenuinicial.SetActive(false);
            if(painelMapa) painelMapa.SetActive(true);
            GameSession.DeveAbrirMapaDireto = false; 
        }
        else
        {
            if(painelmenuinicial) painelmenuinicial.SetActive(true);
            if(painelMapa) painelMapa.SetActive(false);
        }

        // 2. Atualiza os cadeados e sprites
        AtualizarBotoesDoMapa();

        if (AudioManager.instance != null && musicaDoMenu != null)
        {
            AudioManager.instance.PlayMusic(musicaDoMenu);
        }
    }

    private void Update()
    {
        // Se apertar a tecla de Cheat (F1), libera tudo
        if (Input.GetKeyDown(teclaDebugCheat))
        {
            DebugDesbloquearTudo();
        }
    }

    void DebugDesbloquearTudo()
    {
        GameSession.FasesDesbloqueadas = 4; // Libera o total de botões que existirem
        Debug.Log("CHEAT ATIVADO: Todas as fases foram liberadas via Teclado!");
        AtualizarBotoesDoMapa(); // Atualiza o visual na hora
    }

    void AtualizarBotoesDoMapa()
        {
            if (botoesDasFases == null || botoesDasFases.Length == 0) return;

            for (int i = 0; i < botoesDasFases.Length; i++)
            {
                if (botoesDasFases[i] == null) continue;

                // 1. Limpa cliques antigos
                botoesDasFases[i].onClick.RemoveAllListeners();
                
                // 2. Pega componentes
                Image imagemDoBotao = botoesDasFases[i].GetComponent<Image>();
                int indice = i; // Truque do C# para o índice

                // --- LÓGICA DIVIDIDA ---

                if (i < GameSession.FasesDesbloqueadas)
                {
                    // === CENÁRIO: FASE DESBLOQUEADA ===
                    
                    // Botão clicável
                    botoesDasFases[i].interactable = true; 
                    
                    // Clique = Jogar a Fase
                    botoesDasFases[i].onClick.AddListener(() => TocarCutsceneFase(indice));

                    // Visual = Sprite da Fase
                    if (spritesDasFases != null && i < spritesDasFases.Count && spritesDasFases[i] != null)
                    {
                        imagemDoBotao.sprite = spritesDasFases[i];
                    }
                }
                else
                {
                    // === CENÁRIO: FASE BLOQUEADA ===
                    
                    // MUDANÇA AQUI: Botão continua CLICÁVEL (true) para podermos mostrar o aviso
                    botoesDasFases[i].interactable = true; 
                    
                    // Clique = Mostrar Aviso de Bloqueio
                    botoesDasFases[i].onClick.AddListener(MostrarAvisoBloqueado);

                    // Visual = Cadeado
                    if (spriteCadeado != null)
                    {
                        imagemDoBotao.sprite = spriteCadeado;
                    }
                }
            }
        }

    // --- NOVAS FUNÇÕES PARA O AVISO ---

    public void MostrarAvisoBloqueado()
    {
        if (painelAvisoBloqueado != null)
        {
            painelAvisoBloqueado.SetActive(true);
        }
    }

    public void FecharAvisoBloqueado()
    {
        if (painelAvisoBloqueado != null)
        {
            painelAvisoBloqueado.SetActive(false);
        }
    }

    // --- MANTENHA O RESTO DAS FUNÇÕES (Jogar, TocarVideo, etc) IGUAIS ---
    
    public void jogar() 
    {

        // 1. Reseta o progresso salvo permanentemente no computador
        PlayerPrefs.SetInt("NivelLiberado", 1);
        PlayerPrefs.Save(); // Garante que a gravação ocorra agora

        // 2. Reseta a variável da sessão atual para o mapa atualizar visualmente
        //GameSession.FasesDesbloqueadas = 1;

        cenaParaCarregar = string.Empty; 
        painelmenuinicial.SetActive(false);
        TocarVideo(introCutsceneClip);
    }

    public void TocarCutsceneFase(int indiceDaFase)
    {
        if (indiceDaFase < 0 || indiceDaFase >= nomesDasCenasDasFases.Count) return;

        // Só permite clicar se realmente estiver desbloqueado (Segurança extra)
        if (indiceDaFase >= GameSession.FasesDesbloqueadas) return;

        cenaParaCarregar = nomesDasCenasDasFases[indiceDaFase];
        painelMapa.SetActive(false);

        VideoClip clipeParaTocar = null;
        if (indiceDaFase < cutscenesDasFases.Count)
            clipeParaTocar = cutscenesDasFases[indiceDaFase];
        
        TocarVideo(clipeParaTocar);
    }

    // Copie aqui as funções VoltarParaMenu, TocarVideo, OnVideoEnd, CheckForSkipInput, PularCutscene, abriropcoes, fecharopcoes, fecharjogo do seu script anterior.
    // (Omiti para economizar espaço, mas elas não mudaram).
    
    public void VoltarParaMenu()
    {
        painelMapa.SetActive(false);
        painelmenuinicial.SetActive(true);
    }

    private void TocarVideo(VideoClip clip)
    {
        if (clip == null) { OnVideoEnd(videoPlayer); return; }
        videoPlayer.clip = clip;
        painelCutscene.SetActive(true);
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
        cutsceneTocando = true;
        StartCoroutine(CheckForSkipInput());
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        cutsceneTocando = false; 
        vp.loopPointReached -= OnVideoEnd;
        painelCutscene.SetActive(false);
        if (string.IsNullOrEmpty(cenaParaCarregar)) painelMapa.SetActive(true);
        else SceneManager.LoadScene(cenaParaCarregar);
    }

    private IEnumerator CheckForSkipInput()
    {
        while (cutsceneTocando) {
            if (Input.GetKeyDown(teclaParaPular)) { PularCutscene(); }
            yield return null;
        }
    }

    private void PularCutscene()
    {
        if (!cutsceneTocando) return;
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }
    public void abriropcoes() { painelopcoes.SetActive(true); }
    public void fecharopcoes() { painelopcoes.SetActive(false); painelmenuinicial.SetActive(true); }
    public void fecharjogo() { Application.Quit(); }
}