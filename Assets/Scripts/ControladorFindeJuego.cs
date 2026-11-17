
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorFinDeJuego : MonoBehaviour
{
    public void NuevaPartida()
    {
        // Reiniciar el bienestar semanal.
        SistemaBienestarSemanal.Reiniciar();

        // Eliminar el checkpoint anterior.
        SistemaGuardadoSemanal.EliminarCheckpoint();

        // Comenzar desde el domingo.
        SceneManager.LoadScene("PrimeraDecisionDomingo");
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("menu 1");
    }
}
