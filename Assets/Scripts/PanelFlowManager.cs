using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelFlowManager : MonoBehaviour
{
    [Header("Panel actual")]
    [SerializeField] private GameObject panelActual;

    [Header("Paneles del martes")]
    [SerializeField] private GameObject[] panelesDelMartes;

    private void Start()
    {
        RestaurarProgreso();
    }

    // Navegación entre paneles.
    public void CambiarPanel(GameObject panelDestino)
    {
        if (panelDestino != null)
        {
            panelActual = panelDestino;

            StartCoroutine(ActivarPanelFluido(panelDestino));

            Debug.Log("Cambiando al panel compartido: " +
                panelDestino.name);
        }
        else
        {
            Debug.LogWarning(
                "El panel de destino está vacío en el botón."
            );
        }
    }

    private IEnumerator ActivarPanelFluido(GameObject panel)
    {
        panel.SetActive(false);

        yield return null;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }

    // Guardar progreso del martes.
    public void GuardarProgreso()
    {
        if (panelActual == null)
        {
            Debug.LogError(
                "No hay un panel actual asignado en PanelFlowManager."
            );
            return;
        }

        string escenaActual =
            SceneManager.GetActiveScene().name;

        SistemaGuardadoSemanal.GuardarCheckpoint(
            escenaActual,
            panelActual.name
        );

        Debug.Log(
            "Progreso del martes guardado. Panel: " +
            panelActual.name
        );
    }

    // Recuperar el último panel guardado.
    private void RestaurarProgreso()
    {
        string escenaActual =
            SceneManager.GetActiveScene().name;

        if (!SistemaGuardadoSemanal.IntentarObtenerPanel(
            escenaActual, out string nombrePanel))
        {
            Debug.Log("No hay checkpoint para esta escena. " +
                "Se conserva el panel inicial.");

            return;
        }

        GameObject panelEncontrado = null;

        foreach (GameObject panel in panelesDelMartes)
        {
            if (panel != null && panel.name == nombrePanel)
            {
                panelEncontrado = panel;
                break;
            }
        }

        if (panelEncontrado == null)
        {
            Debug.LogError(
                "No se encontró el panel guardado: " +
                nombrePanel +
                ". Comprueba el arreglo panelesDelMartes."
            );

            return;
        }

        // Ocultar los paneles registrados.
        foreach (GameObject panel in panelesDelMartes)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        // Mostrar el panel guardado.
        panelActual = panelEncontrado;
        panelActual.SetActive(true);
        panelActual.transform.SetAsLastSibling();

        Debug.Log(
            "Partida restaurada correctamente en: " +
            panelActual.name
        );
    }
}