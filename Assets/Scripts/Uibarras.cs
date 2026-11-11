using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBarrasController : MonoBehaviour
{
    // Instancia estática para llamarlo desde cualquier botón sin arrastrar referencias
    public static UIBarrasController Instance;

    [Header("Valores de Estado")]
    public float bienestarMental = 50f;
    public float bienestarMaximo = 100f;
    public float dinero = 100000f;
    public float dineroMaximo = 100000f; // Límite máximo para normalizar la barra de 0 a 1

    [Header("Referencias de UI (Imágenes o Barras)")]
    public Image barraBienestarImage; // Componente Image en modo Filled
    public Image barraDineroImage;    // Componente Image en modo Filled para el dinero

    private void Awake()
    {
        // Guardamos la instancia global para esta escena
        Instance = this;
    }

    private void Start()
    {
        ActualizarUI();
    }

    // Métodos globales invocables desde cualquier script o botón
    public void ModificarBienestar(float cantidad)
    {
        bienestarMental = Mathf.Clamp(bienestarMental + cantidad, 0f, bienestarMaximo);
        ActualizarUI();
    }

    public void ModificarDinero(float cantidad)
    {
        dinero += cantidad;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (barraBienestarImage != null)
        {
            barraBienestarImage.fillAmount = bienestarMental / bienestarMaximo;
        }

        // Actualiza la barra de dinero de forma proporcional (0 a 1)
        if (barraDineroImage != null)
        {
            barraDineroImage.fillAmount = Mathf.Clamp01(dinero / dineroMaximo);
        }
    }

    // Método para mantener las barras siempre al frente del Canvas
    public void TraerBarrasAlFrente()
    {
        transform.SetAsLastSibling();
    }

    // Método para el botón de Tomar Café (resta 2000 y actualiza la barra)
    public void ComprarCafe()
    {
        ModificarDinero(-2000f);

        // Si tomar café también te sube bienestar mental, puedes descomentar esto:
        // ModificarBienestar(10f); 
    }
}