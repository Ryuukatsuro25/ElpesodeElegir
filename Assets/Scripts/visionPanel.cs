using System.Collections;
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

    [Header("Gestión de Paneles (Siguiente Escena/Panel)")]
    public GameObject panelActual;     // El panel de decisión actual que se debe ocultar
    public GameObject panelSiguiente;  // El panel al que deseas avanzar tras confirmar

    [Header("Configuración")]
    public float tiempoVisibilidad = 7f;

    private Coroutine corrutinaTemporizador;
    private bool advertenciaActiva = false;

    private void Start()
    {
        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(false);
    }

    // --- OPCIÓN: TOMAR CAFÉ ---
    public void ClickTomarCafe()
    {
        if (!advertenciaActiva)
        {
            // PRIMER CLIC: Solo muestra la advertencia y abre la tarjeta por 7s
            MostrarAdvertenciaCafe();
            advertenciaActiva = true;
        }
        else
        {
            // SEGUNDO CLIC: El usuario confirma después de leer la advertencia
            EjecutarAccionCafe();
        }
    }

    private void MostrarAdvertenciaCafe()
    {
        if (UIBarrasController.Instance != null)
        {
            float dineroActual = UIBarrasController.Instance.dinero;
            float bienestarActual = UIBarrasController.Instance.bienestarMental;

            // Calculamos los valores proyectados finales
            float dineroFinal = dineroActual - 2000f;
            float bienestarFinal = Mathf.Clamp(bienestarActual + 10f, 0f, UIBarrasController.Instance.bienestarMaximo);

            if (textoDescripcion != null)
                textoDescripcion.text = "ADVERTENCIA: Vas a gastar 2000 de tu dinero y mantenerte despierto.";

            // Muestra ÚNICAMENTE cómo quedará el saldo y el bienestar final
            if (textoImpactoEconomico != null)
                textoImpactoEconomico.text = $"te quedaría: ${dineroFinal:N0}";

            if (textoImpactoBienestar != null)
                textoImpactoBienestar.text = $"bienestar: {bienestarFinal:F0}%";
        }

        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(true);

        if (corrutinaTemporizador != null)
            StopCoroutine(corrutinaTemporizador);

        corrutinaTemporizador = StartCoroutine(TemporizadorAdvertencia());
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
                textoImpactoBienestar.text = $"bienestar: {bienestarFinal:F0}%";
        }

        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(true);

        if (corrutinaTemporizador != null)
            StopCoroutine(corrutinaTemporizador);

        corrutinaTemporizador = StartCoroutine(TemporizadorAdvertencia());
    }

    private IEnumerator TemporizadorAdvertencia()
    {
        // Espera los 7 segundos de vigilancia
        yield return new WaitForSeconds(tiempoVisibilidad);

        // Si se acaba el tiempo sin segundo clic, se oculta la tarjeta y se reinicia el estado
        OcultarTarjeta();
    }

    private void OcultarTarjeta()
    {
        if (panelTarjetaFlotante != null)
            panelTarjetaFlotante.SetActive(false);

        advertenciaActiva = false;
    }

    // SEGUNDO CLIC: Acción real de confirmación y cambio de panel
    private void EjecutarAccionCafe()
    {
        if (corrutinaTemporizador != null)
            StopCoroutine(corrutinaTemporizador);

        OcultarTarjeta();

        if (UIBarrasController.Instance != null)
        {
            // Aplica los cambios reales en el banco y las barras
            UIBarrasController.Instance.RegistrarTransaccion("Compra de Café", 2000f, false);
            UIBarrasController.Instance.ModificarBienestar(10f);
        }

        Debug.Log("Acción confirmada: Café comprado y aplicado.");

        // CAMBIO DE PANEL: Oculta el actual y muestra el siguiente
        if (panelActual != null)
            panelActual.SetActive(false);

        if (panelSiguiente != null)
            panelSiguiente.SetActive(true);
    }
}