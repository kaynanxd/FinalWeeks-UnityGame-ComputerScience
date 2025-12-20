using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 deslocamento;
    private float velocidade = 0.2f;
    void Start()
    {
        // o deslocamento sera a diferenca entre a posicao da camera e do player
        deslocamento = transform.position - player.transform.position;
    }

    // o LateUpdate esta sendo usado para atualizar apos o deslocamento ser executado
    void LateUpdate()
    {
        // a posicao da camera agr sera a posicao atual do player com o deslocamento
        transform.position = (player.transform.position + deslocamento) * velocidade;
    }
}
