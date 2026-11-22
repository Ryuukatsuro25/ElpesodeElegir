
using UnityEngine;

public static class SistemaBienestarSemanal
{
    private const string CLAVE_BIENESTAR = "Semana_Bienestar";
    private const string CLAVE_DESMAYO = "Semana_DesmayoMostrado";

    private const float BIENESTAR_INICIAL = 50f;

    // Inicializar el bienestar solo si todavía no existe.
    public static void InicializarSiNoExiste()
    {
        if (!PlayerPrefs.HasKey(CLAVE_BIENESTAR))
        {
            PlayerPrefs.SetFloat(
                CLAVE_BIENESTAR,
                BIENESTAR_INICIAL
            );

            PlayerPrefs.SetInt(CLAVE_DESMAYO, 0);
            PlayerPrefs.Save();

            Debug.Log("Bienestar semanal iniciado en 50.");
        }
    }

    // Consultar el bienestar acumulado.
    public static float ObtenerBienestar()
    {
        InicializarSiNoExiste();

        return Mathf.Clamp(
            PlayerPrefs.GetFloat(CLAVE_BIENESTAR, BIENESTAR_INICIAL),
            0f,
            100f
        );
    }

    // Aplicar aumentos o disminuciones.
    public static float AplicarCambio(float cambio)
    {
        InicializarSiNoExiste();

        float bienestarActual = ObtenerBienestar();

        float nuevoBienestar = Mathf.Clamp(
            bienestarActual + cambio,
            0f,
            100f
        );

        PlayerPrefs.SetFloat(CLAVE_BIENESTAR, nuevoBienestar);
        PlayerPrefs.Save();

        Debug.Log(
            "Bienestar semanal actualizado: "
            + bienestarActual + " -> " + nuevoBienestar
        );

        return nuevoBienestar;
    }

    // Consultar si el bienestar llegó a cero.
    public static bool BienestarEnCero()
    {
        return ObtenerBienestar() <= 0f;
    }

    // Consultar si el desmayo ya fue mostrado.
    public static bool DesmayoYaMostrado()
    {
        return PlayerPrefs.GetInt(CLAVE_DESMAYO, 0) == 1;
    }

    // Registrar que la consecuencia ya fue presentada.
    public static void RegistrarDesmayo()
    {
        PlayerPrefs.SetInt(CLAVE_DESMAYO, 1);
        PlayerPrefs.Save();
    }

    // Reiniciar el bienestar para una partida nueva.
    public static void Reiniciar()
    {
        PlayerPrefs.SetFloat(CLAVE_BIENESTAR, BIENESTAR_INICIAL);
        PlayerPrefs.SetInt(CLAVE_DESMAYO, 0);
        PlayerPrefs.Save();

        Debug.Log("Bienestar semanal reiniciado en 50.");
    }
}
