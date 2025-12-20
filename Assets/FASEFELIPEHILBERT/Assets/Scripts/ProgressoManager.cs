using UnityEngine;

public class ProgressoManager : MonoBehaviour
{
    // static para que o valor não se perca entre as cenas
    public static int nivelMaximoLiberado = 1;

    void Awake()
    {
        // Se já existir um manager, destrói o novo. Se não, protege este.
        if (FindObjectsOfType<ProgressoManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Função para liberar a próxima fase
    public static void DesbloquearProximaFase(int faseAtual)
    {
        if (faseAtual >= nivelMaximoLiberado)
        {
            nivelMaximoLiberado = faseAtual + 1;
            Debug.Log("Nova fase desbloqueada! Nível máximo: " + nivelMaximoLiberado);
        }
    }
}