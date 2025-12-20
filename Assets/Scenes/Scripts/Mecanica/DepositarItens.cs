using UnityEngine;

public class DepositarItens : MonoBehaviour
{
    public KeyCode teclaDepositar = KeyCode.Return;

    private bool podeDepositar;
    private Totem totemAtual;

    void Update()
    {
        if (podeDepositar && Input.GetKeyDown(teclaDepositar))
        {
            DepositarItem();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Totem t = other.GetComponent<Totem>();
        if (t != null)
        {
            podeDepositar = true;
            totemAtual = t;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Totem>())
        {
            podeDepositar = false;
            totemAtual = null;
        }
    }

    void DepositarItem()
{
    if (totemAtual == null)
        return;

    ItemData item = Inventario.Instancia.PegarItemParaDepositar(totemAtual.tipoAceito);

    if (item == null)
    {
        Debug.Log("Nenhum item compatível.");
        return;
    }

    // Deposita no totem
    totemAtual.Depositar(item);

    // Remove do inventário
    Inventario.Instancia.RemoverItem(item);

    // Verifica todos os totens
    FindObjectOfType<GerenciadorTotens>().VerificarTotens();
}

}
