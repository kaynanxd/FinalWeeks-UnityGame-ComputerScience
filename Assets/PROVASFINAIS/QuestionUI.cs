using UnityEngine;
using UnityEngine.UI;
using TMPro; // Se estiver usando TextMeshPro

public class QuestionUI : MonoBehaviour
{
    [Header("Elementos da UI")]
    public TextMeshProUGUI textoEnunciado;
    public Toggle[] togglesAlternativas; // Arraste os 4 toggles aqui
    public TextMeshProUGUI[] textosAlternativas; // Textos dos 4 toggles

    private int _indiceRespostaCorreta;

    // Configura a questão visualmente
    public void Setup(Question q)
    {
        textoEnunciado.text = q.enunciado;
        _indiceRespostaCorreta = q.indiceCorreto;

        for (int i = 0; i < togglesAlternativas.Length; i++)
        {
            // Define o texto de cada opção
            if (i < q.alternativas.Length)
                textosAlternativas[i].text = q.alternativas[i];
            
            togglesAlternativas[i].isOn = false; // Reseta seleção
        }
    }

    // Verifica se o jogador marcou a certa nesta questão específica
    public bool IsCorrect()
    {
        for (int i = 0; i < togglesAlternativas.Length; i++)
        {
            if (togglesAlternativas[i].isOn)
            {
                return i == _indiceRespostaCorreta;
            }
        }
        return false; // Nenhuma selecionada ou errada
    }
}