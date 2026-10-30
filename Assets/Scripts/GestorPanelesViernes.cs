
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorPanelesViernes : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject mision;
    [SerializeField] private GameObject introduccion;
    [SerializeField] private GameObject panelDecision;

    [Header("Decisiones")]
    [SerializeField] private GameObject decisionNoHacerTarea;
    [SerializeField] private GameObject decisionPedirCopia;

    [Header("Paneles de las decisiones")]
    [SerializeField] private GameObject segundoPanelNoHacerTarea;
    [SerializeField] private GameObject segundoPanelPedirCopia;

    [Header("Configuracion inicial")]
    [SerializeField] private GameObject panelInicial;
    [SerializeField] private GameObject botonSiguienteMision;

    private GameObject panelActual;

    private void Start()
    {
        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
        else if (mision != null)
        {
            MostrarPanel(mision);
        }
    }

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
    }

    private void OcultarTodosLosPaneles()
    {
        GameObject[] paneles =
        {
            mision,
            introduccion,
            panelDecision,
            decisionNoHacerTarea,
            decisionPedirCopia,
            segundoPanelNoHacerTarea,
            segundoPanelPedirCopia
        };

        foreach (GameObject panel in paneles)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    // PANELES PRINCIPALES

    public void IrAMision()
    {
        MostrarPanel(mision);
    }

    public void IrAIntroduccion()
    {
        MostrarPanel(introduccion);
    }

    public void IrAPanelDecision()
    {
        MostrarPanel(panelDecision);
    }

    // DECISION: NO HACER LA TAREA

    public void IrADecisionNoHacerTarea()
    {
        MostrarPanel(decisionNoHacerTarea);
    }

    public void IrASegundoPanelNoHacerTarea()
    {
        MostrarPanel(segundoPanelNoHacerTarea);
    }

    // DECISION: PEDIR COPIA

    public void IrADecisionPedirCopia()
    {
        MostrarPanel(decisionPedirCopia);
    }

    public void IrASegundoPanelPedirCopia()
    {
        MostrarPanel(segundoPanelPedirCopia);
    }

    // VOLVER AL PANEL INICIAL

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

    // CARGAR OTRA ESCENA

    public void IrAEscena(string nombreEscena)
    {
        if (!string.IsNullOrWhiteSpace(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning(
                "GestorPanelesViernes: el nombre de la escena esta vacio."
            );
        }
    }
}
