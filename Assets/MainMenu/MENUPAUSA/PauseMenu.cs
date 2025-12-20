using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuOptions;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        pauseMenuOptions.SetActive(false); // Fecha opções se estiver aberto
        GameIsPaused = false;

        // --- A CORREÇÃO ESTÁ AQUI ---
        // Verifica se o DialogueManager existe E se o diálogo está ativo
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            // Se estiver em diálogo, mantemos o jogo pausado (TimeScale 0)
            Time.timeScale = 0f;
        }
        else
        {
            // Se NÃO estiver em diálogo, vida normal, despausa o jogo
            Time.timeScale = 1f;
        }
    }

    public void Home()
    {
        Time.timeScale = 1f; // Força despausa para mudar de cena
        GameIsPaused = false;
        SceneManager.LoadScene("MenuInicial");
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Força despausa para reiniciar
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Options()
    {
        pauseMenu.SetActive(false);
        pauseMenuOptions.SetActive(true);
    }

    public void OptionsClose()
    {
        pauseMenuOptions.SetActive(false);
        pauseMenu.SetActive(true);
    }
}