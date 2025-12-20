using UnityEngine;

public class ObjectFloater : MonoBehaviour
{
    public float amplitude = 0.5f; // Altura do movimento
    public float frequency = 1f;   // Velocidade do movimento

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Usa Seno para criar um movimento suave de sobe e desce
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}