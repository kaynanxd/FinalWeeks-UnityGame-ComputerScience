using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class TrocarCena : MonoBehaviour
{
    [Header("Nome da Cena de Destino")]
    public string nomeCena; // Nome da cena para onde será trocado

    void Update()
    {
        // Verifica se o jogador apertou Enter (Return)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Troca para a cena especificada
            SceneManager.LoadScene(nomeCena);
        }
    }
}
