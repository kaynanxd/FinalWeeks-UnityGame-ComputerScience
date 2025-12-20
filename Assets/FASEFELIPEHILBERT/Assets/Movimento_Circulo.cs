using UnityEngine;

public class Movimento_Circulo : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public Transform targetAtivador;
    public float velocidade = 5f;

    [Header("Identificação da Nota")]
    public string tipoDaPorta;

    private Vector3 direcao;
    private bool direcaoDefinida = false;

    // Função que o GeradorNotas chama para injetar a velocidade da fase
    public void SetVelocidade(float novaVel)
    {
        velocidade = novaVel;
    }

    void Update()
    {
        // Se ainda não temos o alvo, não fazemos nada
        if (targetAtivador == null) return;

        // Na primeira vez que o Update roda, calculamos a direção exata para o alvo
        if (!direcaoDefinida)
        {
            // Calcula o vetor entre a nota e o alvo e o normaliza (tamanho 1)
            direcao = (targetAtivador.position - transform.position).normalized;
            direcaoDefinida = true;
        }

        // Move a nota continuamente naquela direção. 
        // Como não usamos MoveTowards, ela NÃO para quando chega no alvo.
        transform.position += direcao * velocidade * Time.deltaTime;
    }
}