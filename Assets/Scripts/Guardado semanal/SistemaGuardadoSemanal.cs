using UnityEngine;
using UnityEngine.SceneManagement;

public static class SistemaGuardadoSemanal
{
    private const string CLAVE_ESCENA = "Semana_Escena";
    private const string CLAVE_PANEL = "Semana_Panel";
    private const string CLAVE_PARTIDA = "Semana_PartidaGuardada";

    // Registrar el ultimo panel visitado.
    public static void GuardarCheckpoint(string escena, string panel)
    {
        if (string.IsNullOrEmpty(escena) || string.IsNullOrEmpty(panel))
            return;

        PlayerPrefs.SetString(CLAVE_ESCENA, escena);
        PlayerPrefs.SetString(CLAVE_PANEL, panel);
        PlayerPrefs.SetInt(CLAVE_PARTIDA, 1);

        PlayerPrefs.Save();

        Debug.Log("Checkpoint guardado: " + escena + " / " + panel);
    }

    // Comprobar si existe una partida.
    public static bool ExistePartida()
    {
        return PlayerPrefs.GetInt(CLAVE_PARTIDA, 0) == 1;
    }

    // Recuperar el nombre de la escena.
    public static string ObtenerEscenaGuardada()
    {
        return PlayerPrefs.GetString(CLAVE_ESCENA, "");
    }

    // Recuperar el nombre del panel.
    public static string ObtenerPanelGuardado()
    {
        return PlayerPrefs.GetString(CLAVE_PANEL, "");
    }

    // Comprobar si el checkpoint pertenece a esta escena.
    public static bool IntentarObtenerPanel(
        string escenaActual, out string panel)
    {
        panel = "";

        if (!ExistePartida())
            return false;

        if (ObtenerEscenaGuardada() != escenaActual)
            return false;

        panel = ObtenerPanelGuardado();
        return !string.IsNullOrEmpty(panel);
    }

    // Guardar todos los cambios pendientes.
    public static void GuardarTodo()
    {
        PlayerPrefs.Save();
    }

    // Eliminar solamente el checkpoint semanal.
    public static void EliminarCheckpoint()
    {
        PlayerPrefs.DeleteKey(CLAVE_ESCENA);
        PlayerPrefs.DeleteKey(CLAVE_PANEL);
        PlayerPrefs.DeleteKey(CLAVE_PARTIDA);
        PlayerPrefs.Save();
    }
}