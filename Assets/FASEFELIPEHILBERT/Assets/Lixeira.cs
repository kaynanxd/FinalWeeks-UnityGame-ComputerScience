using UnityEngine;

public class Lixeira : MonoBehaviour
{
    // Quando algo entra na lixeira...
    void OnTriggerEnter2D(Collider2D col)
    {
        // Verifica se é uma nota (para não destruir o cenário sem querer)
        if (col.gameObject.tag == "note")
        {
            // Opcional: Aqui é o lugar perfeito para tirar vida!
            // GameManager.instance.PerderVida();
            
            Debug.Log("Nota perdida! Destruindo objeto.");
            Destroy(col.gameObject);
        }
    }
}