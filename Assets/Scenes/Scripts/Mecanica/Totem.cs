using UnityEngine;

public class Totem : MonoBehaviour
{
    public TipoItem tipoAceito;

    [HideInInspector]
    public bool concluido = false;

    public void Depositar(ItemData item)
    {
        if (concluido)
            return;

        if (item.tipo == tipoAceito)
        {
            concluido = true;
            Debug.Log($"Totem {name} concluído!");
        }
    }
}
