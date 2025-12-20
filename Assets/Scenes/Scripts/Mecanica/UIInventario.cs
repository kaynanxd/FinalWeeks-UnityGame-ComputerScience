using UnityEngine;
using UnityEngine.UI;

public class UIInventario : MonoBehaviour
{
    public Transform[] slots;

    void Start()
    {
        if (Inventario.Instancia != null)
        {
            Inventario.Instancia.OnInventarioAtualizado += AtualizarUI;
            AtualizarUI();
        }
        else
        {
            Debug.LogError("Inventario não encontrado!");
        }
    }

    void OnDestroy()
    {
        if (Inventario.Instancia != null)
            Inventario.Instancia.OnInventarioAtualizado -= AtualizarUI;
    }

    void AtualizarUI()
    {
        var itens = Inventario.Instancia.itens;

        for (int i = 0; i < slots.Length; i++)
        {
            foreach (Transform child in slots[i])
                Destroy(child.gameObject);

            if (i < itens.Count && itens[i] != null)
            {
                CriarIcone(slots[i], itens[i]);
            }
        }
    }

    void CriarIcone(Transform slot, ItemData item)
    {
        GameObject icone = new GameObject("Icone");
        icone.transform.SetParent(slot, false);

        Image img = icone.AddComponent<Image>();
        img.sprite = item.icone;
        img.preserveAspect = true;

        RectTransform rect = img.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
