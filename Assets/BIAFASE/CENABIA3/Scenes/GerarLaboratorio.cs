using UnityEngine;
using UnityEngine.UI; // Necessário para acessar o Text

public class GerarLaboratorio : MonoBehaviour
{
    [Header("Componente de Texto")]
    public Text textoUI; // Arraste o Text do Canvas aqui no Inspector

    [Header("Lista de Nomes")]
    public string[] nomes; // Preencha com os nomes que quer sortear

    void Start()
    {
        GerarNome(); // Gera um nome ao iniciar
    }

    // Função para gerar um nome aleatório
    public void GerarNome()
    {
        if (nomes.Length == 0)
        {
            textoUI.text = "Nenhum nome disponível!";
            return;
        }

        int indiceAleatorio = Random.Range(0, nomes.Length); // Escolhe um índice aleatório
        textoUI.text = nomes[indiceAleatorio]; // Define o texto no Canvas
    }
}
