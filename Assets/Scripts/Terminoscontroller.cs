using UnityEngine;

public class TerminosController : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject terminosPanel;     // El panel de términos y condiciones
    public GameObject menuPrincipalPanel; // El panel con los botones iniciales

    // Método para mostrar términos y ocultar el menú principal
    public void AbrirTerminos()
    {
        if (terminosPanel != null)
        {
            terminosPanel.SetActive(true);
        }

        if (menuPrincipalPanel != null)
        {
            menuPrincipalPanel.SetActive(false); // Oculta los botones de inicio
        }
    }

    // Método para cerrar términos y volver a mostrar el menú principal
    public void CerrarTerminos()
    {
        if (terminosPanel != null)
        {
            terminosPanel.SetActive(false);
        }

        if (menuPrincipalPanel != null)
        {
            menuPrincipalPanel.SetActive(true); // Muestra los botones de inicio de nuevo
        }
    }
}