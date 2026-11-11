
using UnityEngine;
using UnityEngine.SceneManagement;

public class NuevaPartidaSemanal : MonoBehaviour
{
    [Header("Escena inicial")]
    [SerializeField] private string escenaInicio = "PrimeraDecisionDomingo";

    private bool iniciandoPartida = false;

    public void IniciarNuevaPartida()
    {
        if (iniciandoPartida)
            return;

        iniciandoPartida = true;

        Debug.Log("Iniciando nueva partida...");

        // Restablecer el tiempo.
        Time.timeScale = 1f;

        // Eliminar checkpoint semanal.
        SistemaGuardadoSemanal.EliminarCheckpoint();

        SistemaBienestarSemanal.Reiniciar();

        // Eliminar progreso del miercoles.
        PlayerPrefs.DeleteKey("Miercoles_Bienestar");
        PlayerPrefs.DeleteKey("Miercoles_AyudaRegistrada");
        PlayerPrefs.DeleteKey("Miercoles_NoPreguntarRegistrada");
        PlayerPrefs.DeleteKey("Miercoles_DecisionTomada");
        PlayerPrefs.DeleteKey("Miercoles_EstadoGuardado");

        // Eliminar progreso del viernes.
        PlayerPrefs.DeleteKey("Viernes_Bienestar");
        PlayerPrefs.DeleteKey("Viernes_Decision");
        PlayerPrefs.DeleteKey("Viernes_DecisionRegistrada");
        PlayerPrefs.DeleteKey("Viernes_PanelActual");
        PlayerPrefs.DeleteKey("Viernes_ProgresoGuardado");

        PlayerPrefs.Save();

        // Iniciar desde el domingo.
        SceneManager.LoadScene(escenaInicio);
    }
}
