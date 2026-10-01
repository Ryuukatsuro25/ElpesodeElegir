using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para cambiar de escenas

public class ControladorEstado : MonoBehaviour
{
    [Header("Asigna las imágenes con Fill")]
    public Image barraIngresos;
    public Image barraBienestarMental;

    [Header("Asigna los Paneles")]
    public GameObject panelApagar;  // El panel que va a desaparecer
    public GameObject panelPrender; // El panel que va a aparecer en el gris

    [Header("Configuración")]
    public float velocidad = 0.5f;
    public string nombreEscenaNueva = "NombreDeTuSiguienteEscena"; // Escribe el nombre exacto de la escena

    private int numeroClics = 0;

    // Estados de animación automática: 
    // 0 = Quieto
    // 1 = Bajando Ingresos, 2 = Subiendo Ingresos
    // 3 = Bajando Bienestar, 4 = Subiendo Bienestar
    private int estadoAnimacion = 0;

    void Update()
    {
        // ------------------ ANIMACIÓN DE INGRESOS ------------------
        if (estadoAnimacion == 1) // Bajando Ingresos
        {
            if (barraIngresos != null)
            {
                barraIngresos.fillAmount -= velocidad * Time.deltaTime;
                if (barraIngresos.fillAmount <= 0f)
                {
                    estadoAnimacion = 2; // Al llegar a 0, cambia a estado de llenado
                }
            }
        }
        else if (estadoAnimacion == 2) // Subiendo Ingresos
        {
            if (barraIngresos != null)
            {
                barraIngresos.fillAmount += velocidad * Time.deltaTime;
                if (barraIngresos.fillAmount >= 1f)
                {
                    estadoAnimacion = 0; // Al llegar a 1, se queda quieto
                }
            }
        }

        // ------------------ ANIMACIÓN DE BIENESTAR ------------------
        else if (estadoAnimacion == 3) // Bajando Bienestar
        {
            if (barraBienestarMental != null)
            {
                barraBienestarMental.fillAmount -= velocidad * Time.deltaTime;
                if (barraBienestarMental.fillAmount <= 0f)
                {
                    estadoAnimacion = 4; // Al llegar a 0, cambia a estado de llenado
                }
            }
        }
        else if (estadoAnimacion == 4) // Subiendo Bienestar
        {
            if (barraBienestarMental != null)
            {
                barraBienestarMental.fillAmount += velocidad * Time.deltaTime;
                if (barraBienestarMental.fillAmount >= 1f)
                {
                    estadoAnimacion = 0; // Al llegar a 1, se queda quieto
                }
            }
        }
    }

    // Función asignada al botón (On Click)
    public void PresionarBoton()
    {
        numeroClics++; // Sumamos 1 al contador cada vez que presionas

        if (numeroClics == 1)
        {
            // PRIMER CLIC: Inicia la secuencia de la barra de ingresos
            estadoAnimacion = 1;
        }
        else if (numeroClics == 2)
        {
            // SEGUNDO CLIC: Apaga un panel, prende otro e inicia la secuencia de bienestar
            if (panelApagar != null) panelApagar.SetActive(false);
            if (panelPrender != null) panelPrender.SetActive(true);

            estadoAnimacion = 3;
        }
        else if (numeroClics >= 3)
        {
            // TERCER CLIC: Cambia de escena
            // Asegúrate de que la escena esté añadida en File > Build Settings
            SceneManager.LoadScene(nombreEscenaNueva);
        }
    }
}