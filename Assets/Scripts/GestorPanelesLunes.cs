
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Maquina de Goldberg")]
    [SerializeField] private GameObject decisionMaquinaGoldberg;
    [SerializeField] private GameObject resultadoComprarGoldberg;
    [SerializeField] private GameObject resultadoImprovisarGoldberg;

    [Header("Sistema bancario")]
    [SerializeField] private GestorBancario gestorBancario;

    [Header("Sistema de bienestar")]
    [SerializeField] private PopUpManagerLunes gestorBienestar;

    [Header("Textos bancarios - Almuerzo barato")]
    [SerializeField] private TMP_Text saldoBarato;
    [SerializeField] private TMP_Text ingresosBarato;
    [SerializeField] private TMP_Text gastosBarato;

    [Header("Textos bancarios - Almuerzo costoso")]
    [SerializeField] private TMP_Text saldoCostoso;
    [SerializeField] private TMP_Text ingresosCostoso;
    [SerializeField] private TMP_Text gastosCostoso;

    [Header("Panel inicial opcional")]
    [SerializeField] private GameObject panelInicial;

    private GameObject panelActual;

    private void Start()
    {
        if (gestorBancario == null)
        {
            gestorBancario = FindObjectOfType<GestorBancario>();
        }

        if (gestorBienestar == null)
        {
            gestorBienestar = FindObjectOfType<PopUpManagerLunes>();
        }

        if (gestorBancario == null)
        {
            Debug.LogWarning(
                "GestorPanelesLunes: no se encontró GestorBancario en la escena."
            );
        }

        if (gestorBienestar == null)
        {
            Debug.LogWarning(
                "GestorPanelesLunes: no se encontró PopUpManagerLunes en la escena."
            );
        }

        if (panelInicial != null)
        {
            MostrarPanel(panelInicial);
        }
    }

    // NAVEGACIÓN ENTRE PANELES

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
            panelCuartoAlmuerzoCostoso,

            // Paneles de la maquina de Goldberg
            decisionMaquinaGoldberg,
            resultadoComprarGoldberg,
            resultadoImprovisarGoldberg
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
        RegistrarGastoAlmuerzo(
            18000,
            "Almuerzo barato con Juan"
        );

        ActualizarPanelBanco(true);

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
        RegistrarGastoAlmuerzo(
            30000,
            "Almuerzo costoso con Juan"
        );

        ActualizarPanelBanco(false);

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

    // MAQUINA DE GOLDBERG

    public void IrADecisionMaquinaGoldberg()
    {
        MostrarPanel(decisionMaquinaGoldberg);
    }

    // OPCION 1: COMPRAR MATERIALES

    public void ElegirComprarMateriales()
    {
        if (gestorBancario == null)
        {
            Debug.LogWarning(
                "No se encontró GestorBancario. No se registró la compra."
            );
            return;
        }

        gestorBancario.AgregarTransaccion(
            15000,
            "Materiales para maquina de Goldberg",
            false
        );

        Debug.Log(
            "Compra de materiales para Goldberg registrada: -$15.000"
        );

        MostrarPanel(resultadoComprarGoldberg);
    }

    // OPCION 2: IMPROVISAR

    public void ElegirImprovisar()
    {
        if (gestorBienestar == null)
        {
            Debug.LogWarning(
                "No se encontró PopUpManagerLunes. No se aplicó el cambio de bienestar."
            );
            return;
        }

        gestorBienestar.AplicarCambioBienestar(
            -7,
            "Improvisaste los materiales. Tu bienestar disminuyó 7 puntos."
        );

        MostrarPanel(resultadoImprovisarGoldberg);
    }

    // REGISTRO DE GASTOS

    private void RegistrarGastoAlmuerzo(int monto, string descripcion)
    {
        if (gestorBancario == null)
        {
            Debug.LogWarning(
                "No se encontró el GestorBancario. No se registró el almuerzo."
            );
            return;
        }

        gestorBancario.AgregarTransaccion(
            monto,
            descripcion,
            false
        );

        Debug.Log("Gasto registrado: " + descripcion + " -$" + monto);
    }

    // ACTUALIZACIÓN DE LOS PANELES BANCARIOS

    private void ActualizarPanelBanco(bool almuerzoBarato)
    {
        if (gestorBancario == null)
        {
            Debug.LogWarning(
                "No se puede actualizar el panel: GestorBancario no está asignado."
            );
            return;
        }

        TMP_Text textoSaldo;
        TMP_Text textoIngresos;
        TMP_Text textoGastos;

        if (almuerzoBarato)
        {
            textoSaldo = saldoBarato;
            textoIngresos = ingresosBarato;
            textoGastos = gastosBarato;
        }
        else
        {
            textoSaldo = saldoCostoso;
            textoIngresos = ingresosCostoso;
            textoGastos = gastosCostoso;
        }

        if (textoSaldo != null)
        {
            textoSaldo.text = $"${gestorBancario.SaldoActual:N0}";
        }
        else
        {
            Debug.LogWarning(
                "No se asignó el texto del saldo del panel bancario."
            );
        }

        if (textoIngresos != null)
        {
            textoIngresos.text = $"+${gestorBancario.IngresosTotales:N0}";
        }
        else
        {
            Debug.LogWarning(
                "No se asignó el texto de ingresos del panel bancario."
            );
        }

        if (textoGastos != null)
        {
            textoGastos.text = $"-${gestorBancario.GastosTotales:N0}";
        }
        else
        {
            Debug.LogWarning(
                "No se asignó el texto de gastos del panel bancario."
            );
        }
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
