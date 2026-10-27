
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorPanelesTutorial : MonoBehaviour
{
    [Header("Paneles principales")]
    [SerializeField] private GameObject introduccion;
    [SerializeField] private GameObject panelDecision;
    [SerializeField] private GameObject decisionComprar;
    [SerializeField] private GameObject decisionNoComprar;

    [Header("Paneles del banco")]
    [SerializeField] private GameObject panelBancoComprar;
    [SerializeField] private GameObject panelBancoNoComprar;

    [Header("Paneles de mensajes")]
    [SerializeField] private GameObject mensajeNoComprar;
    [SerializeField] private GameObject mensajeComprar;
    [SerializeField] private GameObject mensajeComprar2;
    [SerializeField] private GameObject mensajeComprar3;

    [Header("Panel inicial opcional")]
    [SerializeField] private GameObject panelInicial;

    private GameObject panelActual;

    private void Start()
    {
        // Si asignaste un panel inicial, lo mostramos.
        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
    }

    public void MostrarPanel(GameObject panelDestino)
    {
        if (panelDestino == null)
        {
            Debug.LogWarning("No se ha asignado el panel de destino.");
            return;
        }

        OcultarTodosLosPaneles();

        panelDestino.SetActive(true);
        panelActual = panelDestino;
    }

    private void OcultarTodosLosPaneles()
    {
        GameObject[] paneles =
        {
            introduccion,
            panelDecision,
            decisionComprar,
            decisionNoComprar,
            panelBancoComprar,
            panelBancoNoComprar,
            mensajeNoComprar,
            mensajeComprar,
            mensajeComprar2,
            mensajeComprar3
        };

        foreach (GameObject panel in paneles)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    // Botones de decisión
    public void IrAIntroduccion()
    {
        MostrarPanel(introduccion);
    }

    public void IrADecision()
    {
        MostrarPanel(panelDecision);
    }

    public void IrADecisionComprar()
    {
        MostrarPanel(decisionComprar);
    }

    public void IrADecisionNoComprar()
    {
        MostrarPanel(decisionNoComprar);
    }

    // Botones del banco
    public void Comprar()
    {
        MostrarPanel(panelBancoComprar);
    }

    public void NoComprar()
    {
        MostrarPanel(panelBancoNoComprar);
    }

    // Botones de mensajes
    public void IrAMensajeNoComprar()
    {
        MostrarPanel(mensajeNoComprar);
    }

    public void IrAMensajeComprar()
    {
        MostrarPanel(mensajeComprar);
    }

    public void IrAMensajeComprar2()
    {
        MostrarPanel(mensajeComprar2);
    }

    public void IrAMensajeComprar3()
    {
        MostrarPanel(mensajeComprar3);
    }

    
    public void IrAEscenaLunes()
    {
        SceneManager.LoadScene("Lunes");
    }


    public void VolverAlPanelInicial()
    {
        MostrarPanel(panelInicial);
    }
}
