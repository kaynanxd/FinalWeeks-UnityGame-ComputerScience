using UnityEngine;

public class ControlePause : MonoBehaviour
{
    public GameObject menuPause; // Arraste seu painel de pause aqui no Inspector
    private bool jogoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (jogoPausado) Retomar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        menuPause.SetActive(true);
        Time.timeScale = 0f; // Congela tudo: física, tempo e animações
        jogoPausado = true;
    }

    public void Retomar()
    {
        menuPause.SetActive(false);
        Time.timeScale = 1f; // Volta o tempo ao normal
        jogoPausado = false;
    }
}