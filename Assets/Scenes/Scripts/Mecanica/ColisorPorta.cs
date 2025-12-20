/* using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Portas : MonoBehaviour
{

    protected bool podeInteragir = false;

    public Transform player;

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Key.Code("Enter"))
        {
            podeInteragir = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Key.Code("Enter"))
        {
            podeInteragir = false;
        }
    }

    void Start()
    {
        if (podeInteragir)
        {
            AtivarElementosPorta();
        }
    }

    void AtivarElementosPorta()
    {
        bttVerificar.gameObject.SetActive(true);
        caixaResposta.gameObject.SetActive(true);
    }
         
} */