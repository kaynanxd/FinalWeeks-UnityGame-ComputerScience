using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicaoLabirinto : TransicaoBase
{
    private Vector3 pontoRetorno;
    public float posicao;

    void Update()
    {
        if (!podeInteragir || cenaJaCarregando) return;

        else
        {
            Trocar();
        }

    }

    protected override void Trocar()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        pontoRetorno = playerObj.transform.position;
        Debug.Log(posicaoPlayer);
        // Guarda a cena atual
        string cenaAtual = SceneManager.GetActiveScene().name;

        // Troca de cena
        SceneManager.LoadScene(nomeCena);

        // Se estiver voltando pra Cena1, define posição relativa a entrada da cena anterior
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "Cena1")
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerObj.transform.position = new Vector3(pontoRetorno.x * posicao, pontoRetorno.y * posicao, -1f);
                }
            }
        };
    }

}