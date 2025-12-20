using UnityEngine;

[CreateAssetMenu(fileName = "NovoDocumento", menuName = "Sistema de Documentos/Novo Documento")]
public class DocumentData : ScriptableObject
{
    [Header("Conteúdo")]
    public string titulo; // Ex: "Diário de Bordo - Dia 1"
    
    [TextArea(5, 20)] 
    public string textoCompleto; // O texto longo do documento
    
    public Sprite imagemIlustrativa; // (Opcional) Se tiver desenho no papel
}