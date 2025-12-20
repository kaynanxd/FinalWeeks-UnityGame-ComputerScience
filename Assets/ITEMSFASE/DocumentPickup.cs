using UnityEngine;

public class DocumentPickup : MonoBehaviour
{
    [Header("Qual documento é este?")]
    public DocumentData documentData;

    [Header("Configuração")]
    public bool destroyOnPickup = true; // O papel some do chão ao pegar?

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (documentData != null)
            {
                // Manda o sistema coletar e ler
                DocumentSystem.Instance.CollectAndRead(documentData);

                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}