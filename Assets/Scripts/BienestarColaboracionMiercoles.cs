
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

    [SerializeField, Range(0f, 50f)]
    private float perdidaSanidad = 2f;

    [SerializeField] private float duracionAnimacion = 1.5f;

    [Header("Texto de retroalimentacion")]
    [SerializeField] private TMP_Text textoRetroalimentacion;

    [SerializeField] private float duracionTexto = 2f;
    [SerializeField] private float desplazamientoTexto = 40f;

    [Header("Control del boton Siguiente")]
    [SerializeField] private Button botonSiguiente;

    [SerializeField] private bool decisionTomada = false;

    [Header("Gestor de paneles del miercoles")]
    [SerializeField] private GestorPanelesMiercoles gestorPaneles;

    private float bienestarActual;

    private Coroutine animacionBarra;
    private Coroutine animacionTexto;

    private bool ayudaRegistrada;
    private bool decisionNoPreguntarRegistrada;


    // Claves de guardado
    private const string CLAVE_BIENESTAR = "Miercoles_Bienestar";
    private const string CLAVE_AYUDA = "Miercoles_AyudaRegistrada";
    private const string CLAVE_NO_PREGUNTAR = "Miercoles_NoPreguntarRegistrada";
    private const string CLAVE_DECISION_TOMADA = "Miercoles_DecisionTomada";
    private const string CLAVE_ESTADO_GUARDADO = "Miercoles_EstadoGuardado";

    private void Start()
    {
        if (PlayerPrefs.GetInt(CLAVE_ESTADO_GUARDADO, 0) == 1)
        {
            CargarProgreso();
        }
        else
        {
            InicializarNuevoProgreso();
        }
    }

    // ==========================================
    // INICIALIZAR PARTIDA NUEVA
    // ==========================================

    public void InicializarNuevoProgreso()
    {
        bienestarActual = Mathf.Clamp(
            bienestarInicial, 0f, 100f
        );

        ayudaRegistrada = false;
        decisionNoPreguntarRegistrada = false;
        decisionTomada = false;

        ActualizarSlider();
        ActualizarBotonSiguiente();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);

        GuardarProgreso();
    }

    // ==========================================
    // PREPARAR DECISION
    // ==========================================

    public void PrepararDecision()
    {
        // No reiniciar una decision ya registrada.
        if (ayudaRegistrada || decisionNoPreguntarRegistrada)
        {
            decisionTomada = true;
            ActualizarBotonSiguiente();
            return;
        }

        decisionTomada = false;

        ActualizarBotonSiguiente();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);
    }

    // ==========================================
    // OPCION 1: PREGUNTAR A LOS COMPAÑEROS
    // ==========================================

    public void RegistrarAyudaCompaneros()
    {
        if (ayudaRegistrada || decisionNoPreguntarRegistrada)
            return;

        ayudaRegistrada = true;
        decisionTomada = true;

        AplicarCambioBienestar(
            aumentoBienestar,
            true,
            "Bienestar"
        );
    }

    // ==========================================
    // OPCION 2: NO PREGUNTAR A LOS COMPAÑEROS
    // ==========================================

    public void RegistrarNoPreguntar()
    {
        if (ayudaRegistrada || decisionNoPreguntarRegistrada)
            return;

        decisionNoPreguntarRegistrada = true;
        decisionTomada = true;

        AplicarCambioBienestar(
            -perdidaSanidad,
            false,
            "Sanidad"
        );
    }

    // ==========================================
    // APLICAR CAMBIO DE BIENESTAR
    // ==========================================

    private void AplicarCambioBienestar(
        float cambio,
        bool esAumento,
        string nombreIndicador
    )
    {
        float bienestarAnterior = bienestarActual;

        bienestarActual = Mathf.Clamp(
            bienestarActual + cambio,
            0f,
            100f
        );

        float diferencia = Mathf.Abs(
            bienestarActual - bienestarAnterior
        );

        if (barraBienestar != null)
        {
            if (animacionBarra != null)
                StopCoroutine(animacionBarra);

            animacionBarra = StartCoroutine(
                AnimarBarra(bienestarActual)
            );
        }

        if (textoRetroalimentacion != null && diferencia > 0f)
        {
            if (animacionTexto != null)
                StopCoroutine(animacionTexto);

            animacionTexto = StartCoroutine(
                MostrarPuntos(
                    diferencia,
                    esAumento,
                    nombreIndicador
                )
            );
        }

        ActualizarBotonSiguiente();

        // Guardar inmediatamente la consecuencia y la decision.
        GuardarProgreso();
    }

    // ==========================================
    // ANIMACION DE LA BARRA
    // ==========================================

    private IEnumerator AnimarBarra(float valorFinal)
    {
        if (barraBienestar == null)
            yield break;

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
                valorInicial,
                valorFinal,
                suavizado
            );

            yield return null;
        }

        barraBienestar.value = valorFinal;
        animacionBarra = null;
    }

    // ==========================================
    // MOSTRAR PUNTOS
    // ==========================================

    private IEnumerator MostrarPuntos(
        float puntos,
        bool esAumento,
        string nombreIndicador
    )
    {
        textoRetroalimentacion.gameObject.SetActive(true);

        string signo = esAumento ? "+" : "-";

        textoRetroalimentacion.text =
            signo + puntos.ToString("0") + " " + nombreIndicador;

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

    // ==========================================
    // BOTON SIGUIENTE
    // ==========================================

    public void Siguiente()
    {
        if (!PuedeContinuar())
            return;

        if (gestorPaneles == null)
        {
            Debug.LogError(
                "No has asignado el GestorPanelesMiercoles."
            );
            return;
        }

        if (ayudaRegistrada)
        {
            gestorPaneles.IrASegundoPanelPreguntales();
        }
        else if (decisionNoPreguntarRegistrada)
        {
            gestorPaneles.IrASegundoPanelNoPreguntarles();
        }
    }

    // ==========================================
    // BLOQUEAR Y DESBLOQUEAR SIGUIENTE
    // ==========================================

    private void ActualizarBotonSiguiente()
    {
        if (botonSiguiente != null)
            botonSiguiente.interactable = decisionTomada;
    }

    // ==========================================
    // GUARDAR PROGRESO
    // ==========================================

    public void GuardarProgreso()
    {
        PlayerPrefs.SetFloat(
            CLAVE_BIENESTAR,
            bienestarActual
        );

        PlayerPrefs.SetInt(
            CLAVE_AYUDA,
            ayudaRegistrada ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CLAVE_NO_PREGUNTAR,
            decisionNoPreguntarRegistrada ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CLAVE_DECISION_TOMADA,
            decisionTomada ? 1 : 0
        );

        PlayerPrefs.SetInt(
            CLAVE_ESTADO_GUARDADO,
            1
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Miercoles guardado. Bienestar: "
            + bienestarActual
            + " | Pregunto: "
            + ayudaRegistrada
            + " | No pregunto: "
            + decisionNoPreguntarRegistrada
        );
    }

    // ==========================================
    // CARGAR PROGRESO
    // ==========================================

    public void CargarProgreso()
    {
        bienestarActual = PlayerPrefs.GetFloat(
            CLAVE_BIENESTAR,
            bienestarInicial
        );

        bienestarActual = Mathf.Clamp(
            bienestarActual,
            0f,
            100f
        );

        ayudaRegistrada = PlayerPrefs.GetInt(
            CLAVE_AYUDA,
            0
        ) == 1;

        decisionNoPreguntarRegistrada = PlayerPrefs.GetInt(
            CLAVE_NO_PREGUNTAR,
            0
        ) == 1;

        decisionTomada = PlayerPrefs.GetInt(
            CLAVE_DECISION_TOMADA,
            0
        ) == 1;

        ActualizarSlider();
        ActualizarBotonSiguiente();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);

        Debug.Log(
            "Miercoles recuperado. Bienestar: "
            + bienestarActual
            + " | Pregunto: "
            + ayudaRegistrada
            + " | No pregunto: "
            + decisionNoPreguntarRegistrada
        );
    }

    // ==========================================
    // ACTUALIZAR SLIDER
    // ==========================================

    private void ActualizarSlider()
    {
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
    }

    // ==========================================
    // CONSULTAR ESTADO
    // ==========================================

    public float ObtenerBienestar()
    {
        return bienestarActual;
    }

    public bool PuedeContinuar()
    {
        return decisionTomada;
    }

    public bool PreguntoAcompaneros()
    {
        return ayudaRegistrada;
    }

    public bool DecidioNoPreguntar()
    {
        return decisionNoPreguntarRegistrada;
    }
}
