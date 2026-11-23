
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorPanelesViernes : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject mision;
    [SerializeField] private GameObject desayuno;
    [SerializeField] private GameObject bus;
    [SerializeField] private GameObject introduccion;
    [SerializeField] private GameObject panelDecision;

    [Header("Decisiones")]
    [SerializeField] private GameObject decisionNoHacerTarea;
    [SerializeField] private GameObject decisionPedirCopia;

    [Header("Paneles de las decisiones")]
    [SerializeField] private GameObject segundoPanelNoHacerTarea;
    [SerializeField] private GameObject tercerPanelNoHacerTarea;
    [SerializeField] private GameObject segundoPanelPedirCopia;
    [SerializeField] private GameObject tercerPanelPedirCopia;

    [Header("Configuracion inicial")]
    [SerializeField] private GameObject panelInicial;
    [SerializeField] private GameObject botonSiguienteMision;

    [Header("Sistema de bienestar del viernes")]
    [SerializeField]
    private BienestarDecisionesViernes bienestarViernes;

    private GameObject panelActual;

    private void Start()
    {
        Time.timeScale = 1f;

        if (RestaurarCheckpoint())
        {
            if (bienestarViernes != null)
            {
                bienestarViernes.CargarProgreso();
            }

            Debug.Log("Progreso del viernes recuperado.");
            return;
        }

        if (bienestarViernes != null)
        {
            bienestarViernes.InicializarNuevoProgreso();
        }

        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
        else if (mision != null)
        {
            MostrarPanel(mision);
        }
    }

    // ==========================================
    // MOSTRAR PANELES
    // ==========================================

    public void MostrarPanel(GameObject panelDestino)
    {
        if (panelDestino == null)
        {
            Debug.LogWarning(
                "GestorPanelesViernes: el panel de destino no esta asignado."
            );
            return;
        }

        OcultarTodosLosPaneles();

        panelDestino.SetActive(true);
        panelActual = panelDestino;

        if (panelDestino == introduccion &&
            botonSiguienteMision != null)
        {
            botonSiguienteMision.SetActive(false);
        }

        SistemaGuardadoSemanal.GuardarCheckpoint(
            SceneManager.GetActiveScene().name,
            panelDestino.name
        );
    }

    private GameObject[] ObtenerTodosLosPaneles()
    {
        return new GameObject[]
        {
            mision,
            desayuno,
            bus,
            introduccion,
            panelDecision,
            decisionNoHacerTarea,
            decisionPedirCopia,
            segundoPanelNoHacerTarea,
            tercerPanelNoHacerTarea,
            segundoPanelPedirCopia,
            tercerPanelPedirCopia
        };
    }

    private void OcultarTodosLosPaneles()
    {
        foreach (GameObject panel in ObtenerTodosLosPaneles())
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    // ==========================================
    // RECUPERAR CHECKPOINT SEMANAL
    // ==========================================

    private bool RestaurarCheckpoint()
    {
        if (!SistemaGuardadoSemanal.IntentarObtenerPanel(
            SceneManager.GetActiveScene().name,
            out string nombrePanel))
        {
            return false;
        }

        foreach (GameObject panel in ObtenerTodosLosPaneles())
        {
            if (panel != null && panel.name == nombrePanel)
            {
                MostrarPanel(panel);

                Debug.Log(
                    "Viernes: panel recuperado: " + nombrePanel
                );

                return true;
            }
        }

        Debug.LogWarning(
            "Viernes: no se encontro el panel guardado: " + nombrePanel
        );

        return false;
    }

    // ==========================================
    // GUARDAR PROGRESO COMPLETO DEL VIERNES
    // ==========================================

    public void GuardarProgreso()
    {
        if (panelActual != null)
        {
            SistemaGuardadoSemanal.GuardarCheckpoint(
                SceneManager.GetActiveScene().name,
                panelActual.name
            );
        }

        if (bienestarViernes != null)
        {
            bienestarViernes.GuardarProgreso();
        }

        SistemaGuardadoSemanal.GuardarTodo();

        Debug.Log("Progreso completo del viernes guardado.");
    }

    // ==========================================
    // PANELES PRINCIPALES
    // ==========================================

    public void IrAMision()
    {
        MostrarPanel(mision);
    }

    public void IrADesayuno()
    {
        MostrarPanel(desayuno);
    }

    public void IrABus()
    {
        MostrarPanel(bus);
    }

    public void IrAIntroduccion()
    {
        MostrarPanel(introduccion);
    }

    public void IrAPanelDecision()
    {
        MostrarPanel(panelDecision);

        if (bienestarViernes != null)
        {
            bienestarViernes.PrepararDecision();
        }
    }

    // ==========================================
    // DECISION: NO HACER LA TAREA
    // ==========================================

    public void IrADecisionNoHacerTarea()
    {
        MostrarPanel(decisionNoHacerTarea);
    }

    public void IrASegundoPanelNoHacerTarea()
    {
        MostrarPanel(segundoPanelNoHacerTarea);
    }

    public void IrATercerPanelNoHacerTarea()
    {
        MostrarPanel(tercerPanelNoHacerTarea);
    }

    // ==========================================
    // DECISION: PEDIR COPIA
    // ==========================================

    public void IrADecisionPedirCopia()
    {
        MostrarPanel(decisionPedirCopia);
    }

    public void IrASegundoPanelPedirCopia()
    {
        MostrarPanel(segundoPanelPedirCopia);
    }

    public void IrATercerPanelPedirCopia()
    {
        MostrarPanel(tercerPanelPedirCopia);
    }

    // ==========================================
    // VOLVER AL PANEL INICIAL
    // ==========================================

    public void VolverAlPanelInicial()
    {
        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
        else
        {
            MostrarPanel(mision);
        }
    }

    // ==========================================
    // CARGAR OTRA ESCENA
    // ==========================================

    public void IrAEscena(string nombreEscena)
    {
        if (!string.IsNullOrWhiteSpace(nombreEscena))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning(
                "GestorPanelesViernes: el nombre de la escena esta vacio."
            );
        }
    }

    // ==========================================
    // GUARDAR Y SALIR AL MENU PRINCIPAL
    // ==========================================

    public void GuardarYSalir(string nombreEscenaMenu)
    {
        GuardarProgreso();

        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogError(
                "No se ha asignado la escena del menu principal."
            );
        }
    }
}
