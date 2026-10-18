using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuHamburger : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panel; // Arrastra tu panel aquí

    [Header("Opciones Opcionales")]
    [SerializeField] private bool cerrarAlIniciar = true;

    private void Start()
    {
        // Opcional: Asegurarse de que el panel comience desactivado al arrancar el juego
        if (cerrarAlIniciar && panel != null)
        {
            panel.SetActive(false);
        }
    }

    // Método para abrir el panel
    public void AbrirPanel()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log("Panel abierto");
        }
    }

    // Método para cerrar el panel
    public void CerrarPanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log("Panel cerrado");
        }
    }

    // Método extra por si algún día quieres alternarlo con un solo botón
    public void AlternarPanel()
    {
        if (panel != null)
        {
            bool estadoActual = panel.activeSelf;
            panel.SetActive(!estadoActual);
        }
    }
}
