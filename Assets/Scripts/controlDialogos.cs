using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SequenceTextManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Componente de texto de TextMeshPro donde se mostrarán las frases.")]
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Tooltip("Botón principal para avanzar de texto.")]
    [SerializeField] private Button nextButton;

    [Tooltip("Panel UI que se activará al llegar al final del texto.")]
    [SerializeField] private GameObject endPanel;

    [Tooltip("Arreglo opcional de botones que se activarán al finalizar.")]
    [SerializeField] private Button[] actionButtons;

    [Header("Content")]
    [TextArea(2, 5)]
    [Tooltip("Lista de textos que se mostrarán en orden.")]
    [SerializeField] private string[] textArray;

    private int currentIndex = 0;

    private void Start()
    {
        // Ocultar el panel final al inicio
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        // Ocultar los botones de acción al inicio
        SetActionButtonsState(false);

        // Validar que exista al menos un texto en el arreglo
        if (textArray != null && textArray.Length > 0)
        {
            textDisplay.text = textArray[currentIndex];

            // Si solo hay 1 texto en el array, de una vez configuramos el estado final para cuando se presione el botón
            if (textArray.Length == 1)
            {
                // Mantiene el botón visible para que el usuario pueda presionarlo y pasar al panel
            }
        }
        else
        {
            Debug.LogWarning("El arreglo de textos está vacío.");
        }

        // Asignar el evento al botón principal por código
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceText);
        }
    }

    /// <summary>
    /// Avanza al siguiente texto del arreglo o activa el panel al finalizar.
    /// </summary>
    public void AdvanceText()
    {
        if (textArray == null || textArray.Length == 0) return;

        // Si aún quedan más textos por mostrar
        if (currentIndex < textArray.Length - 1)
        {
            currentIndex++;
            textDisplay.text = textArray[currentIndex];

            // Si el texto al que acabamos de avanzar es el último de la lista
            if (currentIndex == textArray.Length - 1)
            {
                // Opcional: si quieres que el panel se active en el último texto directamente, 
                // o al presionar el botón una vez más estando en el último texto.
            }
        }
        else
        {
            // Ya estábamos en el último texto y se volvió a presionar el botón -> Activar final
            OnSequenceEnd();
        }
    }

    /// <summary>
    /// Maneja la transición al finalizar la secuencia de textos.
    /// </summary>
    private void OnSequenceEnd()
    {
        // Desactivar el botón de avanzar
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        // Activar el panel final
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        // Activar los botones de acción opcionales
        SetActionButtonsState(true);
    }

    /// <summary>
    /// Enciende o apaga todos los botones asignados en el array de botones de acción.
    /// </summary>
    private void SetActionButtonsState(bool state)
    {
        if (actionButtons == null) return;

        foreach (Button btn in actionButtons)
        {
            if (btn != null)
            {
                btn.gameObject.SetActive(state);
            }
        }
    }

    private void OnDestroy()
    {
        // Limpiar el listener para evitar fugas de memoria
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceText);
        }
    }
}