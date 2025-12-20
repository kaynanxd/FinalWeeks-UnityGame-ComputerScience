using UnityEngine;
using System.Collections;

public class LightFade : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Configuração do Fade")]
    public float fadeDuration = 0.8f; // Duração total do efeito
    public float maxAlpha = 0.8f;     // Opacidade máxima que a luz atinge
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("LightFade requer um SpriteRenderer no objeto.");
            Destroy(gameObject);
            return;
        }

        // Começa a rotina de Fade
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        float timer = 0f;
        
        // FASE 1: APARECER (Rápido, 10% da duração total)
        float fadeInTime = fadeDuration * 0.1f;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, timer / fadeInTime);
            SetAlpha(alpha);
            yield return null;
        }

        // Garante que a opacidade máxima foi atingida
        SetAlpha(maxAlpha); 
        
        // FASE 2: DESAPARECER (Lento, 90% da duração total)
        float fadeOutTime = fadeDuration * 0.9f;
        timer = 0f;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            // Reduz do maxAlpha até 0
            float alpha = Mathf.Lerp(maxAlpha, 0f, timer / fadeOutTime);
            SetAlpha(alpha);
            yield return null;
        }

        // Destroi o objeto depois que o efeito terminar
        Destroy(gameObject);
    }

    void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}