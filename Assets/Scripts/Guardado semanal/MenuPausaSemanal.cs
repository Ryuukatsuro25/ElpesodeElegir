
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
    [SerializeField] private PanelFlowManager gestorMartes;
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
        Debug.Log("PRUEBA: SE PRESIONÓ EL BOTÓN DE PAUSA");
        
        if (panelPausa == null) 
        {
        Debug.LogError("ERROR: NO SE HA ASIGNADO EL PANEL DE PAUSA");
        return;
        }

        Debug.Log("2. OBJETO ASIGNADO: " + panelPausa.name);

        panelPausa.SetActive(true);

        Debug.Log("3. ESTADO DESPUÉS DE ACTIVAR: " +
        panelPausa.activeSelf);

        Debug.Log("4. ESTADO EN JERARQUÍA: " +
        panelPausa.activeInHierarchy);

        panelPausa.transform.SetAsLastSibling();

        Time.timeScale = 0f;
        juegoPausado = true;

        Debug.Log("PRUEBA: PANEL DE PAUSA ACTIVADO");
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

        // MARTES
        if (gestorMartes != null)
        {
            gestorMartes.GuardarProgreso();
            progresoGuardado = true;
        }
        // LUNES
        else if (gestorLunes != null)
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
