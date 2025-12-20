using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("0 para Fase 1, 1 para Fase 2, etc.")]
    public int idDaFaseDesteNivel; 

    public string nomeDaCenaDaProva = "CenaProva"; // Nome exato da cena da prova

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Grava no "bloco de notas" qual é a fase atual
            GameSession.FaseAtualIndex = idDaFaseDesteNivel;

            // 2. Carrega a cena única da prova
            SceneManager.LoadScene(nomeDaCenaDaProva);
        }
    }
}