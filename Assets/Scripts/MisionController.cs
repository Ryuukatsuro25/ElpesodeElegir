using UnityEngine;

public class MisionBancoManager : MonoBehaviour
{
    [Header("Paneles Clave para la Misión")]
    public GameObject panelDecisionTomarCafe;  // Paso 1
    public GameObject panelSigimprovisar;       // Paso 2
    public GameObject panelImprocasa;           // Paso 3

    [Header("Panel de Recompensa")]
    public GameObject panelMedalla;             // Panel de la recompensa

    // Control de progreso (Paso actual)
    private int pasoActualMision = 0;

    // Candados para evitar que se repita el mismo paso haciendo clics repetidos en un solo panel
    private bool paso1Completado = false;
    private bool paso2Completado = false;
    private bool paso3Completado = false;

    public void IntentarConsultarBanco()
    {
        // PASO 1: Desde el café
        if (!paso1Completado && panelDecisionTomarCafe != null && panelDecisionTomarCafe.activeInHierarchy)
        {
            pasoActualMision = 1;
            paso1Completado = true;
            Debug.Log("Misión Banco [Paso 1/3]: ¡Registrado en el café!");
            return;
        }

        // PASO 2: Desde Sigimprovisar (Exige que el paso 1 ya esté hecho y no se repita en este mismo)
        if (paso1Completado && !paso2Completado && panelSigimprovisar != null && panelSigimprovisar.activeInHierarchy)
        {
            pasoActualMision = 2;
            paso2Completado = true;
            Debug.Log("Misión Banco [Paso 2/3]: ¡Registrado en improvisación!");
            return;
        }

        // PASO 3: Desde Improcasa (Exige que el paso 2 ya esté hecho)
        if (paso2Completado && !paso3Completado && panelImprocasa != null && panelImprocasa.activeInHierarchy)
        {
            pasoActualMision = 3;
            paso3Completado = true;
            Debug.Log("Misión Banco [Paso 3/3]: ¡Registrado en la casa! Misión lista para cerrar.");
            return;
        }

        Debug.Log("Consulta de banco realizada, pero no cumple con la secuencia o ya fue registrada.");
    }

    public void CerrarBancoYVerificarMision()
    {
        // Si ya completó los 3 pasos y está cerrando el banco en la última fase:
        if (pasoActualMision >= 3)
        {
            Debug.Log("¡Misión Cumplida! Forzando aparición del panel medalla.");

            if (panelMedalla != null)
            {
                // 1. Activa el objeto por completo
                panelMedalla.SetActive(true);

                // 2. FORZAR AL FRENTE: Lo mueve al final de la lista de renderizado de su padre
                panelMedalla.transform.SetAsLastSibling();

                // 3. Asegurar componente Canvas opcional si lo tuviera independiente
                CanvasGroup canvasGroup = panelMedalla.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
            else
            {
                Debug.LogWarning("¡Advertencia! El 'PanelMedalla' no está asignado en el Inspector.");
            }
        }
        else
        {
            Debug.Log("Aún te faltan pasos en la misión del banco para reclamar la medalla.");
        }
    }
}