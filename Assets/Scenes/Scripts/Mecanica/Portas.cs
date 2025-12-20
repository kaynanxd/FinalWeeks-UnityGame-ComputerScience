using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Portas : MonoBehaviour
{
    [Header("Porta e UI")]
    public GameObject porta; 
    public GameObject portaAberta;
    public InputField caixaResposta;
    public Button bttVerificar;

    [Header("Luzes")]
    public Light2D[] spotLights;
    public float tempoPiscando = 0.5f;
    public bool[] alternarCoresPorLuz;

    [Header("Elementos de cor")]
    public Image[] elementosCor;

    [Header("Resposta")]
    public string[] comparacoes; // será preenchido automaticamente
    public string operador = "&&"; // && ou ||, definido no Inspector

    public GameObject passagemTransicao;
    private bool podeInteragir = false;

    private Color[] corFixa;
    private Color[][] coresAlternadas;

    void Start()
    {
        if (bttVerificar != null) bttVerificar.gameObject.SetActive(false);
        if (caixaResposta != null) caixaResposta.gameObject.SetActive(false);
        if (portaAberta != null) portaAberta.SetActive(false);
        if (passagemTransicao != null) passagemTransicao.SetActive(false);

        int nLuzes = spotLights.Length;
        corFixa = new Color[nLuzes];
        coresAlternadas = new Color[nLuzes][];
        comparacoes = new string[nLuzes];

        for (int i = 0; i < nLuzes; i++)
        {
            if (alternarCoresPorLuz != null && alternarCoresPorLuz.Length > i && alternarCoresPorLuz[i])
            {
                // Alterna entre duas cores aleatórias
                coresAlternadas[i] = new Color[2];
                coresAlternadas[i][0] = GerarCor();
                coresAlternadas[i][1] = GerarCor();

                // Atualiza elemento visual
                if (elementosCor != null && elementosCor.Length > i && elementosCor[i] != null)
                    elementosCor[i].color = coresAlternadas[i][0];

                if (spotLights[i] != null)
                    StartCoroutine(PiscarLuzAlternando(spotLights[i], coresAlternadas[i], elementosCor != null && elementosCor.Length > i ? elementosCor[i] : null));

                comparacoes[i] = $"cor{i+1}=={NomeCor(coresAlternadas[i][0])}";
            }
            else
            {
                // Cor fixa aleatória
                corFixa[i] = GerarCor();

                if (elementosCor != null && elementosCor.Length > i && elementosCor[i] != null)
                    elementosCor[i].color = corFixa[i];

                if (spotLights[i] != null)
                    StartCoroutine(PiscarLuzFixa(spotLights[i], corFixa[i]));

                comparacoes[i] = $"cor{i+1}=={NomeCor(corFixa[i])}";
            }
        }
    }

    Color GerarCor()
    {
        Color[] cores = { Color.red, Color.green, Color.yellow };
        return cores[Random.Range(0, cores.Length)];
    }

    string NomeCor(Color c)
    {
        if (c == Color.red) return "vermelho";
        if (c == Color.green) return "verde";
        if (c == Color.yellow) return "amarelo";
        return "desconhecido";
    }

    IEnumerator PiscarLuzFixa(Light2D luz, Color cor)
    {
        while (true)
        {
            if (luz != null)
            {
                luz.color = cor;
                luz.enabled = true;
            }
            yield return new WaitForSeconds(tempoPiscando);
            if (luz != null) luz.enabled = false;
            yield return new WaitForSeconds(tempoPiscando);
        }
    }

    IEnumerator PiscarLuzAlternando(Light2D luz, Color[] cores, Image elemento = null)
    {
        int index = 0;
        while (true)
        {
            if (luz != null)
            {
                luz.color = cores[index];
                if (elemento != null) elemento.color = cores[index];
                luz.enabled = true;
            }
            yield return new WaitForSeconds(tempoPiscando);
            if (luz != null) luz.enabled = false;
            yield return new WaitForSeconds(tempoPiscando);

            index = (index + 1) % cores.Length;
        }
    }

    public void OnClickVerificar()
    {
        if (!podeInteragir || caixaResposta == null) return;

        string respostaJogador = caixaResposta.text.Trim().Replace(" ", "").ToLower();
        bool acertou = true;

        foreach (string comp in comparacoes)
        {
            if (!respostaJogador.Contains(comp.Replace(" ", "").ToLower()))
            {
                acertou = false;
                break;
            }
        }

        if (!string.IsNullOrEmpty(operador) && !respostaJogador.Contains(operador.Replace(" ", "")))
            acertou = false;

        if (!respostaJogador.Contains("abrirporta()"))
            acertou = false;

        if (acertou)
            AbrirPorta();
        else
            FecharPorta();
    }

    void AbrirPorta()
    {
        if (porta != null) porta.SetActive(false);
        if (portaAberta != null) portaAberta.SetActive(true);

        if (bttVerificar != null) bttVerificar.gameObject.SetActive(false);
        if (caixaResposta != null) caixaResposta.gameObject.SetActive(false);
        if (passagemTransicao != null) passagemTransicao.SetActive(true);

        Debug.Log("Porta aberta com sucesso!");
    }

    void FecharPorta()
    {
        Debug.Log("Resposta incorreta, porta continua fechada!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
            if (bttVerificar != null) bttVerificar.gameObject.SetActive(true);
            if (caixaResposta != null) caixaResposta.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
            if (bttVerificar != null) bttVerificar.gameObject.SetActive(false);
            if (caixaResposta != null) caixaResposta.gameObject.SetActive(false);
        }
    }

    public bool PortaAberta()
    {
        return portaAberta != null && portaAberta.activeSelf;
    }
}
