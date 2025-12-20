using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Importante para buscar os IDs

[CreateAssetMenu(fileName = "NovaConversa", menuName = "Sistema de Dialogo/Conversa Completa")]
public class DialogueData : ScriptableObject
{
    [Header("Configuração Inicial")]
    public string startNodeID = "inicio"; // Qual o nome do primeiro bloco?

    [Header("Todos os Blocos da Conversa")]
    public List<DialogueNode> nodes;

    // Função auxiliar para achar um nó pelo nome
    public DialogueNode GetNode(string id)
    {
        return nodes.FirstOrDefault(n => n.nodeID == id);
    }
}

[System.Serializable]
public class DialogueNode
{
    [Header("Identificador do Bloco (Ex: 'inicio', 'aceitou_missao')")]
    public string nodeID; 
    
    public List<Sentence> sentences;
    public List<Choice> choices;
}

[System.Serializable]
public struct Sentence
{
    public string characterName;
    [TextArea(3, 10)] public string text;
    public Sprite portrait;
    public AudioClip voiceSound;
}

[System.Serializable]
public struct Choice
{
    public string text;
    
    [Header("Para onde ir?")]
    public string targetNodeID; // Nome do bloco dentro DESTE arquivo
    
    [Header("Ou mudar de arquivo? (Opcional)")]
    public DialogueData externalDialogue; // Caso queira pular para outra cena/arquivo
}