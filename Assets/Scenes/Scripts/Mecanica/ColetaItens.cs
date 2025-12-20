using UnityEngine;

public class ColetaItens : MonoBehaviour
{
    [Header("Configuração")]
    public KeyCode teclaColetar = KeyCode.E;
    public KeyCode teclaFecharPopup = KeyCode.Return; // ENTER

    [Header("Pop-up (Canvas)")]
    public GameObject popupCanvas;

    private bool podeColetar;
    private GameObject itemFisicoAtual;

    void Start()
    {
        if (popupCanvas != null)
            popupCanvas.SetActive(false);
    }

    void Update()
    {
        // Fecha o pop-up com ENTER
        if (popupCanvas != null && popupCanvas.activeSelf)
        {
            if (Input.GetKeyDown(teclaFecharPopup))
            {
                FecharPopup();
            }

            // Bloqueia coleta enquanto o pop-up está aberto
            return;
        }

        if (podeColetar && Input.GetKeyDown(teclaColetar))
        {
            ColetarItem();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Item>())
        {
            podeColetar = true;
            itemFisicoAtual = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Item>())
        {
            podeColetar = false;
            itemFisicoAtual = null;
        }
    }

    void ColetarItem()
    {
        if (itemFisicoAtual == null) return;

        Item item = itemFisicoAtual.GetComponent<Item>();
        if (item == null || item.data == null) return;

        Inventario.Instancia.AdicionarItem(item.data);

        // Abre o pop-up
        if (popupCanvas != null)
            popupCanvas.SetActive(true);

        Destroy(itemFisicoAtual);

        podeColetar = false;
        itemFisicoAtual = null;
    }

    public void FecharPopup()
    {
        if (popupCanvas != null)
            popupCanvas.SetActive(false);
    }
}