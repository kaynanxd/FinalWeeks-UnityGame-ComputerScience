using UnityEngine;
using UnityEngine.UI; // Necessário para acessar o RawImage

public class EfeitoTraste : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Velocidade da rolagem. Negativo sobe, Positivo desce.")]
    public float velocidadeY = 0.5f; 
    
    private RawImage imagemDoTraste;
    private Rect uvRectAtual;

    void Start()
    {
        // Pega o componente RawImage que acabamos de colocar
        imagemDoTraste = GetComponent<RawImage>();

        if (imagemDoTraste == null)
        {
            Debug.LogError("ERRO: O objeto " + gameObject.name + " precisa ter um componente 'Raw Image'!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Se o jogo estiver pausado (TimeScale 0), não move
        if (Time.timeScale == 0) return;

        // Pega o retângulo atual da textura (UV Rect)
        uvRectAtual = imagemDoTraste.uvRect;

        // Move a coordenada Y baseado no tempo e velocidade
        uvRectAtual.y += velocidadeY * Time.deltaTime;

        // Aplica de volta ao componente
        imagemDoTraste.uvRect = uvRectAtual;
    }
}