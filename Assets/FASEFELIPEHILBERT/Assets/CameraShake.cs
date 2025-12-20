using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Função pública que outros scripts podem chamar
    public void Tremer(float duracao, float forca)
    {
        StartCoroutine(ProcessoTremer(duracao, forca));
    }

    IEnumerator ProcessoTremer(float duracao, float forca)
    {
        Vector3 posicaoOriginal = transform.localPosition;
        float tempoDecorrido = 0.0f;

        while (tempoDecorrido < duracao)
        {
            // Gera uma posição aleatória perto da original
            float x = Random.Range(-1f, 1f) * forca;
            float y = Random.Range(-1f, 1f) * forca;

            transform.localPosition = new Vector3(posicaoOriginal.x + x, posicaoOriginal.y + y, posicaoOriginal.z);

            tempoDecorrido += Time.deltaTime;

            yield return null; // Espera o próximo frame
        }

        // Garante que a câmera volte para o lugar certo no final
        transform.localPosition = posicaoOriginal;
    }
}