using UnityEngine;

[CreateAssetMenu(fileName = "NovaDica", menuName = "Sistema de Dicas/Nova Dica")]
public class HintData : ScriptableObject
{
    [Header("Conteúdo")]
    public string titulo; 
    
    [TextArea(3, 10)]
    public string texto;

    [Header("Visual")]
    public Sprite imagem; // <--- NOVO CAMPO
}