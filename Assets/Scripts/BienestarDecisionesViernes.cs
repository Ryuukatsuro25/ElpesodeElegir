
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BienestarDecisionesViernes : MonoBehaviour
{
    public enum TipoDecision
    {
        Ninguna,
        PedirCopia,
        EntregarIncompleta
    }

    [Header("Indicador de bienestar")]
    [SerializeField] private Slider barraBienestar;

    [SerializeField, Range(0f, 100f)]
    private float bienestarInicial = 50f;

    [Header("Consecuencias de las decisiones")]
    [SerializeField] private float perdidaPedirCopia = 15f;
    [SerializeField] private float perdidaEntregarIncompleta = 3f;

    [SerializeField] private float duracionAnimacion = 1.5f;

    [Header("Texto de retroalimentacion")]
    [SerializeField] private TMP_Text textoRetroalimentacion;

    [SerializeField] private float duracionTexto = 2f;
    [SerializeField] private float desplazamientoTexto = 40f;

    [Header("Control del boton Siguiente")]
    [SerializeField] private Button botonSiguiente;

    [Header("Gestor de paneles del viernes")]
    [SerializeField] private GestorPanelesViernes gestorPaneles;

    private float bienestarActual;

    private TipoDecision decisionActual = TipoDecision.Ninguna;

    private Coroutine animacionBarra;
    private Coroutine animacionTexto;

    private bool decisionRegistrada = false;

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

        if (textoRetroalimentacion != null)
        {
            textoRetroalimentacion.gameObject.SetActive(false);
        }

        BloquearSiguiente();
    }

    // PREPARAR PANEL DE DECISION

    public void PrepararDecision()
    {
        decisionRegistrada = false;
        decisionActual = TipoDecision.Ninguna;

        BloquearSiguiente();

        if (textoRetroalimentacion != null)
        {
            textoRetroalimentacion.gameObject.SetActive(false);
        }
    }

    public void RegistrarPedirCopia()
    {
        if (decisionRegistrada) return;

        decisionRegistrada = true;
        decisionActual = TipoDecision.PedirCopia;

        // Aplicar perdida de bienestar
        AplicarCambioBienestar(perdidaPedirCopia);

        StartCoroutine(AplicarDecisionYContinuar(
        perdidaPedirCopia,
        true
        ));
    }

    public void RegistrarEntregarIncompleta()
    {
        if (decisionRegistrada) return;

        decisionRegistrada = true;
        decisionActual = TipoDecision.EntregarIncompleta;

        // Aplicar perdida de bienestar
        AplicarCambioBienestar(perdidaEntregarIncompleta);

        StartCoroutine(AplicarDecisionYContinuar(
        perdidaEntregarIncompleta,
        false
        ));
    }

    // APLICAR CAMBIO DE BIENESTAR

    private void AplicarCambioBienestar(float perdida)
    {
        float bienestarAnterior = bienestarActual;

        bienestarActual = Mathf.Clamp(
            bienestarActual - perdida,
            0f,
            100f
        );

        float puntosPerdidos =
            bienestarAnterior - bienestarActual;

        if (barraBienestar != null)
        {
            if (animacionBarra != null)
                StopCoroutine(animacionBarra);

            animacionBarra = StartCoroutine(
                AnimarBarra(bienestarActual)
            );
        }

        if (textoRetroalimentacion != null && puntosPerdidos > 0f)
        {
            if (animacionTexto != null)
                StopCoroutine(animacionTexto);

            animacionTexto = StartCoroutine(
                MostrarPuntos(puntosPerdidos)
            );
        }

        DesbloquearSiguiente();
    }

    // ANIMACION DESCENDENTE

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

    // MOSTRAR PUNTOS PERDIDOS

    private IEnumerator MostrarPuntos(float puntos)
    {
        textoRetroalimentacion.gameObject.SetActive(true);

        textoRetroalimentacion.text =
            "-" + puntos.ToString("0") + " Bienestar";

        RectTransform rectTransform =
            textoRetroalimentacion.rectTransform;

        Vector2 posicionInicial =
            rectTransform.anchoredPosition;

        Vector2 posicionFinal =
            posicionInicial +
            Vector2.up * desplazamientoTexto;

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

    private IEnumerator AplicarDecisionYContinuar(
         float perdida,
         bool pedirCopia)
    {
    // Aplicar el cambio de bienestar
    AplicarCambioBienestar(perdida);

    // Esperar a que termine la animación
    yield return new WaitForSeconds(
        duracionAnimacion
    );

    // Esperar también a que termine el texto flotante
    if (textoRetroalimentacion != null)
    {
        yield return new WaitForSeconds(duracionTexto);
    }

    // Cambiar al panel correspondiente
    if (gestorPaneles != null)
    {
        if (pedirCopia)
        {
            gestorPaneles.IrADecisionPedirCopia();
        }
        else
        {
            gestorPaneles.IrADecisionNoHacerTarea();
        }
    }
}

    // BLOQUEAR Y DESBLOQUEAR SIGUIENTE

    private void BloquearSiguiente()
    {
        if (botonSiguiente != null)
        {
            botonSiguiente.interactable = false;
        }
    }

    private void DesbloquearSiguiente()
    {
        if (botonSiguiente != null)
        {
            botonSiguiente.interactable = true;
        }
    }

    // CONTINUAR SEGUN LA DECISION

    public void Continuar()
    {
        if (!decisionRegistrada || gestorPaneles == null)
            return;

        if (decisionActual == TipoDecision.PedirCopia)
        {
            gestorPaneles.IrASegundoPanelPedirCopia();
        }
        else if (decisionActual == TipoDecision.EntregarIncompleta)
        {
            gestorPaneles.IrASegundoPanelNoHacerTarea();
        }
    }

    public float ObtenerBienestar()
    {
        return bienestarActual;
    }
}
