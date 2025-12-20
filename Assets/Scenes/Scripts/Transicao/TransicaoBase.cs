using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class TransicaoBase : MonoBehaviour 
{
    protected bool podeInteragir = false;
    public string nomeCena;

    protected bool cenaJaCarregando = false;
    public GameObject telaCarregamento; // opcional

    public Transform player;
    protected Vector3 posicaoPlayer;

    protected virtual void LateUpdate()
    {
        if (podeInteragir && !cenaJaCarregando)
        {
            Trocar(); // chamada ao método abstrato implementado na classe filha
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            podeInteragir = false;
        }
    }

    protected void TrocarCena(string nomeCena)
    {
        if (cenaJaCarregando) return;
        cenaJaCarregando = true;

        if (telaCarregamento != null)
        {
            telaCarregamento.SetActive(true);
        }

        SceneManager.LoadScene(nomeCena);
    }

    protected abstract void Trocar();
}
