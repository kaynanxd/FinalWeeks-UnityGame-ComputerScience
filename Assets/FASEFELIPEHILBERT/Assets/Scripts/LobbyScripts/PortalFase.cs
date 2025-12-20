using UnityEngine;
using UnityEngine.SceneManagement; 

public class PortalFase : MonoBehaviour
{
    [Header("Configurações da Fase")]
    public string nomeDaCena;     
    public int nivelNecessario;   // Qual nível o player precisa ter alcançado (Ex: 2 para entrar na fase 2)

    [Header("Feedback Visual")]
    public Sprite spriteBloqueado; // ARRASTE AQUI A IMAGEM (Cadeado/Porta Fechada)
    public Sprite spriteLiberado;  // ARRASTE AQUI A IMAGEM (Porta Aberta/Brilhando)
    
    private SpriteRenderer sr;
    private bool estaBloqueada;   // Variável interna para controle

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        AtualizarStatus();
    }

    public void AtualizarStatus()
    {
        // 1. Busca o progresso salvo
        int progressoSalvo = PlayerPrefs.GetInt("NivelLiberado", 1);

        // 2. Verifica se esta porta deve estar bloqueada
        // Se o meu progresso é menor que o necessário, está bloqueada.
        if (progressoSalvo < nivelNecessario)
        {
            estaBloqueada = true;
        }
        else
        {
            estaBloqueada = false;
        }

        // 3. Troca o Sprite (A Imagem) ao invés da cor
        if (sr != null)
        {
            if (estaBloqueada)
            {
                sr.sprite = spriteBloqueado; // Mostra imagem de bloqueado
            }
            else
            {
                sr.sprite = spriteLiberado;  // Mostra imagem de liberado
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Se estiver bloqueada, avisa e não deixa entrar
            if (estaBloqueada)
            {
                Debug.Log("Fase bloqueada! Complete a fase anterior primeiro.");
                return; 
            }

            if (string.IsNullOrEmpty(nomeDaCena))
            {
                Debug.LogError("ERRO: O nome da cena não foi preenchido no Inspector!");
                return;
            }

            Debug.Log("Entrando na fase: " + nomeDaCena);
            SceneManager.LoadScene(nomeDaCena);
        }
    }
}