
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorPanelesLunes : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject mision;
    [SerializeField] private GameObject introduccion;
    [SerializeField] private GameObject Paneldecision;
    [SerializeField] private GameObject decisionIrAlComer;
    [SerializeField] private GameObject panelBancoIrAlComer;

    [Header("Boton Siguiente de la mision")]
    [SerializeField] private GameObject botonSiguienteMision;

    [Header("Decision de quedarse en casa")]
    [SerializeField] private GameObject decisionQuedarseEnCasa;
    [SerializeField] private GameObject panelBancoQuedarseEnCasa;

    [Header("Segundo almuerzo")]
    [SerializeField] private GameObject panelSegundoIrAlComer;
    [SerializeField] private GameObject decisionSegundoAlmuerzo;
    [SerializeField] private GameObject decisionAlmuerzoBarato;
    [SerializeField] private GameObject panelBancoAlmuerzoBarato;
    [SerializeField] private GameObject panelSegundoAlmuerzoBarato;

    [Header("Convergencia de almuerzos")]
    [SerializeField] private GameObject panelConvergenciaAlmuerzo;

    [Header("Decision de camisa")]
    [SerializeField] private GameObject decisionComprarCamisa;
    [SerializeField] private GameObject panelDecisionCamisa;
    [SerializeField] private GameObject panelResultadoComprarCamisa;
    [SerializeField] private GameObject panelResultadoNoComprarCamisa;

    [Header("Paneles bancarios de camisa")]
    [SerializeField] private GameObject panelBancoComprarCamisa;
    [SerializeField] private GameObject panelBancoNoComprarCamisa;

    [Header("Tercer almuerzo")]
    [SerializeField] private GameObject decisionAlmuerzoCostoso;
    [SerializeField] private GameObject panelBancoAlmuerzoCostoso;
    [SerializeField] private GameObject panelSegundoAlmuerzoCostoso;
    [SerializeField] private GameObject panelTercerAlmuerzoCostoso;
    [SerializeField] private GameObject panelCuartoAlmuerzoCostoso;

    [Header("Panel inicial opcional")]
    [SerializeField] private GameObject panelInicial;

    private GameObject panelActual;

    private void Start()
    {
        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
    }

    public void MostrarPanel(GameObject panelDestino)
    {
        if (panelDestino == null)
        {
            Debug.LogWarning(
                "GestorPanelesLunes: el panel de destino no está asignado."
            );
            return;
        }

        OcultarTodosLosPaneles();

        panelDestino.SetActive(true);
        panelActual = panelDestino;

        if (panelDestino == introduccion && botonSiguienteMision != null)
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
            Paneldecision,
            decisionIrAlComer,
            panelBancoIrAlComer,
            decisionQuedarseEnCasa,
            panelBancoQuedarseEnCasa,
            panelSegundoIrAlComer,
            decisionSegundoAlmuerzo,
            decisionAlmuerzoBarato,
            panelBancoAlmuerzoBarato,
            panelSegundoAlmuerzoBarato,
            panelConvergenciaAlmuerzo,
            decisionComprarCamisa,
            panelDecisionCamisa,
            panelResultadoComprarCamisa,
            panelResultadoNoComprarCamisa,
            panelBancoComprarCamisa,
            panelBancoNoComprarCamisa,
            decisionAlmuerzoCostoso,
            panelBancoAlmuerzoCostoso,
            panelSegundoAlmuerzoCostoso,
            panelTercerAlmuerzoCostoso,
            panelCuartoAlmuerzoCostoso
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

    public void IrAIntroduccion()
    {
        MostrarPanel(introduccion);
    }

    public void IrADecision()
    {
        MostrarPanel(Paneldecision);
    }

    // DECISION: IR A COMER

    public void IrADecisionIrAlComer()
    {
        MostrarPanel(decisionIrAlComer);
    }

    public void IrAPanelBancoIrAlComer()
    {
        MostrarPanel(panelBancoIrAlComer);
    }

    // DECISION: QUEDARSE EN CASA

    public void IrADecisionQuedarseEnCasa()
    {
        MostrarPanel(decisionQuedarseEnCasa);
    }

    public void IrAPanelBancoQuedarseEnCasa()
    {
        MostrarPanel(panelBancoQuedarseEnCasa);
    }

    // SEGUNDO ALMUERZO

    public void IrAPanelSegundoIrAlComer()
    {
        MostrarPanel(panelSegundoIrAlComer);
    }

    public void IrADecisionSegundoAlmuerzo()
    {
        MostrarPanel(decisionSegundoAlmuerzo);
    }

    // ALMUERZO BARATO

    public void IrADecisionAlmuerzoBarato()
    {
        MostrarPanel(decisionAlmuerzoBarato);
    }

    public void IrAPanelBancoAlmuerzoBarato()
    {
        MostrarPanel(panelBancoAlmuerzoBarato);
    }

    public void IrAPanelSegundoAlmuerzoBarato()
    {
        MostrarPanel(panelSegundoAlmuerzoBarato);
    }

    // CONVERGENCIA DE ALMUERZOS

    public void IrAConvergenciaAlmuerzo()
    {
        MostrarPanel(panelConvergenciaAlmuerzo);
    }

    // DECISION DE CAMISA

    public void IrADecisionComprarCamisa()
    {
        MostrarPanel(decisionComprarCamisa);
    }

    public void IrAPanelDecisionCamisa()
    {
        MostrarPanel(panelDecisionCamisa);
    }

    public void IrAPanelResultadoComprarCamisa()
    {
        MostrarPanel(panelResultadoComprarCamisa);
    }

    public void IrAPanelResultadoNoComprarCamisa()
    {
        MostrarPanel(panelResultadoNoComprarCamisa);
    }

    // PANELES BANCARIOS DE CAMISA

    public void IrAPanelBancoComprarCamisa()
    {
        MostrarPanel(panelBancoComprarCamisa);
    }

    public void IrAPanelBancoNoComprarCamisa()
    {
        MostrarPanel(panelBancoNoComprarCamisa);
    }

    // ALMUERZO COSTOSO

    public void IrADecisionAlmuerzoCostoso()
    {
        MostrarPanel(decisionAlmuerzoCostoso);
    }

    public void IrAPanelBancoAlmuerzoCostoso()
    {
        MostrarPanel(panelBancoAlmuerzoCostoso);
    }

    public void IrAPanelSegundoAlmuerzoCostoso()
    {
        MostrarPanel(panelSegundoAlmuerzoCostoso);
    }

    public void IrAPanelTercerAlmuerzoCostoso()
    {
        MostrarPanel(panelTercerAlmuerzoCostoso);
    }

    public void IrAPanelCuartoAlmuerzoCostoso()
    {
        MostrarPanel(panelCuartoAlmuerzoCostoso);
    }

    // VOLVER AL PANEL INICIAL

    public void VolverAlPanelInicial()
    {
        MostrarPanel(panelInicial);
    }

    // CARGAR OTRA ESCENA

    public void IrAEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
