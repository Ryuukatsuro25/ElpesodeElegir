using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    [SerializeField] private GestorBancario gestorBancario;

    [Header("UI Elements - Dialogs")]
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject endPanel;

    [Header("Velocidad de escritura")]
    [SerializeField] private float velocidadNormal = 0.04f;
    [SerializeField] private float velocidadRapida = 0.008f;

    [Header("UI Elements - Action Buttons Config")]
    [SerializeField] private OpcionBoton[] opciones;

    [Header("UI Elements - Confirmation Panel")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmNextButton;

    [Header("Content")]
    [TextArea(2, 5)]
    [SerializeField] private string[] textArray;

    private int currentIndex = 0;
    private OpcionBoton opcionSeleccionada;

    private Coroutine escrituraCoroutine;
    private bool escribiendo = false;

    private void Start()
    {
        if (gestorBancario == null)
        {
            gestorBancario = FindObjectOfType<GestorBancario>();
        }

        if (endPanel != null) endPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);

        SetActionButtonsState(false);
        SetupActionButtons();

        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseConfirmationPanel);
        }

        if (confirmNextButton != null)
        {
            confirmNextButton.onClick.AddListener(SaveToJSONOnly);
        }

        if (textArray != null && textArray.Length > 0)
        {
            MostrarTexto(currentIndex);
        }
        else
        {
            Debug.LogWarning("El arreglo de textos está vacío.");
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceText);
        }
    }

    private void MostrarTexto(int indice)
    {
        if (textDisplay == null) return;

        if (escrituraCoroutine != null)
        {
            StopCoroutine(escrituraCoroutine);
        }

        textDisplay.text = textArray[indice] ?? string.Empty;
        textDisplay.maxVisibleCharacters = 0;

        escrituraCoroutine = StartCoroutine(EscribirTexto());
    }

    private IEnumerator EscribirTexto()
    {
        escribiendo = true;

        // Esperar un frame asegura que TextMeshPro haya inicializado textInfo
        yield return null;

        if (textDisplay == null)
        {
            escribiendo = false;
            yield break;
        }

        textDisplay.ForceMeshUpdate();

        bool textoRapido = PlayerPrefs.GetInt("TextoRapido", 0) == 1;
        float velocidad = textoRapido ? velocidadRapida : velocidadNormal;

        int totalCaracteres = (textDisplay.textInfo != null && textDisplay.textInfo.characterCount > 0)
            ? textDisplay.textInfo.characterCount
            : textDisplay.text.Length;

        for (int i = 0; i < totalCaracteres; i++)
        {
            if (textDisplay == null) yield break;

            textDisplay.maxVisibleCharacters = i + 1;
            yield return new WaitForSeconds(velocidad);
        }

        if (textDisplay != null)
        {
            textDisplay.maxVisibleCharacters = totalCaracteres;
        }

        escribiendo = false;
        escrituraCoroutine = null;
    }

    public void AdvanceText()
    {
        if (textArray == null || textArray.Length == 0) return;

        // Si todavía se está escribiendo, mostrar toda la frase.
        if (escribiendo)
        {
            if (escrituraCoroutine != null)
            {
                StopCoroutine(escrituraCoroutine);
                escrituraCoroutine = null;
            }

            if (textDisplay != null)
            {
                textDisplay.ForceMeshUpdate();
                int total = (textDisplay.textInfo != null && textDisplay.textInfo.characterCount > 0)
                    ? textDisplay.textInfo.characterCount
                    : textDisplay.text.Length;

                textDisplay.maxVisibleCharacters = total;
            }

            escribiendo = false;
            return;
        }

        // Avanzar a la siguiente frase.
        if (currentIndex < textArray.Length - 1)
        {
            currentIndex++;
            MostrarTexto(currentIndex);
        }
        else
        {
            OnSequenceEnd();
        }
    }

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

    private void SetupActionButtons()
    {
        if (opciones == null) return;

        foreach (OpcionBoton op in opciones)
        {
            if (op == null || op.boton == null) continue;

            OpcionBoton seleccion = op;
            op.boton.onClick.AddListener(() => OnOptionSelected(seleccion));
        }
    }

    private void OnOptionSelected(OpcionBoton opcion)
    {
        opcionSeleccionada = opcion;

        string nombreMostrar = string.IsNullOrEmpty(opcion.descripcionOp)
            ? (opcion.boton.GetComponentInChildren<TextMeshProUGUI>() != null
                ? opcion.boton.GetComponentInChildren<TextMeshProUGUI>().text
                : opcion.boton.gameObject.name)
            : opcion.descripcionOp;

        if (confirmationText != null)
        {
            confirmationText.text = $"Escoges {nombreMostrar}";
        }

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

    public void CloseConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    public void SaveToJSONOnly()
    {
        if (opcionSeleccionada != null && gestorBancario != null)
        {
            int montoAbsoluto = Mathf.Abs(opcionSeleccionada.monto);

            if (opcionSeleccionada.monto > 0)
            {
                gestorBancario.AgregarTransaccion(
                    montoAbsoluto,
                    opcionSeleccionada.descripcionOp,
                    true
                );
            }
            else if (opcionSeleccionada.monto < 0)
            {
                gestorBancario.AgregarTransaccion(
                    montoAbsoluto,
                    opcionSeleccionada.descripcionOp,
                    false
                );
            }

            Debug.Log("Transacción guardada correctamente en JSON: "
                + opcionSeleccionada.descripcionOp);
        }
        else
        {
            Debug.LogWarning(
                "No se pudo guardar: GestorBancario no asignado o no se seleccionó ninguna opción."
            );
        }

        CloseConfirmationPanel();
    }

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

    private void OnDisable()
    {
        if (escrituraCoroutine != null)
        {
            StopCoroutine(escrituraCoroutine);
            escrituraCoroutine = null;
            escribiendo = false;
        }
    }

    private void OnDestroy()
    {
        if (escrituraCoroutine != null)
        {
            StopCoroutine(escrituraCoroutine);
        }

        if (nextButton != null)
            nextButton.onClick.RemoveListener(AdvanceText);

        if (backButton != null)
            backButton.onClick.RemoveListener(CloseConfirmationPanel);

        if (confirmNextButton != null)
            confirmNextButton.onClick.RemoveListener(SaveToJSONOnly);
    }
}