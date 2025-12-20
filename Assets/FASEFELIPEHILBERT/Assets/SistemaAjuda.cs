using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SistemaAjuda : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject painelAjuda; 
    public TextMeshProUGUI textoExplicacao; 
    public Image imagemExemplo; 

    [Header("Imagens do Manual (Arraste aqui)")]
    public Sprite spriteAND;
    public Sprite spriteOR;
    public Sprite spriteNOT;
    public Sprite spriteNAND;
    public Sprite spriteXOR;

    private bool estaAberto = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJanela();
        }
    }

    public void ToggleJanela() 
    {
        estaAberto = !estaAberto;

        if (estaAberto) {
            Abrir();
        }
        else {
            Fechar();
        }
    }

    void Abrir() 
    {
        painelAjuda.SetActive(true);
        Time.timeScale = 0; 
        AtualizarConteudo();
    }

    void Fechar()
    {
        painelAjuda.SetActive(false);
        Time.timeScale = 1;
    }

    void AtualizarConteudo()
    {
        // Pega o nome da regra atual do outro script
        string regra = GeradorPortas.regraAtual;

        // Se por acaso a regra for nula, para o código para não dar erro
        if (string.IsNullOrEmpty(regra)) return;

        // --- LÓGICA DE TROCA DE TEXTO E IMAGEM ---

        // Nota: Verifique NAND antes de AND ou NOT para evitar confusão de nomes parecidos
        if (regra.Contains("NAND"))
        {
            textoExplicacao.text = "Porta NAND:\n\nO inverso do AND. Sai 0 só se todas as entradas forem 1.\n\n Voce deve apertar SOMENTE essa nota no momento que um icone chegar nela :";
            imagemExemplo.sprite = spriteNAND;
        }
        else if (regra.Contains("XOR"))
        {
            textoExplicacao.text = "Porta XOR:\n\nOu Exclusivo. Sai 1 apenas se as entradas forem diferentes.\n\n Voce deve apertar SOMENTE essa nota no momento que um icone chegar nela :";
            imagemExemplo.sprite = spriteXOR;
        }
        else if (regra.StartsWith("AND"))
        {
            textoExplicacao.text = "Porta AND (E):\n\nSó liga se TODAS as entradas forem 1.\nÉ a porta mais rigorosa!\n\n Voce deve apertar SOMENTE essa nota no momento que um icone chegar nela :";
            imagemExemplo.sprite = spriteAND;
        }
        else if (regra.StartsWith("OR"))
        {
            textoExplicacao.text = "Porta OR (OU):\n\nLiga se PELO MENOS UMA entrada for 1.\nÉ a porta mais flexível.\n\n Voce deve apertar SOMENTE essa nota no momento que um icone chegar nela :";
            imagemExemplo.sprite = spriteOR;
        }
        else if (regra.StartsWith("NOT"))
        {
            textoExplicacao.text = "Porta NOT (Não):\n\nInverte o sinal.\nSe entra 1, sai 0. Se entra 0, sai 1.\n\n Voce deve apertar SOMENTE essa nota no momento que um icone chegar nela :";
            imagemExemplo.sprite = spriteNOT;
        }
        else
        {
            // Caso genérico
            textoExplicacao.text = "Regra Atual: " + regra;
            imagemExemplo.sprite = null; // Ou uma imagem de interrogação
        }

        // Garante que a imagem não fique esticada
        if(imagemExemplo.sprite != null)
        {
            imagemExemplo.preserveAspect = true;
        }
    }
}