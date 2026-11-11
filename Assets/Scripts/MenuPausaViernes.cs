
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausaViernes : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPausa;

    [Header("Opciones")]
    [SerializeField] private bool cerrarAlIniciar = true;

    [Header("Gestor de paneles del viernes")]
    [SerializeField] private GestorPanelesViernes gestorPaneles;

    [Header("Escena del menu principal")]
    [SerializeField] private string nombreEscenaMenu = "MainMenu";

    private bool juegoPausado = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (cerrarAlIniciar && panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        juegoPausado = false;
    }

    // ==========================================
    // ABRIR PANEL
    // ==========================================

    public void AbrirPanel()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);

            Time.timeScale = 0f;
            juegoPausado = true;

            Debug.Log("Menu de pausa abierto");
        }
    }

    // ==========================================
    // CERRAR PANEL
    // ==========================================

    public void CerrarPanel()
    {
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);

            Time.timeScale = 1f;
            juegoPausado = false;

            Debug.Log("Menu de pausa cerrado");
        }
    }

    // ==========================================
    // ALTERNAR PANEL
    // ==========================================

    public void AlternarPausa()
    {
        if (juegoPausado)
        {
            CerrarPanel();
        }
        else
        {
            AbrirPanel();
        }
    }

    // ==========================================
    // CONTINUAR JUGANDO
    // ==========================================

    public void ContinuarJuego()
    {
        CerrarPanel();
    }

    // ==========================================
    // GUARDAR Y SALIR
    // ==========================================

    public void GuardarYSalir()
    {
        if (gestorPaneles == null)
        {
            Debug.LogError(
                "No se ha asignado GestorPanelesViernes."
            );

            return;
        }

        gestorPaneles.GuardarProgreso();

        Time.timeScale = 1f;
        juegoPausado = false;

        if (!string.IsNullOrWhiteSpace(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogError(
                "No se ha asignado el nombre de la escena principal."
            );
        }
    }

    // ==========================================
    // SALIR SIN GUARDAR
    // ==========================================

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;

        if (!string.IsNullOrWhiteSpace(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogError(
                "No se ha asignado el nombre de la escena principal."
            );
        }
    }
}
