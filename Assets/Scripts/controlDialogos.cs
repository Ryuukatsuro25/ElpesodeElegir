using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class OpcionBoton
{
    [Tooltip("Botón de acción de la UI.")]
    public Button boton;

    [Tooltip("Texto descriptivo de la opción para la transacción.")]
    public string descripcionOp;

    [Tooltip("Monto de la transacción. Positivo para ingreso, negativo para gasto, 0 para gratis.")]
    public int monto;
}

public class controlDialogos : MonoBehaviour
{
    [Header("Referencias de Sistemas")]
    [Tooltip("Instancia del GestorBancario en la escena.")]
    [SerializeField] private GestorBancario gestorBancario;

    [Header("UI Elements - Dialogs")]
    [Tooltip("Componente de texto de TextMeshPro donde se mostrarán las frases.")]
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Tooltip("Botón principal para avanzar de texto.")]
    [SerializeField] private Button nextButton;

    [Tooltip("Panel de opciones que se activará al llegar al final del texto.")]
    [SerializeField] private GameObject endPanel;

    [Header("UI Elements - Action Buttons Config")]
    [Tooltip("Lista de botones con sus nombres y montos (positivos = ingreso, negativos = gasto).")]
    [SerializeField] private OpcionBoton[] opciones;

    [Header("UI Elements - Confirmation Panel")]
    [Tooltip("Panel de confirmación que aparece tras seleccionar una opción.")]
    [SerializeField] private GameObject confirmationPanel;

    [Tooltip("Texto dentro del panel de confirmación (ej: Escoges Uber).")]
    [SerializeField] private TextMeshProUGUI confirmationText;

    [Tooltip("Texto donde se mostrará el costo/ingreso en el panel de confirmación.")]
    [SerializeField] private TextMeshProUGUI costText;

    [Tooltip("Botón para cancelar/regresar del panel de confirmación.")]
    [SerializeField] private Button backButton;

    [Tooltip("Botón para confirmar y registrar la transacción en el JSON.")]
    [SerializeField] private Button confirmNextButton;

    [Header("Content")]
    [TextArea(2, 5)]
    [Tooltip("Lista de textos que se mostrarán en orden.")]
    [SerializeField] private string[] textArray;

    private int currentIndex = 0;
    private OpcionBoton opcionSeleccionada;

    private void Start()
    {
        // Auto-detectar GestorBancario si no fue asignado manualmente en el Inspector
        if (gestorBancario == null)
        {
            gestorBancario = FindObjectOfType<GestorBancario>();
        }

        // Ocultar paneles al inicio
        if (endPanel != null) endPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);

        // Ocultar los botones de opción al inicio
        SetActionButtonsState(false);

        // Suscribir eventos dinámicos a cada botón de opción
        SetupActionButtons();

        // Suscribir eventos a los botones del panel de confirmación
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseConfirmationPanel);
        }

        if (confirmNextButton != null)
        {
            confirmNextButton.onClick.AddListener(SaveToJSONOnly);
        }

        // Configurar el texto inicial
        if (textArray != null && textArray.Length > 0)
        {
            textDisplay.text = textArray[currentIndex];
        }
        else
        {
            Debug.LogWarning("El arreglo de textos está vacío.");
        }

        // Asignar el evento al botón principal de avance
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceText);
        }
    }

    /// <summary>
    /// Asigna dinámicamente los eventos OnClick a cada opción configurada.
    /// </summary>
    private void SetupActionButtons()
    {
        if (opciones == null) return;

        foreach (OpcionBoton op in opciones)
        {
            if (op == null || op.boton == null) continue;

            OpcionBoton seleccion = op; // Copia local necesaria para la expresión lambda
            op.boton.onClick.AddListener(() => OnOptionSelected(seleccion));
        }
    }

    /// <summary>
    /// Avanza al siguiente texto del arreglo o activa el panel de opciones si es el último.
    /// </summary>
    public void AdvanceText()
    {
        if (textArray == null || textArray.Length == 0) return;

        if (currentIndex < textArray.Length - 1)
        {
            currentIndex++;
            textDisplay.text = textArray[currentIndex];
        }
        else
        {
            OnSequenceEnd();
        }
    }

    /// <summary>
    /// Oculta el botón principal y activa las opciones al finalizar las frases.
    /// </summary>
    private void OnSequenceEnd()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        SetActionButtonsState(true);
    }

    /// <summary>
    /// Captura la opción seleccionada y abre el panel de confirmación mostrando su detalle y costo.
    /// </summary>
    private void OnOptionSelected(OpcionBoton opcion)
    {
        opcionSeleccionada = opcion;

        // Determinar el nombre a mostrar (prioriza descripcionOp, luego el texto del botón o su nombre)
        string nombreMostrar = string.IsNullOrEmpty(opcion.descripcionOp)
            ? (opcion.boton.GetComponentInChildren<TextMeshProUGUI>() != null
                ? opcion.boton.GetComponentInChildren<TextMeshProUGUI>().text
                : opcion.boton.gameObject.name)
            : opcion.descripcionOp;

        if (confirmationText != null)
        {
            confirmationText.text = $"Escoges {nombreMostrar}";
        }

        // Formatear y mostrar el costo o ingreso con colores HTML
        if (costText != null)
        {
            int montoAbsoluto = Mathf.Abs(opcion.monto);

            if (opcion.monto < 0)
            {
                costText.text = $"Costo: <color=#FF0000>-${montoAbsoluto:N0}</color>";
            }
            else if (opcion.monto > 0)
            {
                costText.text = $"Ingreso: <color=#32CD32>+${montoAbsoluto:N0}</color>";
            }
            else
            {
                costText.text = "Costo: <color=#888888>Gratis</color>";
            }
        }

        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Cierra el panel de confirmación para cambiar de elección.
    /// </summary>
    public void CloseConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Aplica la transacción directamente al GestorBancario (guardando en el JSON) sin cambiar de escena.
    /// </summary>
    public void SaveToJSONOnly()
    {
        if (opcionSeleccionada != null && gestorBancario != null)
        {
            int montoAbsoluto = Mathf.Abs(opcionSeleccionada.monto);

            if (opcionSeleccionada.monto > 0)
            {
                // Registra el ingreso en el JSON
                gestorBancario.AgregarTransaccion(montoAbsoluto, opcionSeleccionada.descripcionOp, true);
            }
            else if (opcionSeleccionada.monto < 0)
            {
                // Registra el gasto en el JSON
                gestorBancario.AgregarTransaccion(montoAbsoluto, opcionSeleccionada.descripcionOp, false);
            }

            Debug.Log($"Transacción guardada correctamente en JSON: {opcionSeleccionada.descripcionOp} (${opcionSeleccionada.monto})");
        }
        else
        {
            Debug.LogWarning("No se pudo guardar: GestorBancario no asignado o no se seleccionó ninguna opción.");
        }

        // Cerrar el panel tras registrar la transacción
        CloseConfirmationPanel();
    }

    /// <summary>
    /// Cambia la visibilidad de los botones de opciones.
    /// </summary>
    private void SetActionButtonsState(bool state)
    {
        if (opciones == null) return;

        foreach (OpcionBoton op in opciones)
        {
            if (op != null && op.boton != null)
            {
                op.boton.gameObject.SetActive(state);
            }
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null) nextButton.onClick.RemoveListener(AdvanceText);
        if (backButton != null) backButton.onClick.RemoveListener(CloseConfirmationPanel);
        if (confirmNextButton != null) confirmNextButton.onClick.RemoveListener(SaveToJSONOnly);
    }
}