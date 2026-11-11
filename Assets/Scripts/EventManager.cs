using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// ==========================================
// NOTA TÉCNICA: Este script utiliza TextMeshPro (TextMeshProUGUI) 
// para la manipulación de textos en la interfaz de usuario.
// ==========================================

[System.Serializable]
public class OpcionEvento
{
    [Tooltip("Texto que se muestra en el botón de decisión.")]
    [TextArea(2, 3)] public string textoDecision;

    [Tooltip("Texto de consecuencia que aparece tras elegir esta opción.")]
    [TextArea(3, 5)] public string textoConsecuencia;

    [Tooltip("Impacto numérico en la estabilidad mental (puede ser positivo o negativo).")]
    public float impactoEstabilidad;

    [Tooltip("Impacto numérico en los ingresos (puede ser positivo o negativo).")]
    public float impactoIngresos;
}

[System.Serializable]
public class EventoJuego
{
    [Tooltip("Texto de planteamiento principal del evento.")]
    [TextArea(3, 6)] public string descripcionEvento;

    [Tooltip("Opciones disponibles para este evento (Mínimo 2, Máximo 3).")]
    public List<OpcionEvento> opciones = new List<OpcionEvento>();
}

public class EventManager : MonoBehaviour
{
    [Header("Configuración de Estadísticas Iniciales")]
    [Range(0f, 100f)] public float estabilidadMentalInicial = 80f;
    public float ingresosIniciales = 50000f;

    [Header("Base de Datos de Eventos")]
    [Tooltip("Lista configurable de eventos aleatorios.")]
    public List<EventoJuego> listaEventos = new List<EventoJuego>();

    [Header("Referencias de UI - Panel Principal / Evento")]
    public GameObject panelEvento;
    public TMP_Text textoDescripcionEvento;
    [Tooltip("Arreglo de exactamente 3 botones para las opciones.")]
    public Button[] botonesOpciones = new Button[3];
    private TMP_Text[] textosBotonesOpciones = new TMP_Text[3];

    [Header("Referencias de UI - Panel de Consecuencia")]
    public GameObject panelConsecuencia;
    public TMP_Text textoConsecuencia;
    public Button botonContinuar;

    [Header("Referencias de UI - Estadísticas Globales (Opcional)")]
    public TMP_Text textoEstabilidadUI;
    public TMP_Text textoIngresosUI;

    // Estado interno del jugador
    private float estabilidadMental;
    private float ingresos;

    // Control de eventos
    private int indiceEventoAnterior = -1;
    private EventoJuego eventoActual;

    // Evento C# para notificar cambios de estado sin consultar cada frame
    public event System.Action<float, float> OnStatsChanged;

    void Awake()
    {
        // Inicializamos referencias de texto de los botones para optimizar
        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (botonesOpciones[i] != null)
            {
                textosBotonesOpciones[i] = botonesOpciones[i].GetComponentInChildren<TMP_Text>();

                // Capturamos el índice local para evitar problemas de cierre (closure) en el bucle
                int indexLocal = i;
                botonesOpciones[i].onClick.AddListener(() => OnOpcionSeleccionada(indexLocal));
            }
        }

        if (botonContinuar != null)
        {
            botonContinuar.onClick.AddListener(ContinuarSiguienteEvento);
        }

