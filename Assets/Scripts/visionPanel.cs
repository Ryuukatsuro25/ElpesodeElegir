using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisionPanel : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoDescripcion;
    public TextMeshProUGUI textoImpactoEconomico;
    public TextMeshProUGUI textoImpactoBienestar;
    public GameObject panelTarjetaFlotante;

    [Header("Gestión de Paneles")]
    public GameObject panelActual;              // Panel actual de decisión que se oculta
    public GameObject panelSiguiente;           // Panel al que se avanza tras confirmar
    public GameObject panelPreDecisionARetirar; // Panel anterior rezagado que se apaga a la fuerza

    private void Start()
    {
        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(false);
    }

    // --- PASO 1: CAFÉ ---
    public void ClickTomarCafe()
    {
        MostrarAdvertenciaCafe();
    }

    // --- PASO 1: DORMIR ---
    public void ClickDormir()
    {
        MostrarAdvertenciaDormir();
    }

    private void MostrarAdvertenciaCafe()
    {
        if (UIBarrasController.Instance != null)
        {
            float dineroActual = UIBarrasController.Instance.dinero;
            float bienestarActual = UIBarrasController.Instance.bienestarMental;

            float dineroFinal = dineroActual - 2000f;
            float bienestarFinal = Mathf.Clamp(bienestarActual + 10f, 0f, UIBarrasController.Instance.bienestarMaximo);

            if (textoDescripcion != null)
                textoDescripcion.text = "ADVERTENCIA: Vas a gastar 2000 de tu dinero y mantenerte despierto.";

            if (textoImpactoEconomico != null)
                textoImpactoEconomico.text = $"Te quedaría: ${dineroFinal:N0}";

            if (textoImpactoBienestar != null)
                textoImpactoBienestar.text = $"Bienestar: {bienestarFinal:F0}%";
        }

        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(true);
    }

    private void MostrarAdvertenciaDormir()
    {
        if (UIBarrasController.Instance != null)
        {
            float dineroActual = UIBarrasController.Instance.dinero;
            float bienestarActual = UIBarrasController.Instance.bienestarMental;

            float bienestarFinal = Mathf.Clamp(bienestarActual + 30f, 0f, UIBarrasController.Instance.bienestarMaximo);

            if (textoDescripcion != null)
                textoDescripcion.text = "ADVERTENCIA: Vas a descansar, recuperando tu mente sin gastar dinero.";

            if (textoImpactoEconomico != null)
                textoImpactoEconomico.text = $"Te quedaría: ${dineroActual:N0} (Sin cambios)";

            if (textoImpactoBienestar != null)
                textoImpactoBienestar.text = $"Bienestar: {bienestarFinal:F0}%";
        }

        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(true);
    }

    private void OcultarTarjeta()
    {
        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(false);
    }

    // --- ACCIÓN REAL: CONFIRMAR CAFÉ ---
    public void ConfirmarAccionCafe()
    {
        OcultarTarjeta();

        if (UIBarrasController.Instance != null)
        {
            UIBarrasController.Instance.RegistrarTransaccion("Compra de Café", 2000f, false);
            UIBarrasController.Instance.ModificarBienestar(10f);
        }

        Debug.Log("Acción confirmada: Café comprado y aplicado.");
        LimpiarPanelesAnterioresYAvanzar();
    }

    // --- ACCIÓN REAL: CONFIRMAR DORMIR ---
    public void ConfirmarAccionDormir()
    {
        OcultarTarjeta();

        if (UIBarrasController.Instance != null)
        {
            UIBarrasController.Instance.ModificarBienestar(30f);
        }

        Debug.Log("Acción confirmada: Dormir aplicado.");
        LimpiarPanelesAnterioresYAvanzar();
    }

    // --- MÉTODO CENTRALIZADO DE LIMPIEZA Y AVANCE ---
    private void LimpiarPanelesAnterioresYAvanzar()
    {
        // Apaga a la fuerza el panel pre-decisión o cualquier rezagado para evitar bucles
        if (panelPreDecisionARetirar != null)
            panelPreDecisionARetirar.SetActive(false);

        if (panelActual != null)
            panelActual.SetActive(false);

        if (panelSiguiente != null)
            panelSiguiente.SetActive(true);
    }
}