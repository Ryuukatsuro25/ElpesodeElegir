using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinuarPartidaSemanal : MonoBehaviour
{
    [Header("Boton Continuar")]
    [SerializeField] private GameObject botonContinuar;

    private void Start()
    {
        ActualizarBotonContinuar();
    }

    public void ActualizarBotonContinuar()
    {
        bool hayPartidaGuardada = SistemaGuardadoSemanal.ExistePartida();

        if (botonContinuar != null)
        {
            botonContinuar.SetActive(hayPartidaGuardada);
        }

        Debug.Log("¿Existe una partida guardada?: " + hayPartidaGuardada);
    }

    public void ContinuarPartida()
    {
        if (!SistemaGuardadoSemanal.ExistePartida())
        {
            Debug.LogWarning("No hay una partida guardada.");
            ActualizarBotonContinuar();
            return;
        }

        string escena = SistemaGuardadoSemanal.ObtenerEscenaGuardada();

        if (string.IsNullOrEmpty(escena))
        {
            Debug.LogError("No se encontro la escena guardada.");
            return;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(escena);
    }
}