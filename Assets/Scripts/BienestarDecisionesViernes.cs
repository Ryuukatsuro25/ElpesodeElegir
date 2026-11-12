
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField, Range(0f, 100f)] private float bienestarInicial = 50f;

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
    private Coroutine corrutinaDecision;

    private bool decisionRegistrada = false;
    private bool estadoInicializado = false;

    private const string CLAVE_BIENESTAR = "Viernes_Bienestar";
    private const string CLAVE_DECISION = "Viernes_Decision";
    private const string CLAVE_DECISION_REGISTRADA = "Viernes_DecisionRegistrada";

    private void Start()
    {
        // Garantizar que exista el indicador semanal.
        SistemaBienestarSemanal.InicializarSiNoExiste();

        // Recuperar la decision si ya habia sido registrada.
        if (PlayerPrefs.GetInt(CLAVE_DECISION_REGISTRADA, 0) == 1)
        {
            CargarProgreso();
        }
        else
        {
            InicializarNuevoProgreso();
        }
    }

    // ==========================================
    // INICIALIZAR PARTIDA
    // ==========================================

    public void InicializarNuevoProgreso()
    {
        // Utilizar el bienestar acumulado de toda la semana.
        bienestarActual = SistemaBienestarSemanal.ObtenerBienestar();

        decisionActual = TipoDecision.Ninguna;
        decisionRegistrada = false;

        estadoInicializado = true;

        ActualizarSlider();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);

        BloquearSiguiente();

        GuardarProgreso();
    }

    // ==========================================
    // PREPARAR PANEL DE DECISION
    // ==========================================

    public void PrepararDecision()
    {
        if (decisionRegistrada)
        {
            DesbloquearSiguiente();
            return;
        }

        decisionActual = TipoDecision.Ninguna;

        BloquearSiguiente();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);
    }

    // ==========================================
    // REGISTRAR PEDIR COPIA
    // ==========================================

    public void RegistrarPedirCopia()
    {
        if (decisionRegistrada)
            return;

        decisionRegistrada = true;
        decisionActual = TipoDecision.PedirCopia;

        if (corrutinaDecision != null)
            StopCoroutine(corrutinaDecision);

        corrutinaDecision = StartCoroutine(
            AplicarDecisionYContinuar(perdidaPedirCopia, true)
        );
    }

    // ==========================================
    // REGISTRAR ENTREGAR INCOMPLETA
    // ==========================================

    public void RegistrarEntregarIncompleta()
    {
        if (decisionRegistrada)
            return;

        decisionRegistrada = true;
        decisionActual = TipoDecision.EntregarIncompleta;

        if (corrutinaDecision != null)
            StopCoroutine(corrutinaDecision);

        corrutinaDecision = StartCoroutine(
            AplicarDecisionYContinuar(perdidaEntregarIncompleta, false)
        );
    }

    // ==========================================
    // APLICAR CAMBIO DE BIENESTAR SEMANAL
    // ==========================================

    private void AplicarCambioBienestar(float perdida)
    {
        float bienestarAnterior =
            SistemaBienestarSemanal.ObtenerBienestar();

        // Aplicar la perdida al indicador compartido.
        bienestarActual = SistemaBienestarSemanal.AplicarCambio(-perdida);

        float puntosPerdidos = bienestarAnterior - bienestarActual;

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

    // ==========================================
    // ANIMACION DESCENDENTE
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

            float suavizado = Mathf.SmoothStep(0f, 1f, progreso);

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
    // MOSTRAR PUNTOS PERDIDOS
    // ==========================================

    private IEnumerator MostrarPuntos(float puntos)
    {
        textoRetroalimentacion.gameObject.SetActive(true);

        textoRetroalimentacion.text =
            "-" + puntos.ToString("0") + " Bienestar";

        RectTransform rectTransform =
            textoRetroalimentacion.rectTransform;

        Vector2 posicionInicial = rectTransform.anchoredPosition;

        Vector2 posicionFinal =
            posicionInicial + Vector2.up * desplazamientoTexto;

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
    // APLICAR DECISION Y CONTINUAR
    // ==========================================

    private IEnumerator AplicarDecisionYContinuar(
        float perdida,
        bool pedirCopia
    )
    {
        // Aplicar la consecuencia una sola vez.
        AplicarCambioBienestar(perdida);

        // Guardar inmediatamente la decision.
        GuardarProgreso();

        // Comprobar si el bienestar llego a cero.
        if (SistemaBienestarSemanal.BienestarEnCero() &&
            !SistemaBienestarSemanal.DesmayoYaMostrado())
        {
            SistemaBienestarSemanal.RegistrarDesmayo();

            Debug.Log(
                "Bienestar semanal en cero. Fin de la partida."
            );

            SceneManager.LoadScene("FinDeJuego");

            yield break;
        }

        // Esperar la animacion de la barra.
        yield return new WaitForSeconds(duracionAnimacion);

        // Esperar el texto flotante.
        if (textoRetroalimentacion != null)
            yield return new WaitForSeconds(duracionTexto);

        // Cambiar al panel correspondiente.
        if (gestorPaneles != null)
        {
            if (pedirCopia)
                gestorPaneles.IrADecisionPedirCopia();
            else
                gestorPaneles.IrADecisionNoHacerTarea();
        }

        corrutinaDecision = null;
    }

    // ==========================================
    // BLOQUEAR Y DESBLOQUEAR SIGUIENTE
    // ==========================================

    private void BloquearSiguiente()
    {
        if (botonSiguiente != null)
            botonSiguiente.interactable = false;
    }

    private void DesbloquearSiguiente()
    {
        if (botonSiguiente != null)
            botonSiguiente.interactable = true;
    }

    // ==========================================
    // CONTINUAR SEGUN LA DECISION
    // ==========================================

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

    // ==========================================
    // GUARDAR PROGRESO
    // ==========================================

    public void GuardarProgreso()
    {
        // El indicador semanal es la fuente principal.
        bienestarActual = SistemaBienestarSemanal.ObtenerBienestar();

        PlayerPrefs.SetFloat(
            CLAVE_BIENESTAR,
            bienestarActual
        );

        PlayerPrefs.SetInt(
            CLAVE_DECISION,
            (int)decisionActual
        );

        PlayerPrefs.SetInt(
            CLAVE_DECISION_REGISTRADA,
            decisionRegistrada ? 1 : 0
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Viernes: progreso guardado. Bienestar semanal: "
            + bienestarActual
            + " | Decision: "
            + decisionActual
        );
    }

    // ==========================================
    // CARGAR PROGRESO
    // ==========================================

    public void CargarProgreso()
    {
        // Recuperar el bienestar compartido.
        bienestarActual = SistemaBienestarSemanal.ObtenerBienestar();

        int decisionGuardada = PlayerPrefs.GetInt(
            CLAVE_DECISION,
            0
        );

        if (System.Enum.IsDefined(
            typeof(TipoDecision),
            decisionGuardada
        ))
        {
            decisionActual = (TipoDecision)decisionGuardada;
        }
        else
        {
            decisionActual = TipoDecision.Ninguna;
        }

        decisionRegistrada = PlayerPrefs.GetInt(
            CLAVE_DECISION_REGISTRADA,
            0
        ) == 1;

        estadoInicializado = true;

        ActualizarSlider();

        if (textoRetroalimentacion != null)
            textoRetroalimentacion.gameObject.SetActive(false);

        if (decisionRegistrada)
            DesbloquearSiguiente();
        else
            BloquearSiguiente();

        Debug.Log(
            "Viernes: progreso recuperado. Bienestar semanal: "
            + bienestarActual
            + " | Decision: "
            + decisionActual
        );
    }

    // ==========================================
    // ACTUALIZAR SLIDER SIN ANIMACION
    // ==========================================

    private void ActualizarSlider()
    {
        if (barraBienestar != null)
        {
            barraBienestar.minValue = 0f;
            barraBienestar.maxValue = 100f;
            barraBienestar.value = bienestarActual;
        }
    }

    // ==========================================
    // OBTENER BIENESTAR Y DECISION
    // ==========================================

    public float ObtenerBienestar()
    {
        return bienestarActual;
    }

    public TipoDecision ObtenerDecision()
    {
        return decisionActual;
    }

    public bool TieneDecisionRegistrada()
    {
        return decisionRegistrada;
    }
}
