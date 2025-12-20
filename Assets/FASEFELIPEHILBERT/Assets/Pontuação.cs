using UnityEngine;
using TMPro;

public class Pontuação : MonoBehaviour
{
    public static Pontuação instance;

    public int pontuacaoAtual = 0;
    public int pontosPorNota = 100;

    public TextMeshProUGUI textoPlacar;
    public GameObject painelFinal;
    public TextMeshProUGUI textoFinal;

    [Header("Efeitos e Sons")]
    public AudioSource audioSource;
    public AudioClip somGanharPontos;
    public AudioClip somPerderPontos;
    public CameraShake cameraShake;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        AtualizarTexto();
        if (painelFinal != null) painelFinal.SetActive(false);
    }

    void Update()
    {
           // MostrarResultadoFinal(); 
    }

    public void GanharPontos()
    {
        pontuacaoAtual += pontosPorNota;
        AtualizarTexto();
        if (audioSource != null && somGanharPontos != null) audioSource.PlayOneShot(somGanharPontos);
    }

    public void PerderPontos()
    {
        pontuacaoAtual -= 100;
        if (pontuacaoAtual < 0) pontuacaoAtual = 0;
        AtualizarTexto();
        if (audioSource != null && somPerderPontos != null) audioSource.PlayOneShot(somPerderPontos);
        if (cameraShake != null) cameraShake.Tremer(0.2f, 0.3f);
    }

    void AtualizarTexto()
    {
        if (textoPlacar != null) textoPlacar.text = "Pontos: " + pontuacaoAtual;
    }

    public void MostrarResultadoFinal()
    {
        Time.timeScale = 0; 

        if (painelFinal != null)
        {
            painelFinal.SetActive(true);
            if (textoFinal != null)
            {
                textoFinal.text = "FIM DE JOGO\nPontos Totais: " + pontuacaoAtual;
            }
        }
    }

    // --- BOTÃO CONTINUAR MODIFICADO ---
    public void BotaoContinuar()
    {
        Debug.Log("Botão Continuar clicado. Verificando próxima etapa...");
        
        // Destrava o tempo para o diálogo funcionar
        Time.timeScale = 1f; 

        // Procura o GeradorPortas na cena
        GeradorPortas gerador = FindObjectOfType<GeradorPortas>();

        if (gerador != null)
        {
            // O Gerador decide se vai para o Lobby ou para a Prova
            gerador.ExecutarSequenciaFinal();
        }
        else
        {
            // Se não achar o gerador (segurança), vai pro Lobby direto
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }
    }
}