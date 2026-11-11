
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

[System.Serializable]
public class DatosBienestarLunes
{
    public float bienestarActual;
}

public class PopUpManagerLunes : MonoBehaviour
{
    [Header("Configuracion del PopUp")]
    [SerializeField] private TextMeshProUGUI textoPopUp;
    [SerializeField] private float tiempoEnPantalla = 2f;

    [Header("Configuracion de la barra")]
    [SerializeField] private Image barraBienestarMental;
    [SerializeField] private float bienestarMaximo = 100f;
    [SerializeField] private float bienestarInicial = 50f;
    [SerializeField] private float velocidadAnimacion = 3f;

    private float bienestarActual;
    private float bienestarObjetivo;

    private string rutaArchivo;
    private DatosBienestarLunes datos;

    public float BienestarActual
    {
        get { return bienestarActual; }
    }

    private void Start()
    {
        // Archivo independiente del sistema original.
        rutaArchivo = Application.persistentDataPath +
                     "/datosBienestarLunes.json";

        CargarDatos();
    }

    // CARGAR DATOS

    private void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);

            datos = JsonUtility.FromJson<DatosBienestarLunes>(json);

            if (datos != null)
            {
                bienestarActual = datos.bienestarActual;
            }
            else
            {
                bienestarActual = bienestarInicial;
                datos = new DatosBienestarLunes();
            }
        }
        else
        {
            bienestarActual = bienestarInicial;
            datos = new DatosBienestarLunes();
        }

        bienestarActual = Mathf.Clamp(
            bienestarActual,
            0f,
            bienestarMaximo
        );

        bienestarObjetivo = bienestarActual / bienestarMaximo;

        ActualizarBarraInmediata();
        GuardarDatos();
    }

    // APLICAR CAMBIOS SEGUN LAS DECISIONES

    public void AplicarCambioBienestar(float cambio, string mensaje = "")
    {
        float bienestarAnterior = bienestarActual;

        bienestarActual += cambio;

        bienestarActual = Mathf.Clamp(
            bienestarActual,
            0f,
            bienestarMaximo
        );

        float cambioReal = bienestarActual - bienestarAnterior;

        bienestarObjetivo = bienestarActual / bienestarMaximo;

        MostrarPopUp(cambioReal, mensaje);

        GuardarDatos();

        Debug.Log(
            "Bienestar del lunes: " +
            bienestarAnterior + " -> " + bienestarActual
        );
    }

    // MOSTRAR POPUP

    private void MostrarPopUp(float cambioReal, string mensaje)
    {
        if (textoPopUp == null)
            return;

        if (!string.IsNullOrEmpty(mensaje))
        {
            textoPopUp.text = mensaje;
        }
        else if (cambioReal > 0)
        {
            textoPopUp.text =
                "+" + cambioReal + " de bienestar mental";
        }
        else if (cambioReal < 0)
        {
            textoPopUp.text =
                cambioReal + " de bienestar mental";
        }
        else
        {
            textoPopUp.text = "Bienestar mental sin cambios";
        }

        textoPopUp.gameObject.SetActive(true);

        CancelInvoke(nameof(OcultarPopUp));

        Invoke(nameof(OcultarPopUp), tiempoEnPantalla);
    }

    private void OcultarPopUp()
    {
        if (textoPopUp != null)
        {
            textoPopUp.gameObject.SetActive(false);
        }
    }

    // ANIMACION DE LA BARRA

    private void Update()
    {
        if (barraBienestarMental != null)
        {
            barraBienestarMental.fillAmount = Mathf.Lerp(
                barraBienestarMental.fillAmount,
                bienestarObjetivo,
                Time.deltaTime * velocidadAnimacion
            );
        }
    }

    private void ActualizarBarraInmediata()
    {
        if (barraBienestarMental != null)
        {
            barraBienestarMental.fillAmount = bienestarObjetivo;
        }
    }

    // GUARDAR DATOS

    private void GuardarDatos()
    {
        if (datos == null)
        {
            datos = new DatosBienestarLunes();
        }

        datos.bienestarActual = bienestarActual;

        string json = JsonUtility.ToJson(datos, true);

        File.WriteAllText(rutaArchivo, json);
    }

    // REINICIAR PARA UNA NUEVA PARTIDA

    public void IniciarNuevaPartida()
    {
        bienestarActual = bienestarInicial;

        bienestarObjetivo = bienestarActual / bienestarMaximo;

        GuardarDatos();

        ActualizarBarraInmediata();

        if (textoPopUp != null)
        {
            textoPopUp.gameObject.SetActive(false);
        }

        Debug.Log("Nueva partida. Bienestar inicial: " + bienestarActual);
    }

    // CONSULTAR BIENESTAR ACTUAL

    public float ObtenerBienestarActual()
    {
        return bienestarActual;
    }
}
