using UnityEngine;

public class BarrasPersistentes : MonoBehaviour
{
    public static BarrasPersistentes Instance;

    private void Awake()
    {
        // Patrón Singleton rápido y seguro
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        // Esto obliga a que, pase lo que pase con los paneles, 
        // las barras siempre se queden prendidas y al frente de todo.
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        transform.SetAsLastSibling();
    }
}