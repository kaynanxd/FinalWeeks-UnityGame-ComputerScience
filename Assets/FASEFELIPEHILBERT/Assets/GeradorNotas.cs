using UnityEngine;
using System;

[Serializable]
public class ConfiguracaoNota
{
    public string nome; // Mantenha "AND", "NAND", "OR", "XOR", "NOT"
    public GameObject prefab;
    public int indiceTrilha;
}

public class GeradorNotas : MonoBehaviour
{
    [Header("Configurações de Notas")]
    public ConfiguracaoNota[] notasConfiguradas;
    public Transform[] pontosNascimento;
    public Transform[] alvosAtivadores;

    [Header("Dificuldade e Probabilidade")]
    [Range(1f, 30f)]
    public float velocidadeNotas = 5f;

    [Range(0.2f, 5f)]
    public float intervalo = 2f;

    [Header("Probabilidade")]
    [Range(0, 100)]
    public float chanceDeVirNotaCorreta = 45f;

    // --- VARIÁVEIS DE CONTROLE ---
    float timer = 0;
    private string ultimaRegraVista = ""; 
    private bool jaGerouGarantiaMinima = false;

    void Update()
    {
        string regraAtualDoJogo = GeradorPortas.regraAtual; 

        if (regraAtualDoJogo != ultimaRegraVista)
        {
            ultimaRegraVista = regraAtualDoJogo;
            jaGerouGarantiaMinima = false; 
        }

        timer += Time.deltaTime;
        if (timer >= intervalo)
        {
            CriarNota();
            timer = 0;
        }
    }

    void CriarNota()
    {
        if (notasConfiguradas.Length == 0) return;

        int indiceSorteado = -1;
        bool tentarGerarCorreta = false;

        // --- 1. DECIDE SE VAI TENTAR GERAR A NOTA CORRETA ---
        if (!string.IsNullOrEmpty(ultimaRegraVista))
        {
            if (!jaGerouGarantiaMinima)
            {
                tentarGerarCorreta = true;
                jaGerouGarantiaMinima = true; 
            }
            else 
            {
                float dado = UnityEngine.Random.Range(0f, 100f);
                if (dado < chanceDeVirNotaCorreta) tentarGerarCorreta = true;
            }
        }

        // --- 2. PROCURA A NOTA CORRETA ---
        if (tentarGerarCorreta)
        {
            string regraMaiuscula = ultimaRegraVista.ToUpper().Trim(); // Ex: "AND_0", "XOR (3)"

            for (int i = 0; i < notasConfiguradas.Length; i++)
            {
                string nomeNotaConfigurada = notasConfiguradas[i].nome.ToUpper().Trim(); // Ex: "AND"
                bool encontrou = false;

                // ---------------------------------------------------------
                // LÓGICA ROBUSTA (Igual ao ativador.cs)
                // ---------------------------------------------------------

                if (nomeNotaConfigurada == "AND")
                {
                    // Contém AND, mas não contém NAND. 
                    // Funciona para: "AND", "AND_0", "AND_1", "AND (Copy)"
                    if (regraMaiuscula.Contains("AND") && !regraMaiuscula.Contains("NAND"))
                    {
                        encontrou = true;
                    }
                }
                else if (nomeNotaConfigurada == "OR")
                {
                    // Contém OR, mas não XOR
                    if (regraMaiuscula.Contains("OR") && !regraMaiuscula.Contains("XOR") && !regraMaiuscula.Contains("PORTA")) 
                    {
                        encontrou = true;
                    }
                    // Fallback se o nome for algo bizarro tipo "_OR_"
                    else if (regraMaiuscula == "OR")
                    {
                        encontrou = true;
                    }
                }
                else if (nomeNotaConfigurada == "NAND")
                {
                    if (regraMaiuscula.Contains("NAND")) encontrou = true;
                }
                else if (nomeNotaConfigurada == "XOR")
                {
                    if (regraMaiuscula.Contains("XOR")) encontrou = true;
                }
                else if (nomeNotaConfigurada == "NOT")
                {
                    if (regraMaiuscula.Contains("NOT")) encontrou = true;
                }

                // Se achou, salva o índice e para de procurar
                if (encontrou)
                {
                    indiceSorteado = i;
                    break;
                }
            }
        }

        // --- 3. SE NÃO ACHOU OU É PRA SER ALEATÓRIO ---
        if (indiceSorteado == -1)
        {
            indiceSorteado = UnityEngine.Random.Range(0, notasConfiguradas.Length);
        }

        // --- 4. GERA O OBJETO ---
        ConfiguracaoNota notaSorteada = notasConfiguradas[indiceSorteado];
        int trilha = notaSorteada.indiceTrilha;

        if (trilha >= pontosNascimento.Length || trilha >= alvosAtivadores.Length) return;

        GameObject novaNota = Instantiate(notaSorteada.prefab, pontosNascimento[trilha].position, Quaternion.identity);

        Movimento_Circulo script = novaNota.GetComponent<Movimento_Circulo>();
        if (script)
        {
            script.targetAtivador = alvosAtivadores[trilha];
            script.SetVelocidade(velocidadeNotas);
        }
    }
}