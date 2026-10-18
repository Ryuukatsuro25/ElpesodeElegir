using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject termsPanel;

    // Método para mostrar Términos y ocultar Opciones
    public void OpenTerms()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (termsPanel != null) termsPanel.SetActive(true);
    }

    // Método para cerrar Términos y volver a Opciones
    public void CloseTerms()
    {
        if (termsPanel != null) termsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }
}