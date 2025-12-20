using UnityEngine;

public enum TipoItem
{
    String,
    Int,
    Char,
    Float
}

[CreateAssetMenu(fileName = "Novo Item", menuName = "Itens/Item Data")]
public class ItemData : ScriptableObject


{
    public string id; 
    public string nomeDoItem;
    public Sprite icone;
    public TipoItem tipo;
}