        // Suscribimos la actualización de UI al evento de cambios
        OnStatsChanged += ActualizarTextosUI;
    }

    void Start()
    {
        // Inicializamos estadísticas del jugador
        estabilidadMental = Mathf.Clamp(estabilidadMentalInicial, 0f, 100f);
        ingresos = ingresosIniciales;

        // Disparamos la actualización inicial de la UI
        OnStatsChanged?.Invoke(estabilidadMental, ingresos);

        // Iniciamos el primer evento si hay datos
        if (listaEventos != null && listaEventos.Count > 0)
        {
            CargarSiguienteEventoAleatorio();
        }
        else
        {
            Debug.LogWarning("EventManager: La lista de eventos está vacía.");
        }
    }

    // ==========================================
    // LÓGICA DE SELECCIÓN DE EVENTOS
    // ==========================================
    public void CargarSiguienteEventoAleatorio()
    {
        if (listaEventos == null || listaEventos.Count == 0) return;

        // Ocultamos panel de consecuencia y aseguramos mostrar el panel de opciones
        if (panelConsecuencia != null) panelConsecuencia.SetActive(false);
        if (panelEvento != null) panelEvento.SetActive(true);

        int indiceElegido = 0;

        // Si hay más de un evento, evitamos que se repita de forma consecutiva
        if (listaEventos.Count > 1)
        {
            do
            {
                indiceElegido = Random.Range(0, listaEventos.Count);
            }
            while (indiceElegido == indiceEventoAnterior);
        }

        indiceEventoAnterior = indiceElegido;
        eventoActual = listaEventos[indiceElegido];

        MostrarEventoEnUI(eventoActual);
    }

    // ==========================================
    // CONTROL DE LA INTERFAZ (UI)
    // ==========================================
    private void MostrarEventoEnUI(EventoJuego evento)
    {
        // Mostrar descripción
        if (textoDescripcionEvento != null)
        {
            textoDescripcionEvento.text = evento.descripcionEvento;
        }

        // Validar cantidad de opciones (mínimo 2, máximo 3)
        int cantidadOpciones = Mathf.Clamp(evento.opciones.Count, 2, 3);

        for (int i = 0; i < botonesOpciones.Length; i++)
        {
            if (botonesOpciones[i] == null) continue;

            if (i < cantidadOpciones)
            {
                botonesOpciones[i].gameObject.SetActive(true);
                if (textosBotonesOpciones[i] != null)
                {
                    textosBotonesOpciones[i].text = evento.opciones[i].textoDecision;
                }
            }
            else
            {
                // Ocultamos los botones sobrantes si el evento tiene menos de 3 opciones
                botonesOpciones[i].gameObject.SetActive(false);
            }
        }
    }

    // ==========================================
    // PROCESAMIENTO DE DECISIONES Y CONSECUENCIAS
    // ==========================================
    private void OnOpcionSeleccionada(int indiceOpcion)
    {
        if (eventoActual == null || indiceOpcion >= eventoActual.opciones.Count) return;

        OpcionEvento opcionElegida = eventoActual.opciones[indiceOpcion];

        // 1. Aplicar impacto al estado del jugador
        estabilidadMental = Mathf.Clamp(estabilidadMental + opcionElegida.impactoEstabilidad, 0f, 100f);
        ingresos += opcionElegida.impactoIngresos;

        // Disparamos el evento de cambio de estadísticas
        OnStatsChanged?.Invoke(estabilidadMental, ingresos);

        // 2. Mostrar la consecuencia y ocultar los botones de decisión
        if (panelEvento != null) panelEvento.SetActive(false);
        if (panelConsecuencia != null) panelConsecuencia.SetActive(true);

        if (textoConsecuencia != null)
        {
            textoConsecuencia.text = opcionElegida.textoConsecuencia;
        }
    }

    private void ContinuarSiguienteEvento()
    {
        CargarSiguienteEventoAleatorio();
    }

    // ==========================================
    // ACTUALIZACIÓN VISUAL DE ESTADÍSTICAS
    // ==========================================
    private void ActualizarTextosUI(float estabilidad, float dinero)
    {
        if (textoEstabilidadUI != null)
        {
            textoEstabilidadUI.text = $"Estabilidad: {estabilidad:F0}%";
        }

        if (textoIngresosUI != null)
        {
            textoIngresosUI.text = $"Saldo: ${dinero:N0}";
        }
    }

    // Desuscribir para evitar fugas de memoria
    void OnDestroy()
    {
        OnStatsChanged -= ActualizarTextosUI;
    }
}