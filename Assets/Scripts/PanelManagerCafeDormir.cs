using System.Collections;
using UnityEngine;

public class PanelFlowManager : MonoBehaviour
{
    // Método genérico y reutilizable para ir a CUALQUIER panel sin duplicar nada
    public void CambiarPanel(GameObject panelDestino)
    {
        if (panelDestino != null)
        {
            // Iniciamos una pequeña corrutina para forzar el reseteo limpio del panel
            StartCoroutine(ActivarPanelFluido(panelDestino));

            Debug.Log("Cambiando al panel compartido: " + panelDestino.name);
        }
        else
        {
            Debug.LogWarning("¡Ojo! El panel de destino está vacío en el botón.");
        }
    }

    private IEnumerator ActivarPanelFluido(GameObject panel)
    {
        // 1. Lo apagamos por si acaso ya estaba medio activo en memoria
        panel.SetActive(false);

        // 2. Esperamos un fotograma para que Unity limpie su estado visual
        yield return null;

        // 3. Lo encendemos de nuevo limpio
        panel.SetActive(true);

        // 4. Lo obligamos a colocarse al frente de todos los elementos hermanos en el Canvas
        panel.transform.SetAsLastSibling();
    }
}