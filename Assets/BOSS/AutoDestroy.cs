using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    public float delay = 0.5f; // Tempo da animação
    void Start() { Destroy(gameObject, delay); }
}