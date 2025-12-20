using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody2D rb;

    private float inputHorizontal;
    private float inputVertical;

    void Start() // funcao que inicializa o script
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() // funcao que captura as entradas
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate() // funcao que aplica o movimento fisico
    {
        MovimentarPersonagem();
    }

    void MovimentarPersonagem() // funcao de movimentacao
    {
        Vector2 direcao = new Vector2(inputHorizontal, inputVertical).normalized;
        rb.linearVelocity = direcao * velocidade;

        if (inputHorizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputHorizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}