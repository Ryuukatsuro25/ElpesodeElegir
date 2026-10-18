
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorTutorial : MonoBehaviour
{
    [Header("Introduccion")]
    public GameObject panelIntroduccion;
    public Button botonIniciarTutorial;

    [Header("Los 4 paneles del tutorial")]
    public GameObject[] paneles = new GameObject[4];

    [Header("Los 4 botones Siguiente")]
    public Button[] botonesSiguiente = new Button[4];

    [Header("Escena del menu")]
    public string escenaMenu = "Menu1";

    private void Start()
    {
        // Mostrar la introduccion
        if (panelIntroduccion != null)
            panelIntroduccion.SetActive(true);

        // Ocultar los cuatro paneles
        for (int i = 0; i < paneles.Length; i++)
        {
            if (paneles[i] != null)
                paneles[i].SetActive(false);
        }

        // Conectar el boton que inicia el tutorial
        if (botonIniciarTutorial != null)
            botonIniciarTutorial.onClick.AddListener(IniciarTutorial);

        // Conectar cada boton con su panel correspondiente
        for (int i = 0; i < botonesSiguiente.Length; i++)
        {
            int indice = i;

            if (botonesSiguiente[indice] != null)
            {
                botonesSiguiente[indice].onClick.AddListener(
                    () => Avanzar(indice)
                );
            }
        }
    }

    public void IniciarTutorial()
    {
        if (panelIntroduccion != null)
            panelIntroduccion.SetActive(false);

        MostrarPanel(0);
    }

    private void Avanzar(int indice)
    {
        // El cuarto boton vuelve al menu
        if (indice == 3)
        {
            SceneManager.LoadScene(escenaMenu);
            return;
        }

        // Cambiar al siguiente panel
        MostrarPanel(indice + 1);
    }

    private void MostrarPanel(int indice)
    {
        for (int i = 0; i < paneles.Length; i++)
        {
            if (paneles[i] != null)
                paneles[i].SetActive(i == indice);
        }
    }

    private void OnDestroy()
    {
        if (botonIniciarTutorial != null)
            botonIniciarTutorial.onClick.RemoveListener(IniciarTutorial);

        for (int i = 0; i < botonesSiguiente.Length; i++)
        {
            int indice = i;

            if (botonesSiguiente[indice] != null)
                botonesSiguiente[indice].onClick.RemoveAllListeners();
        }
    }
}