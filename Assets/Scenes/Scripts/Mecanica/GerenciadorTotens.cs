using UnityEngine;

public class GerenciadorTotens : MonoBehaviour
{
    public Totem[] totens;
    public GameObject objetoFinal; // o GameObject que será ativado

    void Start()
    {
        objetoFinal.SetActive(false);
    }

    public void VerificarTotens()
    {
        foreach (Totem t in totens)
        {
            if (!t.concluido)
                return; // ainda falta algum
        }

        // TODOS os totens concluídos
        objetoFinal.SetActive(true);
        Debug.Log("Todos os totens concluídos! Objeto final ativado.");
    }
}
