using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScriptLuzes : MonoBehaviour
{
    [Header("Luzes")]
    public Light2D[] luzes;           // quantas quiser
    public GameObject[] objetos;      // mesmos índices das luzes

    [Header("Intervalo")]
    public float minIntervalo = 0.05f;
    public float maxIntervalo = 0.3f;

    [Header("Intensidade")]
    public float intensidadeMin = 0.2f;
    public float intensidadeMax = 1.5f;

    private float[] tempos;

    void Start()
    {
        tempos = new float[luzes.Length];

        for (int i = 0; i < luzes.Length; i++)
        {
            tempos[i] = Random.Range(minIntervalo, maxIntervalo);
            luzes[i].intensity = 0f;

            if (objetos.Length > i && objetos[i] != null)
                objetos[i].SetActive(false);
        }
    }

    void Update()
    {
        for (int i = 0; i < luzes.Length; i++)
        {
            tempos[i] -= Time.deltaTime;

            if (tempos[i] <= 0f)
            {
                bool ligada = Random.value > 0.4f;

                luzes[i].intensity = ligada
                    ? Random.Range(intensidadeMin, intensidadeMax)
                    : 0f;

                if (objetos.Length > i && objetos[i] != null)
                    objetos[i].SetActive(ligada);

                tempos[i] = Random.Range(minIntervalo, maxIntervalo);
            }
        }
    }
}
