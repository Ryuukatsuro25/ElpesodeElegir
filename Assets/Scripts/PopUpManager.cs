using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO; // Necesario para guardar y leer archivos

// Creamos una clase serializable para guardar los datos
[System.Serializable]
public class DatosBienestar
{
    public float bienestarActual;
}

public class PopUpManager : MonoBehaviour
{
    [Header("Configuración del PopUp")]
    public TextMeshProUGUI textoPopUp;
    public float tiempoEnPantalla = 2f;

    [Header("Configuración de la Barra")]
    public Image barraBienestarMental;
    public float bienestarMaximo = 100f;
    public float bienestarInicial = 50f; // Solo se usará la primera vez que se juegue
    public float velocidadAnimacion = 3f;

    private float bienestarActual;
    private float bienestarObjetivo;

    // Variables para el guardado
    private string rutaArchivo;
    private DatosBienestar datos;

    void Start()
    {
        // Definimos dónde se guardará el archivo en el celular/PC
        rutaArchivo = Application.persistentDataPath + "/datosBienestar.json";

        // Cargamos los datos al iniciar
        CargarDatos();
    }

    private void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            // Si el archivo existe, leemos el puntaje guardado
            string json = File.ReadAllText(rutaArchivo);
            datos = JsonUtility.FromJson<DatosBienestar>(json);
            bienestarActual = datos.bienestarActual;
        }
        else
        {
            // Si no existe (partida nueva), iniciamos con el valor por defecto (50)
            datos = new DatosBienestar();
            bienestarActual = bienestarInicial;
            GuardarDatos();
        }

        // Ajustamos la barra visualmente al cargar
        bienestarObjetivo = bienestarActual / bienestarMaximo;
        if (barraBienestarMental != null)
        {
            barraBienestarMental.fillAmount = bienestarObjetivo;
        }
    }

    private void GuardarDatos()
    {
        // Actualizamos la clase y la convertimos a JSON
        datos.bienestarActual = bienestarActual;
        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivo, json);
    }

    public void MostrarPopUpAleatorio()
    {
        int numeroAleatorio = Random.Range(-10, 11);

        if (numeroAleatorio == 0)
        {
            numeroAleatorio = 1;
        }

        if (numeroAleatorio > 0)
        {
            textoPopUp.text = $"+{numeroAleatorio} de bienestar mental";
        }
        else
        {
            textoPopUp.text = $"{numeroAleatorio} de bienestar mental";
        }

        textoPopUp.gameObject.SetActive(true);
        CancelInvoke("OcultarPopUp");
        Invoke("OcultarPopUp", tiempoEnPantalla);

        // --- ACTUALIZACIÓN Y GUARDADO ---

        bienestarActual += numeroAleatorio;
        bienestarActual = Mathf.Clamp(bienestarActual, 0, bienestarMaximo);

        bienestarObjetivo = bienestarActual / bienestarMaximo;

        // Guardamos el nuevo valor automáticamente en el disco
        GuardarDatos();
    }

    private void OcultarPopUp()
    {
        textoPopUp.gameObject.SetActive(false);
    }

    void Update()
    {
        if (barraBienestarMental != null)
        {
            barraBienestarMental.fillAmount = Mathf.Lerp(barraBienestarMental.fillAmount, bienestarObjetivo, Time.deltaTime * velocidadAnimacion);
        }
    }

    // Método para desarrollo: Asígnalo a un botón si necesitas reiniciar el bienestar a 50
    public void BorrarDatosBienestar()
    {
        if (File.Exists(rutaArchivo))
        {
            File.Delete(rutaArchivo);
            Debug.Log("Datos de bienestar borrados. Reinicia para volver a 50.");
        }
    }
}