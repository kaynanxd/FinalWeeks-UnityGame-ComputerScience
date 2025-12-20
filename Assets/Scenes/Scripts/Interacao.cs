using UnityEngine;
using UnityEngine.UI;

public class DialogoSimples : MonoBehaviour
{
    [Header("Personagem")]
    public string nomePersonagem;
    public Sprite spritePersonagem;

    [Header("UI")]
    public GameObject balaoFala;
    public Text textoDialogo;
    public Text textoNome;
    public Image imagemPersonagem;

    [Header("Mensagens")]
    [TextArea(3, 5)]
    public string[] mensagens;

    private int indexMensagem = 0;
    private bool dialogoAtivo = false;
    private bool playerNoTrigger = false;

    void Start()
    {
        FecharDialogo();
    }

    void Update()
    {
        // Só avança se o diálogo estiver ativo
        if (dialogoAtivo && Input.GetKeyDown(KeyCode.Return))
        {
            ProximaMensagem();
        }
    }

    // PLAYER ENTROU NO TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !dialogoAtivo)
        {
            playerNoTrigger = true;
            IniciarDialogo();
        }
    }

    // PLAYER SAIU DO TRIGGER
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNoTrigger = false;
            FecharDialogo();
        }
    }

    private void IniciarDialogo()
    {
        if (mensagens.Length == 0) return;

        dialogoAtivo = true;
        indexMensagem = 0;

        textoNome.text = nomePersonagem;
        imagemPersonagem.sprite = spritePersonagem;

        balaoFala.SetActive(true);
        textoDialogo.gameObject.SetActive(true);
        textoNome.gameObject.SetActive(true);
        imagemPersonagem.gameObject.SetActive(true);

        textoDialogo.text = mensagens[indexMensagem];
    }

    private void ProximaMensagem()
    {
        indexMensagem++;

        if (indexMensagem >= mensagens.Length)
        {
            FecharDialogo();
            return;
        }

        textoDialogo.text = mensagens[indexMensagem];
    }

    private void FecharDialogo()
    {
        dialogoAtivo = false;

        balaoFala.SetActive(false);
        textoDialogo.gameObject.SetActive(false);
        textoNome.gameObject.SetActive(false);
        imagemPersonagem.gameObject.SetActive(false);
    }
}
