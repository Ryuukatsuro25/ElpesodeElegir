
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BienestarColaboracionMiercoles : MonoBehaviour
{
    [Header("Indicador de bienestar")]
    [SerializeField] private Slider barraBienestar;

    [SerializeField, Range(0f, 100f)]
    private float bienestarInicial = 50f;

    [SerializeField, Range(0f, 50f)]
    private float aumentoBienestar = 15f;

    [SerializeField] private float duracionAnimacion = 1.5f;

    [Header("Texto de retroalimentacion")]
    [SerializeField] private TMP_Text textoRetroalimentacion;

    [SerializeField] private float duracionTexto = 2f;
    [SerializeField] private float desplazamientoTexto = 40f;

    [Header("Control del boton Siguiente")]
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private bool decisionTomada = false;


    private float bienestarActual;
    private Coroutine animacionBarra;
    private Coroutine animacionTexto;
    private bool ayudaRegistrada;

    private void Start()
    {
        bienestarInicial = Mathf.Clamp(
            bienestarInicial, 0f, 100f
        );

        bienestarActual = bienestarInicial;

        if (barraBienestar != null)
        {
            barraBienestar.minValue = 0f;
            barraBienestar.maxValue = 100f;
            barraBienestar.value = bienestarActual;
        }
        else
        {
            Debug.LogWarning(
                "Asigna el Slider de bienestar en el Inspector."
            );
        }

        if (textoRetroalimentacion != null)
        {
            textoRetroalimentacion.gameObject.SetActive(false);
        }

        BloquearSiguiente();
    }

    
    private void BloquearSiguiente()
    {
        decisionTomada = false;

        if (botonSiguiente != null)
        { 
            botonSiguiente.interactable = false;
        }
    }

    private void DesbloquearSiguiente()
    {
        decisionTomada = true;

        if (botonSiguiente != null)
        {
            botonSiguiente.interactable = true;
        }
    }


    // Se ejecuta al pulsar el boton Preguntar.
    public void RegistrarAyudaCompaneros()
    {
        if (ayudaRegistrada)
        {
            return;
        }

        ayudaRegistrada = true;

        DesbloquearSiguiente();

        float bienestarAnterior = bienestarActual;

        bienestarActual = Mathf.Clamp(
            bienestarActual + aumentoBienestar,
            0f,
            100f
        );

        float puntosGanados = bienestarActual - bienestarAnterior;

        if (barraBienestar != null)
        {
            if (animacionBarra != null)
            {
                StopCoroutine(animacionBarra);
            }

            animacionBarra = StartCoroutine(
                AnimarBarra(bienestarActual)
            );
        }

        if (textoRetroalimentacion != null && puntosGanados > 0f)
        {
            if (animacionTexto != null)
            {
                StopCoroutine(animacionTexto);
            }

            animacionTexto = StartCoroutine(
                MostrarPuntos(puntosGanados)
            );
        }

        // No se cambia de panel.
        // El jugador permanece en el panel actual.
    }

    private IEnumerator AnimarBarra(float valorFinal)
    {
        float valorInicial = barraBienestar.value;
        float tiempo = 0f;

        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / duracionAnimacion
            );

            float suavizado = Mathf.SmoothStep(
                0f, 1f, progreso
            );

            barraBienestar.value = Mathf.Lerp(
                valorInicial, valorFinal, suavizado
            );

            yield return null;
        }

        barraBienestar.value = valorFinal;
        animacionBarra = null;
    }

    private IEnumerator MostrarPuntos(float puntos)
    {
        textoRetroalimentacion.gameObject.SetActive(true);

        textoRetroalimentacion.text =
            "+" + puntos.ToString("0") + " Bienestar";

        RectTransform rectTransform =
            textoRetroalimentacion.rectTransform;

        Vector2 posicionInicial = rectTransform.anchoredPosition;
        Vector2 posicionFinal = posicionInicial
            + Vector2.up * desplazamientoTexto;

        textoRetroalimentacion.alpha = 1f;

        float tiempo = 0f;

        while (tiempo < duracionTexto)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / duracionTexto
            );

            rectTransform.anchoredPosition = Vector2.Lerp(
                posicionInicial,
                posicionFinal,
                progreso
            );

            textoRetroalimentacion.alpha = 1f - progreso;

            yield return null;
        }

        rectTransform.anchoredPosition = posicionInicial;
        textoRetroalimentacion.alpha = 1f;
        textoRetroalimentacion.gameObject.SetActive(false);

        animacionTexto = null;
    }

    public float ObtenerBienestar()
    {
        return bienestarActual;
    }

    public bool PuedeContinuar()
    {
        return decisionTomada;
    }

}
