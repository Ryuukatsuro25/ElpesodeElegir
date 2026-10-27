
using UnityEngine;

public class BancoCompraController : MonoBehaviour
{
    [Header("Paneles del banco")]
    [SerializeField] private GameObject panelBancoComprar;
    [SerializeField] private GameObject panelBancoNoComprar;

    [Header("Panel anterior (opcional)")]
    [SerializeField] private GameObject panelBancoActual;

    public void Comprar()
    {
        MostrarPanel(panelBancoComprar);
    }

    public void NoComprar()
    {
        MostrarPanel(panelBancoNoComprar);
    }

    private void MostrarPanel(GameObject panelDestino)
    {
        if (panelDestino == null)
        {
            Debug.LogWarning("No has asignado el panel de destino.");
            return;
        }

        // Ocultar el panel actual si fue asignado
        if (panelBancoActual != null)
        {
            panelBancoActual.SetActive(false);
        }

        // Mostrar el panel seleccionado
        panelBancoComprar.SetActive(false);
        panelBancoNoComprar.SetActive(false);

        panelDestino.SetActive(true);
    }
}
