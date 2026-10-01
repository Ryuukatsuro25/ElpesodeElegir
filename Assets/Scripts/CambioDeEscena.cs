using UnityEngine;
using UnityEngine.SceneManagement; // Librería obligatoria para manejar escenas

public class CambioDeEscena : MonoBehaviour
{
    // Al poner (string nombreDeLaEscena) entre los paréntesis, 
    // Unity creará una casilla de texto en el Inspector del botón.
    public void CargarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}