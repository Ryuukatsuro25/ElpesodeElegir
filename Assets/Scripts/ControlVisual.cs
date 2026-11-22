using UnityEngine;

public class ControladorVisibilidadHUD : MonoBehaviour
{
    [Header("Elementos Globales")]
    public GameObject objetoBarras;       // Arrastra aquí tus barras
    public GameObject botonBanco;         // Arrastra aquí tu botón del banco

    // Método para ocultar el HUD (Asócialo a Misión y Medalla)
    public void OcultarHUD()
    {
        if (objetoBarras != null) objetoBarras.SetActive(false);
        if (botonBanco != null) botonBanco.SetActive(false);
    }

    // Método para mostrar el HUD (Asócialo a los demás paneles: Café, Tienda, etc.)
    public void MostrarHUD()
    {
        if (objetoBarras != null) objetoBarras.SetActive(true);
        if (botonBanco != null) objetoBarras.SetActive(true);
    }
}