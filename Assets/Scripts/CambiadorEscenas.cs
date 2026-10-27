using UnityEngine;
using UnityEngine.SceneManagement; // Librería obligatoria para manejar escenas

public class CambiadorEscenas : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Escribe el nombre exacto de la escena a la que quieres ir")]
    public string nombreDeLaEscena;

    // Este es el método que ejecutará el botón
    public void Cambiar()
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}