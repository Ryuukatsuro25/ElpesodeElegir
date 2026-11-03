
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorPanelesMiercoles : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject mision;

    [Header("Nuevos paneles iniciales del miercoles")]
    [SerializeField] private GameObject consejoMiercoles;
    [SerializeField] private GameObject conversacionpadre;
    [SerializeField] private GameObject conversacionpadre2;
    [SerializeField] private GameObject preintroduccion;
    [SerializeField] private GameObject retroalimentacionintro;

    [Header("Paneles posteriores")]
    [SerializeField] private GameObject introduccion;
    [SerializeField] private GameObject segundoPanel;
    [SerializeField] private GameObject tercerPanel;
    [SerializeField] private GameObject panelDecision;

    [Header("Decisiones de grupo")]
    [SerializeField] private GameObject decisionGrupo1;
    [SerializeField] private GameObject decisionGrupo2;
    [SerializeField] private GameObject segundoPanelGrupo1;
    [SerializeField] private GameObject segundoPanelGrupo2;
    [SerializeField] private GameObject tercerPanelGrupo2;
    [SerializeField] private GameObject cuartoPanelGrupo2;

    [Header("Decisiones: preguntar a compañeros")]
    [SerializeField] private GameObject decisionPreguntarCompaneros;
    [SerializeField] private GameObject decisionPreguntales;
    [SerializeField] private GameObject segundoPanelPreguntales;
    [SerializeField] private GameObject tercerPanelPreguntales;
    [SerializeField] private GameObject cuartoPanelPreguntales;

    [Header("Decisiones: no preguntar a compañeros")]
    [SerializeField] private GameObject decisionNoPreguntarles;
    [SerializeField] private GameObject segundoPanelNoPreguntarles;
    [SerializeField] private GameObject tercerPanelNoPreguntarles;
    [SerializeField] private GameObject cuartoPanelNoPreguntarles;
    [SerializeField] private GameObject quintoPanelNoPreguntarles;
    [SerializeField] private GameObject sextoPanelNoPreguntarles;
    [SerializeField] private GameObject septimoPanelNoPreguntarles;
    [SerializeField] private GameObject octavoPanelNoPreguntarles;

    [Header("Configuración inicial")]
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
                "GestorPanelesMiercoles: el panel de destino no está asignado."
            );
            return;
        }

        OcultarTodosLosPaneles();

        panelDestino.SetActive(true);
        panelActual = panelDestino;

        if (panelDestino == consejoMiercoles &&
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

            // Nuevos paneles iniciales
            consejoMiercoles,
            conversacionpadre,
            conversacionpadre2,
            preintroduccion,
            retroalimentacionintro,

            introduccion,
            segundoPanel,
            tercerPanel,
            panelDecision,

            decisionGrupo1,
            decisionGrupo2,
            segundoPanelGrupo1,
            segundoPanelGrupo2,
            tercerPanelGrupo2,
            cuartoPanelGrupo2,

            decisionPreguntarCompaneros,
            decisionPreguntales,
            segundoPanelPreguntales,
            tercerPanelPreguntales,
            cuartoPanelPreguntales,

            decisionNoPreguntarles,
            segundoPanelNoPreguntarles,
            tercerPanelNoPreguntarles,
            cuartoPanelNoPreguntarles,
            quintoPanelNoPreguntarles,
            sextoPanelNoPreguntarles,
            septimoPanelNoPreguntarles,
            octavoPanelNoPreguntarles
        };

        foreach (GameObject panel in paneles)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    // PANEL PRINCIPAL

    public void IrAMision()
    {
        MostrarPanel(mision);
    }

    // NUEVOS PANELES INICIALES DEL MIERCOLES

    public void IrAConsejoMiercoles()
    {
        MostrarPanel(consejoMiercoles);
    }

    public void IrAConversacionPadre()
    {
        MostrarPanel(conversacionpadre);
    }

    public void IrAConversacionPadre2()
    {
        MostrarPanel(conversacionpadre2);
    }

    public void IrAPreintroduccion()
    {
        MostrarPanel(preintroduccion);
    }

    public void IrARetroalimentacionIntro()
    {
        MostrarPanel(retroalimentacionintro);
    }

    // PANELES POSTERIORES

    public void IrAIntroduccion()
    {
        MostrarPanel(introduccion);
    }

    public void IrASegundoPanel()
    {
        MostrarPanel(segundoPanel);
    }

    public void IrATercerPanel()
    {
        MostrarPanel(tercerPanel);
    }

    public void IrAPanelDecision()
    {
        MostrarPanel(panelDecision);
    }

    // DECISIONES DE GRUPO

    public void IrADecisionGrupo1()
    {
        MostrarPanel(decisionGrupo1);
    }

    public void IrADecisionGrupo2()
    {
        MostrarPanel(decisionGrupo2);
    }

    public void IrASegundoPanelGrupo1()
    {
        MostrarPanel(segundoPanelGrupo1);
    }

    public void IrASegundoPanelGrupo2()
    {
        MostrarPanel(segundoPanelGrupo2);
    }

    public void IrATercerPanelGrupo2()
    {
        MostrarPanel(tercerPanelGrupo2);
    }

    public void IrACuartoPanelGrupo2()
    {
        MostrarPanel(cuartoPanelGrupo2);
    }

    // DECISIONES: PREGUNTAR A COMPAÑEROS

    public void IrADecisionPreguntarCompaneros()
    {
        MostrarPanel(decisionPreguntarCompaneros);
    }

    public void IrADecisionPreguntales()
    {
        MostrarPanel(decisionPreguntales);
    }

    public void IrASegundoPanelPreguntales()
    {
        MostrarPanel(segundoPanelPreguntales);
    }

    public void IrATercerPanelPreguntales()
    {
        MostrarPanel(tercerPanelPreguntales);
    }

    public void IrACuartoPanelPreguntales()
    {
        MostrarPanel(cuartoPanelPreguntales);
    }

    // DECISIONES: NO PREGUNTAR A COMPAÑEROS

    public void IrADecisionNoPreguntarles()
    {
        MostrarPanel(decisionNoPreguntarles);
    }

    public void IrASegundoPanelNoPreguntarles()
    {
        MostrarPanel(segundoPanelNoPreguntarles);
    }

    public void IrATercerPanelNoPreguntarles()
    {
        MostrarPanel(tercerPanelNoPreguntarles);
    }

    public void IrACuartoPanelNoPreguntarles()
    {
        MostrarPanel(cuartoPanelNoPreguntarles);
    }

    public void IrAQuintoPanelNoPreguntarles()
    {
        MostrarPanel(quintoPanelNoPreguntarles);
    }

    public void IrASextoPanelNoPreguntarles()
    {
        MostrarPanel(sextoPanelNoPreguntarles);
    }

    public void IrASeptimoPanelNoPreguntarles()
    {
        MostrarPanel(septimoPanelNoPreguntarles);
    }

    public void IrAOctavoPanelNoPreguntarles()
    {
        MostrarPanel(octavoPanelNoPreguntarles);
    }

    // NAVEGACION GENERAL

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

    public void IrAEscena(string nombreEscena)
    {
        if (!string.IsNullOrWhiteSpace(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning(
                "GestorPanelesMiercoles: el nombre de la escena está vacío."
            );
        }
    }

    public void OcultarPanelActual()
    {
        if (panelActual != null)
        {
            panelActual.SetActive(false);
            panelActual = null;
        }
    }
}
