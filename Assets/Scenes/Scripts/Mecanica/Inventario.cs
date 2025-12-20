using UnityEngine;
using System;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    public static Inventario Instancia;

    public List<ItemData> itens = new();

    public event Action OnInventarioAtualizado;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AdicionarItem(ItemData item)
    {
        itens.Add(item);
        OnInventarioAtualizado?.Invoke();
    }

    public void RemoverItem(ItemData item)
    {
        if (itens.Remove(item))
        {
            OnInventarioAtualizado?.Invoke();
        }
    }

    // 🔥 Retorna automaticamente o item que será usado
    public ItemData PegarItemParaDepositar(TipoItem tipoAceito)
    {
        foreach (var item in itens)
        {
            if (item.tipo == tipoAceito)
                return item;
        }
        return null;
    }
}
