using UnityEngine;
using UnityEngine.UI; 
using System.Collections; 

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    private CanvasGroup canvasGroup;
    
    [Header("Configurações de Tempo")]
    public float fadeDuration = 0.5f; 
    public float blackHoldDuration = 1.0f; 
    
    public delegate void FadeCompleteAction();
    public static event FadeCompleteAction OnFadeOutComplete;
    
    public static FadeController Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Opcional
        }
        else
        {
            Destroy(gameObject);
        }
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Garante que o jogo comece jogável e transparente
        canvasGroup.blocksRaycasts = false;
        if (fadeImage != null)
        {
            Color initialColor = fadeImage.color;
            fadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); 
        }
    }

    public void StartRespawnFade()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // PASSO 1: PAUSA O JOGO e BLOQUEIA INPUTS
        Time.timeScale = 0f; // Pausa o jogo
        canvasGroup.blocksRaycasts = true; 
        
        float timer = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Alpha 1
        
        // --- INÍCIO DO FADE OUT (Escurecer) ---
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            fadeImage.color = Color.Lerp(startColor, targetColor, timer / fadeDuration);
            yield return null; 
        }

        fadeImage.color = targetColor; // Garante 100% preto

        // PASSO 2: PERÍODO DE PAUSA TOTALMENTE PRETA
        yield return new WaitForSecondsRealtime(blackHoldDuration); 

        // Notifica o PlayerRespawn para teleportar o jogador.
        OnFadeOutComplete?.Invoke();
    }
    
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // **********************************************
        // NOVO POSICIONAMENTO: DESPAUSA O JOGO AQUI
        // O jogo volta a rodar, mas o jogador ainda não vê nada.
        // **********************************************
        Time.timeScale = 1f; 
        
        float timer = 0f;
        Color startColor = fadeImage.color; // Começa de Preto (Alpha 1)
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Alpha 0

        // --- INÍCIO DO FADE IN (Clarear) ---
        while (timer < fadeDuration)
        {
            // Continuamos usando Time.unscaledDeltaTime para o Fade, pois ele é um efeito de UI
            // independente da velocidade do jogo, mas o jogo já está rodando em segundo plano.
            timer += Time.unscaledDeltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, timer / fadeDuration);
            yield return null;
        }

        fadeImage.color = targetColor; 
        
        // PASSO 3: LIBERA INPUTS
        // Depois de clarear totalmente, o raycast é desativado para liberar o clique.
        canvasGroup.blocksRaycasts = false; 
    }
}