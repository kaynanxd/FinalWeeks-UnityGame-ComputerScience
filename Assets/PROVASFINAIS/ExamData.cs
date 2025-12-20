using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NovaProva", menuName = "Sistema de Prova/Nova Prova")]
public class ExamData : ScriptableObject
{
    [Header("Configurações da Prova")]
    public string tituloDaProva;
    public int notaMinimaParaAprovar = 3; // Ex: acertar 3 de 5

    [Header("Lista de Questões")]
    public List<Question> questoes;
}

[System.Serializable]
public class Question
{
    [TextArea] public string enunciado;
    public string[] alternativas; // Deve ter 4 itens
    public int indiceCorreto; // 0, 1, 2 ou 3 (qual é a resposta certa)
}