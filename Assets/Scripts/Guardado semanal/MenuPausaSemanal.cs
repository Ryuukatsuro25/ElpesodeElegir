
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausaSemanal : MonoBehaviour
{
    [Header("Panel de pausa")]
    [SerializeField] private GameObject panelPausa;

    [Header("Configuracion")]
    [SerializeField] private string escenaMenuPrincipal = "MainMenu";
    [SerializeField] private bool cerrarAlIniciar = true;

    [Header("Guardado de cada dia")]
    [SerializeField] private GestorPanelesLunes gestorLunes;
    [SerializeField] private GestorPanelesMiercoles gestorMiercoles;
    [SerializeField] private GestorPanelesViernes gestorViernes;

    private bool juegoPausado = false;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (cerrarAlIniciar && panelPausa != null)
            panelPausa.SetActive(false);

        juegoPausado = false;
    }

    public void AbrirPanel()
    {
        if (panelPausa == null) return;

        panelPausa.SetActive(true);
        panelPausa.transform.SetAsLastSibling();

        Time.timeScale = 0f;
        juegoPausado = true;

        Debug.Log("Menu de pausa abierto");
    }

    public void CerrarPanel()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        Time.timeScale = 1f;
        juegoPausado = false;

        Debug.Log("Menu de pausa cerrado");
    }

    public void AlternarPausa()
    {
        if (juegoPausado)
            CerrarPanel();
        else
            AbrirPanel();
    }

    public void ContinuarJuego()
    {
        CerrarPanel();
    }

    public void GuardarYSalir()
    {
        bool progresoGuardado = false;

    // LUNES
    if (gestorLunes != null)
    {
        gestorLunes.GuardarProgreso();
        progresoGuardado = true;
    }

    // MIERCOLES
    else if (gestorMiercoles != null)
    {
        gestorMiercoles.GuardarProgreso();
        progresoGuardado = true;
    }

    // VIERNES
    else if (gestorViernes != null)
    {
        gestorViernes.GuardarProgreso();
        progresoGuardado = true;
    }

    if (!progresoGuardado)
    {
        Debug.LogError(
            "No se encontro el gestor de progreso de esta escena."
        );
        return;
    }

    SistemaGuardadoSemanal.GuardarTodo();

    Debug.Log("Progreso guardado correctamente.");

    Time.timeScale = 1f;
    juegoPausado = false;

    SceneManager.LoadScene(escenaMenuPrincipal);
    }

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;

        SceneManager.LoadScene(escenaMenuPrincipal);
    }

    public void ComprobarEstadoJuego()
{
    Debug.Log("TIME SCALE: " + Time.timeScale);
    Debug.Log("MENU PAUSA ACTIVO: " +
        (panelPausa != null && panelPausa.activeSelf));
}
}
