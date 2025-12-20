using UnityEngine;
using UnityEngine.UI; // Necessário para mexer com Image
using TMPro;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance;

    [Header("UI Components")]
    public GameObject hintPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image hintImage; // <--- ARRASTE A IMAGEM DA UI AQUI

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if(hintPanel) hintPanel.SetActive(false);
    }

    public void ShowHint(HintData data)
    {
        // 1. Preenche os textos
        if (titleText) titleText.text = data.titulo;
        if (bodyText) bodyText.text = data.texto;

        // 2. Lógica da Imagem (NOVO)
        if (hintImage != null)
        {
            if (data.imagem != null)
            {
                // Se tem imagem: Ativa o objeto e troca o sprite
                hintImage.sprite = data.imagem;
                hintImage.gameObject.SetActive(true);
                
                // Dica: Isso evita que a imagem fique esticada/deformada
                hintImage.preserveAspect = true; 
            }
            else
            {
                // Se NÃO tem imagem: Desativa para não ficar um quadrado branco
                hintImage.gameObject.SetActive(false);
            }
        }

        // 3. Mostra a tela e Pausa
        hintPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseHint()
    {
        hintPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}