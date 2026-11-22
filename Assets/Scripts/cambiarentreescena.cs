using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiarentreescena : MonoBehaviour
{
    // Al poner (string nombreEscena) dentro del paréntesis, Unity habilita el cuadro de texto en el botón
    public void CambiarEscena(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("No escribiste el nombre de la escena en el botón.");
        }
    }
}