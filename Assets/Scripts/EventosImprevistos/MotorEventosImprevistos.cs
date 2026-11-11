using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PesoDeElegir.EventosImprevistos
{
    /// <summary>
    /// MOTOR DE EVENTOS IMPREVISTOS (épica EI4 + GM1.1.4)
    ///
    /// Un solo controlador que reutiliza TRES paneles (Narrativa, Decisión, Consecuencia)
    /// para procesar los eventos definidos en un JSON ("Computador", "Cumpleaños", "Vecina").
    ///
    ///  - Lee los eventos de un TextAsset (por defecto Resources/EventosImprevistos.json).
    ///  - Los ejecuta en orden aleatorio, uno tras otro, en la misma escena.
    ///  - Asigna el texto de cada opción a su botón; al elegir, ese botón muestra la opción elegida.
    ///  - Aplica dinero y bienestar y los guarda al instante en datosBancarios.json y datosBienestar.json
    ///    (mismo formato que GestorBancario/AppBanco y PopUpManager).
    ///  - El bienestar queda siempre entre 0 y 100.
    ///  - Dinero en pantalla: "-$30.000" / "+$15.000".
    ///  - Cobro diferido de "Pedir prestado" con una bandera en PlayerPrefs.
    ///
    /// Va en: un GameObject siempre activo, por ejemplo "--- Sistemas ---/MotorEventos".
    /// </summary>
    public class MotorEventosImprevistos : MonoBehaviour
    {
        // =====================================================================
        //  ESTRUCTURA DEL JSON (los nombres de los campos deben coincidir)
        // =====================================================================

        [Serializable]
        private class ArchivoEventos
        {
            public CobroDiferidoDatos cobroDiferido;
            public EventoDatos[] eventos;
        }

        [Serializable]
        private class CobroDiferidoDatos
        {
            public string texto;
            public string descripcionMovimiento;
            public int monto;          // negativo: -40000
            public float bienestar;    // negativo: -50
        }

        [Serializable]
        private class EventoDatos
        {
            public string id;
            public string[] pantallasIntro;        // Se leen con "Siguiente" antes de decidir
            public string textoDecision;           // Visible junto a las opciones
            public float segundosAntesDeOpciones;  // > 0: las opciones se habilitan después de leer
            public OpcionDatos[] opciones;
        }

        [Serializable]
        private class OpcionDatos
        {
            public string etiqueta;               // Texto del botón
            public string impacto;                // Costo/consecuencia visible ANTES de elegir ("" = no mostrar)
            public int monto;                     // + ingreso, - gasto, 0 = no toca el saldo
            public string descripcionMovimiento;  // Texto en el historial del banco
            public float bienestar;               // Cambio exacto de bienestar
            public string medalla;                // Id de la medalla ("" = ninguna)
            public bool activaCobroDiferido;      // true en "Pedir prestado"
            public string[] consecuencias;        // Pantallas después de elegir; la última va con los cambios
            public string mensajePersonaje;       // GM1.1.4: solo se muestra si monto != 0
        }

        // Formato idéntico al de GestorBancario / AppBanco
        [Serializable]
        private class Movimiento
        {
            public string descripcion;
            public int monto;
            public bool esIngreso;
        }

        [Serializable]
        private class Cuenta
        {
            public int saldoActual;
            public int ingresosTotales;
            public int gastosTotales;
            public List<Movimiento> historial = new();
        }

        // Formato idéntico al de PopUpManager
        [Serializable]
        private class DatosDeBienestar
        {
            public float bienestarActual;
        }

        // Un botón de opción con sus dos textos
        [Serializable]
        public class BotonOpcion
        {
            public Button boton;
            public TextMeshProUGUI etiqueta;
            [Tooltip("Texto pequeño con el costo o la consecuencia (opcional)")]
            public TextMeshProUGUI impacto;
        }

        // =====================================================================
        //  INSPECTOR
        // =====================================================================

        [Header("Datos")]
        [Tooltip("Vacío = se carga Resources/EventosImprevistos.json")]
        [SerializeField] private TextAsset archivoEventos;
        [SerializeField] private bool ordenAleatorio = true;

        [Header("Partida existente (datosBancarios.json)")]
        [Tooltip("Apagado: se usa SOLO la partida que ya creó el banco. Encendido: si no existe, crea una de prueba.")]
        [SerializeField] private bool crearPartidaSiNoExiste = false;
        [SerializeField] private int mesadaDePrueba = 100000;

        [Header("Barras de arriba (aquí se ve el cambio de dinero y bienestar)")]
        [Tooltip("Barras/Barra ingresos/Barra llena  (Image Type = Filled, Horizontal)")]
        [SerializeField] private Image barraIngresos;
        [Tooltip("Barras/Barra Mental/Barra llena  (Image Type = Filled, Horizontal)")]
        [SerializeField] private Image barraBienestar;
        [SerializeField, Min(0.1f)] private float velocidadBarra = 1.5f;

        [Header("Textos flotantes junto a las barras (opcionales)")]
        [Tooltip("Muestra '-$30.000' / '+$15.000' junto a la barra de ingresos unos segundos")]
        [SerializeField] private TextMeshProUGUI textoDeltaIngresos;
        [Tooltip("Muestra '+50 SANIDAD' / '-20 SANIDAD' junto a la barra de bienestar unos segundos")]
        [SerializeField] private TextMeshProUGUI textoDeltaBienestar;
        [SerializeField, Min(0.5f)] private float segundosDelta = 2.5f;

        [Header("Textos de valor (opcionales)")]
        [Tooltip("Opcional. El saldo NO se toma de aquí: se lee de 'saldoActual' en datosBancarios.json.")]
        [SerializeField] private TextMeshProUGUI textoSaldo;
        [SerializeField] private TextMeshProUGUI textoBienestar;

        [Header("Panel Narrativa")]
        [SerializeField] private GameObject panelNarrativa;
        [SerializeField] private TextMeshProUGUI textoNarrativa;
        [SerializeField] private Button botonSiguiente;

        [Header("Panel Decisión")]
        [SerializeField] private GameObject panelDecision;
        [SerializeField] private TextMeshProUGUI textoDecision;
        [Tooltip("Uno por opción, en el mismo orden del JSON. Los que sobren se ocultan.")]
        [SerializeField] private BotonOpcion[] botonesOpcion = new BotonOpcion[3];
        [SerializeField] private string prefijoEleccion = "Elegiste: ";
        [SerializeField, Min(0f)] private float segundosMostrandoEleccion = 0.8f;

        [Header("Panel Consecuencia")]
        [SerializeField] private GameObject panelConsecuencia;
        [SerializeField] private TextMeshProUGUI textoConsecuencia;
        [SerializeField] private TextMeshProUGUI textoCambioDinero;
        [SerializeField] private TextMeshProUGUI textoCambioBienestar;
        [Tooltip("GM1.1.4: frase en primera persona")]
        [SerializeField] private TextMeshProUGUI textoMensajePersonaje;
        [SerializeField] private Button botonContinuar;

        [Header("Ajuste de textos (impide que se salgan de su cuadro)")]
        [Tooltip("Activa el ajuste automático: letra que se encoge, saltos de línea y recorte con '…'")]
        [SerializeField] private bool forzarTextosDentroDelCuadro = true;
        [Tooltip("Si el rectángulo del texto se sale de su objeto padre (el cuadro naranja), lo recorta para que quede dentro")]
        [SerializeField] private bool encerrarEnSuPadre = true;
        [Tooltip("Espacio libre entre el texto y el borde de su cuadro")]
        [SerializeField, Min(0f)] private float margenInterno = 20f;
        [SerializeField, Min(6f)] private float tamanoMinimo = 14f;
        [SerializeField, Min(6f)] private float tamanoMaximoTextos = 36f;
        [SerializeField, Min(6f)] private float tamanoMaximoBotones = 28f;
        [Tooltip("Pone Scale (1,1,1) en los paneles, cuadros, botones y textos SIN cambiar lo que se ve: " +
                 "pasa la escala al Width/Height. Arregla las letras estiradas.")]
        [SerializeField] private bool corregirEscalasAlIniciar = true;

        [Header("Fin y conexiones con otros sistemas")]
        [SerializeField] private GameObject panelFin;
        [Tooltip("Opcional: conecta aquí MedallasManager.DesbloquearMedalla para mostrar su notificación")]
        [SerializeField] private UnityEvent<string> alDesbloquearMedalla;
        [Tooltip("Lo que sigue al terminar los tres eventos")]
        [SerializeField] private UnityEvent alTerminarEventos;

        // =====================================================================
        //  ESTADO INTERNO
        // =====================================================================

        private const string ArchivoBanco = "datosBancarios.json";
        private const string ArchivoBienestar = "datosBienestar.json";
        private const string RecursoEventos = "EventosImprevistos";
        private const string ClaveCobroDiferido = "EI4_CobroDiferidoPendiente";
        private const float BienestarMin = 0f, BienestarMax = 100f, BienestarInicial = 50f;

        private static readonly NumberFormatInfo FormatoPesos = new()
        {
            NumberGroupSeparator = ".",
            NumberDecimalSeparator = ","
        };

        private ArchivoEventos datos;
        private readonly Queue<EventoDatos> colaEventos = new();
        private bool cobroActivadoEnEsteEvento;
        private float rellenoObjetivo;          // bienestar 0..1
        private float rellenoIngresosObjetivo;  // saldo / ingresos totales 0..1
        private Coroutine rutinaDecision;
        private Coroutine rutinaDeltaIngresos, rutinaDeltaBienestar;

        private string RutaBanco => Path.Combine(Application.persistentDataPath, ArchivoBanco);
        private string RutaBienestar => Path.Combine(Application.persistentDataPath, ArchivoBienestar);

        // =====================================================================
        //  CICLO DE UNITY
        // =====================================================================

        private void Start()
        {
            if (!ValidarJerarquia()) return;
            if (corregirEscalasAlIniciar) CorregirEscalas();
            if (forzarTextosDentroDelCuadro) ConfigurarTextos();
            OcultarPaneles();
            if (textoDeltaIngresos != null) textoDeltaIngresos.gameObject.SetActive(false);
            if (textoDeltaBienestar != null) textoDeltaBienestar.gameObject.SetActive(false);

            if (!CargarEventos()) return;

            VerificarPartida();
            RefrescarHUD(instantaneo: true);

            PrepararCola();
            SiguientePaso();
        }

        private void Update()
        {
            // Las dos barras de arriba se mueven suavemente hacia su valor nuevo (suben o bajan)
            if (barraBienestar != null)
                barraBienestar.fillAmount = Mathf.MoveTowards(
                    barraBienestar.fillAmount, rellenoObjetivo, velocidadBarra * Time.deltaTime);

            if (barraIngresos != null)
                barraIngresos.fillAmount = Mathf.MoveTowards(
                    barraIngresos.fillAmount, rellenoIngresosObjetivo, velocidadBarra * Time.deltaTime);
        }

        // =====================================================================
        //  LECTURA DEL JSON Y ORDEN ALEATORIO
        // =====================================================================

        private bool CargarEventos()
        {
            TextAsset fuente = archivoEventos != null ? archivoEventos : Resources.Load<TextAsset>(RecursoEventos);
            if (fuente == null)
            {
                Debug.LogError($"MotorEventos: no se encontró Resources/{RecursoEventos}.json ni hay archivo asignado.");
                return false;
            }

            try
            {
                datos = JsonUtility.FromJson<ArchivoEventos>(fuente.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"MotorEventos: el JSON tiene un error de formato. {e.Message}");
                return false;
            }

            if (datos?.eventos == null || datos.eventos.Length == 0)
            {
                Debug.LogError("MotorEventos: el JSON no contiene eventos.");
                return false;
            }

            foreach (EventoDatos evento in datos.eventos)
            {
                if (evento.opciones != null && evento.opciones.Length > botonesOpcion.Length)
                    Debug.LogWarning($"MotorEventos: '{evento.id}' tiene {evento.opciones.Length} opciones y solo hay {botonesOpcion.Length} botones.");
            }

            return true;
        }

        private void PrepararCola()
        {
            var lista = new List<EventoDatos>(datos.eventos);

            if (ordenAleatorio)
            {
                // Fisher–Yates: cada orden posible tiene la misma probabilidad
                for (int i = lista.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    (lista[i], lista[j]) = (lista[j], lista[i]);
                }
            }

            colaEventos.Clear();
            foreach (EventoDatos evento in lista) colaEventos.Enqueue(evento);
        }

        // =====================================================================
        //  FLUJO: evento → (cobro diferido) → evento → … → fin
        // =====================================================================

        private void SiguientePaso()
        {
            // El cobro se hace en una pantalla POSTERIOR: no justo después de pedir prestado,
            // sino antes del siguiente evento (o al final si ya no quedan eventos).
            bool hayCobro = PlayerPrefs.GetInt(ClaveCobroDiferido, 0) == 1;
            bool puedeCobrarse = !cobroActivadoEnEsteEvento || colaEventos.Count == 0;
            cobroActivadoEnEsteEvento = false;

            if (hayCobro && puedeCobrarse)
            {
                EjecutarCobroDiferido();
                return;
            }

            if (colaEventos.Count == 0)
            {
                OcultarPaneles();
                if (panelFin != null) panelFin.SetActive(true);
                alTerminarEventos?.Invoke();
                return;
            }

            EventoDatos evento = colaEventos.Dequeue();
            MostrarNarrativa(evento.pantallasIntro, () => MostrarDecision(evento));
        }

        private void EjecutarCobroDiferido()
        {
            CobroDiferidoDatos cobro = datos.cobroDiferido;
            PlayerPrefs.DeleteKey(ClaveCobroDiferido);
            PlayerPrefs.Save();

            if (cobro == null)
            {
                Debug.LogWarning("MotorEventos: hay un cobro pendiente pero el JSON no define 'cobroDiferido'.");
                SiguientePaso();
                return;
            }

            AplicarDinero(cobro.monto, cobro.descripcionMovimiento);
            AplicarBienestar(cobro.bienestar);

            // El cobro mueve el saldo, pero no es una decisión del personaje: sin mensaje GM1.1.4
            MostrarConsecuencia(cobro.texto, cobro.monto, cobro.bienestar, null, SiguientePaso);
        }

        // =====================================================================
        //  PANEL NARRATIVA
        // =====================================================================

        private void MostrarNarrativa(IReadOnlyList<string> pantallas, Action alTerminar)
        {
            if (pantallas == null || pantallas.Count == 0)
            {
                alTerminar?.Invoke();
                return;
            }

            int indice = 0;
            MostrarSolo(panelNarrativa);
            PonerTexto(textoNarrativa, pantallas[indice]);

            botonSiguiente.onClick.RemoveAllListeners();
            botonSiguiente.onClick.AddListener(() =>
            {
                indice++;
                if (indice < pantallas.Count)
                {
                    // Aún quedan textos de introducción: se cambia el texto en el mismo panel
                    PonerTexto(textoNarrativa, pantallas[indice]);
                    return;
                }

                // Último "Siguiente": se APAGA el panel de narrativa antes de pasar al siguiente
                botonSiguiente.onClick.RemoveAllListeners();
                panelNarrativa.SetActive(false);
                alTerminar?.Invoke();
            });
        }

        // =====================================================================
        //  PANEL DECISIÓN
        // =====================================================================

        private void MostrarDecision(EventoDatos evento)
        {
            // Narrativa apagada → se ENCIENDE el panel de decisión con el dilema y las opciones
            if (panelNarrativa != null) panelNarrativa.SetActive(false);
            MostrarSolo(panelDecision);
            PonerTexto(textoDecision, evento.textoDecision);

            OpcionDatos[] opciones = evento.opciones ?? Array.Empty<OpcionDatos>();

            for (int i = 0; i < botonesOpcion.Length; i++)
            {
                BotonOpcion ui = botonesOpcion[i];
                if (ui == null || ui.boton == null) continue;

                bool usada = i < opciones.Length;
                ui.boton.gameObject.SetActive(usada);
                ui.boton.onClick.RemoveAllListeners();
                ui.boton.interactable = false; // se habilitan al final de este método o tras la espera
                if (!usada) continue;

                OpcionDatos opcion = opciones[i];
                BotonOpcion botonElegido = ui;

                // El texto de cada opción va a SU botón
                if (ui.etiqueta != null) PonerTexto(ui.etiqueta, opcion.etiqueta);
                if (ui.impacto != null)
                {
                    bool hayImpacto = !string.IsNullOrEmpty(opcion.impacto);
                    ui.impacto.gameObject.SetActive(hayImpacto);
                    if (hayImpacto) PonerTexto(ui.impacto, opcion.impacto);
                }

                ui.boton.onClick.AddListener(() => ElegirOpcion(botonElegido, opcion));
            }

            if (rutinaDecision != null) StopCoroutine(rutinaDecision);
            rutinaDecision = StartCoroutine(HabilitarOpciones(evento.segundosAntesDeOpciones));
        }

        private IEnumerator HabilitarOpciones(float espera)
        {
            // Vecina: el texto completo se lee antes de poder elegir
            if (espera > 0f) yield return new WaitForSeconds(espera);
            PonerOpcionesInteractivas(true);
            rutinaDecision = null;
        }

        private void ElegirOpcion(BotonOpcion botonElegido, OpcionDatos opcion)
        {
            if (rutinaDecision != null) StopCoroutine(rutinaDecision);

            // Evita doble clic y deja ver solo la elección
            PonerOpcionesInteractivas(false);
            foreach (BotonOpcion ui in botonesOpcion)
            {
                if (ui == null || ui.boton == null) continue;
                ui.boton.onClick.RemoveAllListeners();
                if (ui != botonElegido) ui.boton.gameObject.SetActive(false);
            }

            // El botón pasa a mostrar la opción elegida
            if (botonElegido.etiqueta != null) PonerTexto(botonElegido.etiqueta, prefijoEleccion + opcion.etiqueta);
            if (botonElegido.impacto != null) botonElegido.impacto.gameObject.SetActive(false);

            rutinaDecision = StartCoroutine(AplicarTrasPausa(opcion));
        }

        private IEnumerator AplicarTrasPausa(OpcionDatos opcion)
        {
            if (segundosMostrandoEleccion > 0f) yield return new WaitForSeconds(segundosMostrandoEleccion);
            rutinaDecision = null;
            AplicarOpcion(opcion);
        }

        private void PonerOpcionesInteractivas(bool valor)
        {
            foreach (BotonOpcion ui in botonesOpcion)
            {
                if (ui != null && ui.boton != null) ui.boton.interactable = valor;
            }
        }

        // =====================================================================
        //  AFECTACIONES DE LA OPCIÓN
        // =====================================================================

        private void AplicarOpcion(OpcionDatos opcion)
        {
            // 1. Afectaciones inmediatas (se guardan y el HUD cambia en este instante)
            AplicarDinero(opcion.monto, opcion.descripcionMovimiento);
            AplicarBienestar(opcion.bienestar);

            if (!string.IsNullOrEmpty(opcion.medalla))
                DesbloquearMedalla(opcion.medalla);

            if (opcion.activaCobroDiferido)
            {
                PlayerPrefs.SetInt(ClaveCobroDiferido, 1);
                PlayerPrefs.Save();
                cobroActivadoEnEsteEvento = true;
            }

            // 2. Pantallas: todas menos la última como narrativa; la última, con los cambios
            string[] textos = opcion.consecuencias ?? Array.Empty<string>();
            var narrativa = new List<string>();
            for (int i = 0; i < textos.Length - 1; i++) narrativa.Add(textos[i]);
            string ultimo = textos.Length > 0 ? textos[textos.Length - 1] : string.Empty;

            // GM1.1.4: el mensaje en primera persona solo si la opción movió el saldo
            string mensaje = opcion.monto != 0 ? opcion.mensajePersonaje : null;

            MostrarNarrativa(narrativa, () =>
                MostrarConsecuencia(ultimo, opcion.monto, opcion.bienestar, mensaje, SiguientePaso));
        }

        // =====================================================================
        //  PANEL CONSECUENCIA
        // =====================================================================

        private void MostrarConsecuencia(string texto, int monto, float cambioBienestar, string mensajePersonaje, Action alContinuar)
        {
            MostrarSolo(panelConsecuencia);

            PonerTexto(textoConsecuencia, texto ?? string.Empty);
            MostrarLinea(textoCambioDinero, monto != 0, DineroConSigno(monto));
            MostrarLinea(textoCambioBienestar, !Mathf.Approximately(cambioBienestar, 0f), BienestarConSigno(cambioBienestar));
            MostrarLinea(textoMensajePersonaje, !string.IsNullOrEmpty(mensajePersonaje), mensajePersonaje);

            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(() =>
            {
                botonContinuar.onClick.RemoveAllListeners();
                alContinuar?.Invoke();
            });
        }

        // =====================================================================
        //  PERSISTENCIA: DINERO (datosBancarios.json)
        // =====================================================================

        private void AplicarDinero(int monto, string descripcion)
        {
            if (monto == 0) return;

            // Siempre se relee el archivo justo antes de escribir: así se modifica la partida
            // tal como la dejó el banco (mesada, gastos de otros días, etc.) y no una copia vieja.
            Cuenta cuenta = LeerCuenta();
            if (cuenta == null)
            {
                Debug.LogWarning($"MotorEventos: no existe {ArchivoBanco}; se crea uno nuevo sin mesada. " +
                                 "Inicia el juego desde el domingo para usar la partida real.");
                cuenta = new Cuenta();
            }
            cuenta.historial ??= new List<Movimiento>();

            bool esIngreso = monto > 0;
            int valor = Mathf.Abs(monto);

            cuenta.historial.Add(new Movimiento
            {
                descripcion = string.IsNullOrEmpty(descripcion) ? (esIngreso ? "Ingreso" : "Gasto") : descripcion,
                monto = valor,
                esIngreso = esIngreso
            });

            if (esIngreso)
            {
                cuenta.saldoActual += valor;
                cuenta.ingresosTotales += valor;
            }
            else
            {
                cuenta.saldoActual -= valor;
                cuenta.gastosTotales += valor;
            }

            File.WriteAllText(RutaBanco, JsonUtility.ToJson(cuenta, true));
            SincronizarScriptsDeLaEscena();
            RefrescarHUD();
            MostrarDelta(textoDeltaIngresos, DineroConSigno(monto), ref rutinaDeltaIngresos);
        }

        /// <summary>
        /// Si en la escena hay un GestorBancario, AppBanco o PopUpManager, les pide que vuelvan a
        /// leer su JSON. Así el celular muestra el movimiento nuevo y, sobre todo, ninguno de ellos
        /// vuelve a guardar su copia vieja encima de lo que escribió este motor.
        /// Se hace por nombre de clase para no depender de que esos scripts existan en la rama.
        /// </summary>
        private static void SincronizarScriptsDeLaEscena()
        {
            foreach (MonoBehaviour mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                string tipo = mb.GetType().Name;
                if (tipo == "GestorBancario" || tipo == "AppBanco" || tipo == "PopUpManager")
                    mb.SendMessage("CargarDatos", SendMessageOptions.DontRequireReceiver);
            }
        }

        private Cuenta LeerCuenta()
        {
            if (!File.Exists(RutaBanco)) return null;

            try
            {
                return JsonUtility.FromJson<Cuenta>(File.ReadAllText(RutaBanco));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"MotorEventos: no se pudo leer {ArchivoBanco}. {e.Message}");
                return null;
            }
        }

        private void VerificarPartida()
        {
            Cuenta existente = LeerCuenta();
            if (existente != null)
            {
                Debug.Log($"MotorEventos: usando la partida de {RutaBanco} " +
                          $"(saldo {Dinero(existente.saldoActual)}, {existente.historial?.Count ?? 0} movimientos).");
                return;
            }

            if (!crearPartidaSiNoExiste)
            {
                Debug.LogWarning($"MotorEventos: no existe {RutaBanco}. Juega primero el domingo " +
                                 "o marca 'Crear Partida Si No Existe' para probar la escena sola.");
                return;
            }

            var cuenta = new Cuenta { saldoActual = mesadaDePrueba, ingresosTotales = mesadaDePrueba };
            cuenta.historial.Add(new Movimiento { descripcion = "Mesada semanal", monto = mesadaDePrueba, esIngreso = true });
            File.WriteAllText(RutaBanco, JsonUtility.ToJson(cuenta, true));

            Debug.LogWarning($"MotorEventos: no había partida; se creó una de prueba con {Dinero(mesadaDePrueba)}.");
        }

        // =====================================================================
        //  PERSISTENCIA: BIENESTAR (datosBienestar.json, siempre 0–100)
        // =====================================================================

        private void AplicarBienestar(float cambio)
        {
            if (Mathf.Approximately(cambio, 0f)) return;

            float nuevo = Mathf.Clamp(LeerBienestar() + cambio, BienestarMin, BienestarMax);
            File.WriteAllText(RutaBienestar, JsonUtility.ToJson(new DatosDeBienestar { bienestarActual = nuevo }, true));
            SincronizarScriptsDeLaEscena();
            RefrescarHUD();
            MostrarDelta(textoDeltaBienestar, BienestarConSigno(cambio), ref rutinaDeltaBienestar);
        }

        private float LeerBienestar()
        {
            if (!File.Exists(RutaBienestar)) return BienestarInicial;

            try
            {
                DatosDeBienestar d = JsonUtility.FromJson<DatosDeBienestar>(File.ReadAllText(RutaBienestar));
                return d != null ? Mathf.Clamp(d.bienestarActual, BienestarMin, BienestarMax) : BienestarInicial;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"MotorEventos: no se pudo leer {ArchivoBienestar}. {e.Message}");
                return BienestarInicial;
            }
        }

        // =====================================================================
        //  MEDALLAS (misma clave que MedallasManager: "Medalla_" + id)
        // =====================================================================

        private void DesbloquearMedalla(string id)
        {
            PlayerPrefs.SetInt("Medalla_" + id, 1);
            PlayerPrefs.Save();
            alDesbloquearMedalla?.Invoke(id); // p. ej. MedallasManager.DesbloquearMedalla para la notificación
        }

        // =====================================================================
        //  HUD Y FORMATO
        // =====================================================================

        private void RefrescarHUD(bool instantaneo = false)
        {
            // Dinero: la barra llena = todo lo que ha entrado (mesada + ingresos); baja con cada gasto
            Cuenta cuenta = LeerCuenta();
            int saldo = cuenta?.saldoActual ?? 0;
            int ingresos = cuenta?.ingresosTotales ?? 0;
            rellenoIngresosObjetivo = ingresos > 0 ? Mathf.Clamp01((float)saldo / ingresos) : 0f;
            if (textoSaldo != null) textoSaldo.text = Dinero(saldo);

            // Bienestar: 0–100
            float bienestar = LeerBienestar();
            rellenoObjetivo = bienestar / BienestarMax;
            if (textoBienestar != null) textoBienestar.text = $"{bienestar:0}/{BienestarMax:0}";

            if (instantaneo)
            {
                if (barraBienestar != null) barraBienestar.fillAmount = rellenoObjetivo;
                if (barraIngresos != null) barraIngresos.fillAmount = rellenoIngresosObjetivo;
            }
        }

        // Texto flotante junto a la barra: aparece con el cambio y se oculta tras unos segundos
        private void MostrarDelta(TextMeshProUGUI campo, string valor, ref Coroutine rutina)
        {
            if (campo == null) return;
            if (rutina != null) StopCoroutine(rutina);
            rutina = StartCoroutine(RutinaDelta(campo, valor));
        }

        private IEnumerator RutinaDelta(TextMeshProUGUI campo, string valor)
        {
            campo.text = valor;
            campo.gameObject.SetActive(true);
            yield return new WaitForSeconds(segundosDelta);
            campo.gameObject.SetActive(false);
        }

        /// <summary>Saldo del HUD: 85000 → "$85.000"; -5000 → "-$5.000"</summary>
        private static string Dinero(int valor) =>
            (valor < 0 ? "-" : "") + "$" + Mathf.Abs(valor).ToString("N0", FormatoPesos);

        /// <summary>-30000 → "-$30.000"; 15000 → "+$15.000"</summary>
        private static string DineroConSigno(int valor) =>
            (valor < 0 ? "-" : "+") + "$" + Mathf.Abs(valor).ToString("N0", FormatoPesos);

        /// <summary>100 → "+100 SANIDAD"; -20 → "-20 SANIDAD"</summary>
        private static string BienestarConSigno(float valor) =>
            $"{(valor < 0 ? "-" : "+")}{Mathf.Abs(valor):0} SANIDAD";

        // =====================================================================
        //  UTILIDADES DE PANELES
        // =====================================================================

        private void MostrarSolo(GameObject panel)
        {
            OcultarPaneles();
            if (panel == null) return;

            panel.SetActive(true);
            if (!panel.activeInHierarchy)
                Debug.LogError($"MotorEventos: '{panel.name}' se encendió pero no se ve porque uno de sus padres está apagado.");
        }

        /// <summary>
        /// Revisa los dos errores de jerarquía que hacen que el panel de decisión no aparezca:
        ///  1. Un panel dentro de otro panel (al apagar uno se apaga el otro).
        ///  2. Este script dentro de un panel (al apagar el panel se apaga el motor).
        /// </summary>
        private bool ValidarJerarquia()
        {
            var paneles = new[] { panelNarrativa, panelDecision, panelConsecuencia, panelFin };
            bool ok = true;

            foreach (GameObject panel in paneles)
            {
                if (panel == null) continue;

                if (transform.IsChildOf(panel.transform))
                {
                    Debug.LogError($"MotorEventos: este script está dentro de '{panel.name}'. Muévelo a un objeto " +
                                   "fuera de los paneles (por ejemplo '--- Sistemas ---/MotorEventos').");
                    ok = false;
                }

                foreach (GameObject otro in paneles)
                {
                    if (otro == null || otro == panel) continue;
                    if (otro.transform.IsChildOf(panel.transform))
                    {
                        Debug.LogError($"MotorEventos: '{otro.name}' está dentro de '{panel.name}'. Los paneles deben ser " +
                                       "hermanos (hijos directos del Canvas), no uno dentro del otro.");
                        ok = false;
                    }
                }
            }

            // Dice exactamente qué casilla falta y, al hacer clic en el error, resalta el objeto con este script
            var faltantes = new List<string>();
            if (panelNarrativa == null) faltantes.Add("Panel Narrativa");
            if (textoNarrativa == null) faltantes.Add("Texto Narrativa");
            if (botonSiguiente == null) faltantes.Add("Boton Siguiente");
            if (panelDecision == null) faltantes.Add("Panel Decision");
            if (textoDecision == null) faltantes.Add("Texto Decision");
            if (panelConsecuencia == null) faltantes.Add("Panel Consecuencia");
            if (textoConsecuencia == null) faltantes.Add("Texto Consecuencia");
            if (botonContinuar == null) faltantes.Add("Boton Continuar");

            if (faltantes.Count > 0)
            {
                Debug.LogError($"MotorEventos en '{name}': faltan por asignar → {string.Join(", ", faltantes)}", this);
                ok = false;
            }

            return ok;
        }

        private void OcultarPaneles()
        {
            if (panelNarrativa != null) panelNarrativa.SetActive(false);
            if (panelDecision != null) panelDecision.SetActive(false);
            if (panelConsecuencia != null) panelConsecuencia.SetActive(false);
            if (panelFin != null) panelFin.SetActive(false);
        }

        private static void MostrarLinea(TextMeshProUGUI campo, bool visible, string valor)
        {
            if (campo == null) return;
            campo.gameObject.SetActive(visible);
            if (visible) campo.text = valor;
        }

        // =====================================================================
        //  AJUSTE DE TEXTOS: ningún texto puede salirse de su cuadro
        // =====================================================================

        /// <summary>
        /// Se ejecuta al iniciar (y también con clic derecho → "Ajustar textos ahora" sin dar Play).
        /// Para cada texto del motor:
        ///  1. Revisa la escala del texto y de sus padres (una escala desigual estira las letras).
        ///  2. Lo encierra dentro de su objeto padre (el cuadro naranja o el botón) y le da un
        ///     tamaño mínimo razonable (evita la "columna" de una letra por línea).
        ///  3. Activa saltos de línea, letra automática (se encoge si no cabe) y recorte con "…".
        /// </summary>
        private void ConfigurarTextos()
        {
            // Los rectángulos de la UI deben estar calculados antes de medirlos
            Canvas.ForceUpdateCanvases();

            // Textos largos: se encierran en su cuadro
            foreach (TextMeshProUGUI t in new[] { textoNarrativa, textoDecision, textoConsecuencia, textoMensajePersonaje })
                AjustarTexto(t, tamanoMaximoTextos, encerrar: true);

            // Líneas cortas (cambios de dinero/bienestar y textos junto a las barras): solo letra automática
            foreach (TextMeshProUGUI t in new[] { textoCambioDinero, textoCambioBienestar, textoDeltaIngresos, textoDeltaBienestar })
                AjustarTexto(t, tamanoMaximoTextos, encerrar: false);

            // Textos de los botones: se encierran dentro de su botón
            foreach (BotonOpcion ui in botonesOpcion)
            {
                if (ui == null) continue;
                AjustarTexto(ui.etiqueta, tamanoMaximoBotones, encerrar: true);
                AjustarTexto(ui.impacto, tamanoMaximoBotones * 0.7f, encerrar: false);
            }
            AjustarTexto(TextoDeBoton(botonSiguiente), tamanoMaximoBotones, encerrar: true);
            AjustarTexto(TextoDeBoton(botonContinuar), tamanoMaximoBotones, encerrar: true);

            AvisarBotonesSueltos();
        }

        private void AjustarTexto(TextMeshProUGUI t, float tamanoMaximo, bool encerrar)
        {
            if (t == null) return;

            RevisarEscala(t.rectTransform);

            if (encerrar && encerrarEnSuPadre) EncerrarEnPadre(t.rectTransform);

            t.enableWordWrapping = true;                   // salta de línea al llegar al borde
            t.enableAutoSizing = true;                     // reduce la letra hasta que quepa
            t.fontSizeMin = tamanoMinimo;
            t.fontSizeMax = Mathf.Max(tamanoMinimo, tamanoMaximo);
            t.lineSpacing = 0f;                            // sin espacio extra entre líneas
            t.characterSpacing = 0f;
            t.wordSpacing = 0f;
            t.overflowMode = TextOverflowModes.Ellipsis;   // si ni así cabe, corta con "…" (nunca se sale)
            t.margin = Vector4.zero;                       // el margen ya lo pone EncerrarEnPadre
        }

        /// <summary>
        /// Una escala distinta de (1,1,1) en el texto o en un padre estira las letras (altas y delgadas).
        /// El texto se corrige solo; los padres solo se avisan, porque cambiarlos movería todo el diseño.
        /// </summary>
        private void RevisarEscala(RectTransform rt)
        {
            if (rt.localScale != Vector3.one)
            {
                Debug.Log($"MotorEventos: '{rt.name}' tenía Scale {rt.localScale}; se puso en (1,1,1).", rt);
                rt.localScale = Vector3.one;
            }

            for (Transform p = rt.parent; p != null && p.GetComponent<Canvas>() == null; p = p.parent)
            {
                Vector3 s = p.localScale;
                if (!Mathf.Approximately(s.x, s.y))
                    Debug.LogWarning($"MotorEventos: '{p.name}' (padre de '{rt.name}') tiene Scale {s}. " +
                                     "Ponlo en (1,1,1) y cambia su tamaño con Width/Height; si no, las letras se ven estiradas.", p);
            }
        }

        /// <summary>
        /// Deja el rectángulo del texto dentro de su padre (con 'margenInterno') y con un tamaño mínimo:
        ///  - Si se sale, se recorta a la parte que queda dentro.
        ///  - Si quedaba muy angosto (la "columna" de letras) o muy bajito, se ensancha/alarga al padre.
        ///  - Si ya estaba bien, no se toca.
        /// No toca textos manejados por un Layout Group (ahí manda el Layout Group).
        /// </summary>
        private void EncerrarEnPadre(RectTransform rt)
        {
            if (!(rt.parent is RectTransform padre)) return;
            if (padre.GetComponent<LayoutGroup>() != null) return;

            Rect area = padre.rect;
            float margen = Mathf.Min(margenInterno, area.width * 0.1f, area.height * 0.1f); // en botones pequeños, margen pequeño
            float xMinPermitido = area.xMin + margen, xMaxPermitido = area.xMax - margen;
            float yMinPermitido = area.yMin + margen, yMaxPermitido = area.yMax - margen;
            float anchoPermitido = xMaxPermitido - xMinPermitido;
            float altoPermitido = yMaxPermitido - yMinPermitido;
            if (anchoPermitido <= 0f || altoPermitido <= 0f) return; // padre sin tamaño

            // Esquinas actuales del texto, medidas en el espacio del padre
            var esquinas = new Vector3[4];
            rt.GetWorldCorners(esquinas);
            Vector2 abajoIzq = padre.InverseTransformPoint(esquinas[0]);
            Vector2 arribaDer = padre.InverseTransformPoint(esquinas[2]);

            // 1. Intersección entre el texto y el área permitida del padre
            float xMin = Mathf.Max(abajoIzq.x, xMinPermitido);
            float xMax = Mathf.Min(arribaDer.x, xMaxPermitido);
            float yMin = Mathf.Max(abajoIzq.y, yMinPermitido);
            float yMax = Mathf.Min(arribaDer.y, yMaxPermitido);

            // 2. Tamaño mínimo: angosto (o fuera del padre) → todo el ancho; bajito → todo el alto
            if (xMax - xMin < Mathf.Max(60f, anchoPermitido * 0.4f)) { xMin = xMinPermitido; xMax = xMaxPermitido; }
            if (yMax - yMin < Mathf.Max(30f, altoPermitido * 0.2f)) { yMin = yMinPermitido; yMax = yMaxPermitido; }

            bool sinCambios = Mathf.Abs(xMin - abajoIzq.x) < 0.5f && Mathf.Abs(xMax - arribaDer.x) < 0.5f &&
                              Mathf.Abs(yMin - abajoIzq.y) < 0.5f && Mathf.Abs(yMax - arribaDer.y) < 0.5f;
            if (sinCambios) return; // el diseño del Inspector ya es correcto

            RegistrarDeshacer(rt);

            // 3. Se fija con anclas relativas al padre: así sigue dentro aunque cambie la resolución
            rt.anchorMin = new Vector2((xMin - area.xMin) / area.width, (yMin - area.yMin) / area.height);
            rt.anchorMax = new Vector2((xMax - area.xMin) / area.width, (yMax - area.yMin) / area.height);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Debug.Log($"MotorEventos: '{rt.name}' se salía de '{padre.name}' (o era muy angosto) y se ajustó para quedar dentro.", rt);
        }

        /// <summary>
        /// Avisa de botones que están dentro de los paneles pero que el motor no controla
        /// (como el "Button" suelto que aparece en pantalla). No los borra: solo dice cuáles son.
        /// </summary>
        private void AvisarBotonesSueltos()
        {
            var conocidos = new HashSet<Button> { botonSiguiente, botonContinuar };
            foreach (BotonOpcion ui in botonesOpcion)
                if (ui != null && ui.boton != null) conocidos.Add(ui.boton);

            foreach (GameObject panel in new[] { panelNarrativa, panelDecision, panelConsecuencia })
            {
                if (panel == null) continue;
                foreach (Button b in panel.GetComponentsInChildren<Button>(true))
                {
                    if (conocidos.Contains(b)) continue;
                    Debug.LogWarning($"MotorEventos: el botón '{b.name}' está en '{panel.name}' pero no está asignado en el motor. " +
                                     "Asígnalo o bórralo.", b);
                }
            }

            // Botón "Siguiente" fuera del panel de narrativa → se ve en todos los paneles
            if (botonSiguiente != null && panelNarrativa != null && !botonSiguiente.transform.IsChildOf(panelNarrativa.transform))
                Debug.LogWarning($"MotorEventos: '{botonSiguiente.name}' (Boton Siguiente) está FUERA de '{panelNarrativa.name}'. " +
                                 "Arrástralo dentro del panel o se verá en las demás pantallas.", botonSiguiente);
            if (botonContinuar != null && panelConsecuencia != null && !botonContinuar.transform.IsChildOf(panelConsecuencia.transform))
                Debug.LogWarning($"MotorEventos: '{botonContinuar.name}' (Boton Continuar) está FUERA de '{panelConsecuencia.name}'.", botonContinuar);
        }

        // =====================================================================
        //  CORRECCIÓN DE ESCALAS (Scale → 1 sin mover ni cambiar el tamaño en pantalla)
        // =====================================================================

        /// <summary>
        /// Muchos objetos de la UI se agrandaron con la herramienta de escalar, por ejemplo un panel con
        /// Scale (6.85, 86.93) y sus textos con Scale (2.01, 0.23) para "compensar". Eso estira las letras
        /// y hace que la letra automática calcule mal. Este método:
        ///  1. Toma cada panel y sube hasta el objeto que cuelga directamente del Canvas.
        ///  2. Guarda dónde se ve en pantalla cada objeto de ese grupo (sus 4 esquinas).
        ///  3. Pone Scale (1,1,1) a todos y ajusta su Width/Height para que se vean en el MISMO lugar.
        /// Resultado: el diseño se ve igual, pero sin escalas raras.
        /// </summary>
        private void CorregirEscalas()
        {
            Canvas.ForceUpdateCanvases();

            var raices = new HashSet<RectTransform>();
            foreach (GameObject panel in new[] { panelNarrativa, panelDecision, panelConsecuencia, panelFin })
            {
                RectTransform raiz = RaizBajoElCanvas(panel);
                if (raiz != null) raices.Add(raiz);
            }

            foreach (RectTransform raiz in raices) CorregirEscalasDe(raiz);
        }

        /// <summary>Sube desde el panel hasta el objeto cuyo padre es el Canvas.</summary>
        private static RectTransform RaizBajoElCanvas(GameObject panel)
        {
            if (panel == null || !(panel.transform is RectTransform actual)) return null;
            while (actual.parent is RectTransform padre && padre.GetComponent<Canvas>() == null)
                actual = padre;
            return actual;
        }

        private void CorregirEscalasDe(RectTransform raiz)
        {
            // GetComponentsInChildren devuelve primero el padre y luego sus hijos: así cada hijo
            // se acomoda cuando su padre ya quedó corregido.
            RectTransform[] todos = raiz.GetComponentsInChildren<RectTransform>(true);

            bool hayEscalaRara = false;
            foreach (RectTransform rt in todos)
                if (rt.localScale != Vector3.one) { hayEscalaRara = true; break; }
            if (!hayEscalaRara) return;

            // 1. Dónde se ve cada objeto ahora mismo (esquina inferior izquierda y superior derecha)
            var abajoIzq = new Dictionary<RectTransform, Vector3>();
            var arribaDer = new Dictionary<RectTransform, Vector3>();
            var esquinas = new Vector3[4];
            foreach (RectTransform rt in todos)
            {
                rt.GetWorldCorners(esquinas);
                abajoIzq[rt] = esquinas[0];
                arribaDer[rt] = esquinas[2];
            }

            // 2. Escala 1 y el mismo rectángulo en pantalla (manteniendo sus anclas)
            int corregidos = 0;
            foreach (RectTransform rt in todos)
            {
                if (!(rt.parent is RectTransform padre)) continue;
                RegistrarDeshacer(rt);

                if (rt.localScale != Vector3.one) corregidos++;
                rt.localScale = Vector3.one;

                Vector2 min = padre.InverseTransformPoint(abajoIzq[rt]);
                Vector2 max = padre.InverseTransformPoint(arribaDer[rt]);
                Rect area = padre.rect;
                Vector2 referenciaMin = area.min + Vector2.Scale(rt.anchorMin, area.size);
                Vector2 referenciaMax = area.min + Vector2.Scale(rt.anchorMax, area.size);

                rt.offsetMin = min - referenciaMin;
                rt.offsetMax = max - referenciaMax;
            }

            Canvas.ForceUpdateCanvases();
            Debug.Log($"MotorEventos: se corrigió la escala de {corregidos} objetos dentro de '{raiz.name}' " +
                      "(ahora Scale = 1, mismo tamaño en pantalla).", raiz);
        }

        private static TextMeshProUGUI TextoDeBoton(Button boton) =>
            boton != null ? boton.GetComponentInChildren<TextMeshProUGUI>(true) : null;

        /// <summary>Pone el texto y avisa en consola si no cupo ni con la letra mínima.</summary>
        private void PonerTexto(TextMeshProUGUI campo, string valor)
        {
            if (campo == null) return;
            campo.text = valor;
            if (!forzarTextosDentroDelCuadro || !campo.gameObject.activeInHierarchy) return;

            campo.ForceMeshUpdate();
            if (campo.isTextOverflowing)
                Debug.LogWarning($"MotorEventos: el texto de '{campo.name}' no cabe ni con letra {tamanoMinimo:0}; " +
                                 "se cortó con '…'. Agranda su cuadro o baja 'Tamano Minimo'.", campo);
        }

        // Permite deshacer (Ctrl+Z) los cambios hechos con "Ajustar textos ahora" fuera de Play
        private static void RegistrarDeshacer(UnityEngine.Object objeto)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.Undo.RecordObject(objeto, "Ajustar textos del motor");
#endif
        }

        /// <summary>
        /// Clic derecho sobre el componente → "Ajustar textos ahora".
        /// Hace el mismo ajuste SIN dar Play y lo deja guardado en la escena, para ver el resultado
        /// mientras diseñas. Se puede deshacer con Ctrl+Z.
        /// </summary>
        [ContextMenu("Ajustar textos ahora (sin dar Play)")]
        private void AjustarTextosEnEditor()
        {
            var textos = new List<UnityEngine.Object>();
            foreach (TextMeshProUGUI t in new[] { textoNarrativa, textoDecision, textoConsecuencia, textoMensajePersonaje,
                                                  textoCambioDinero, textoCambioBienestar, textoDeltaIngresos, textoDeltaBienestar,
                                                  TextoDeBoton(botonSiguiente), TextoDeBoton(botonContinuar) })
                if (t != null) { textos.Add(t); textos.Add(t.rectTransform); }
            foreach (BotonOpcion ui in botonesOpcion)
            {
                if (ui?.etiqueta != null) { textos.Add(ui.etiqueta); textos.Add(ui.etiqueta.rectTransform); }
                if (ui?.impacto != null) { textos.Add(ui.impacto); textos.Add(ui.impacto.rectTransform); }
            }

#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(textos.ToArray(), "Ajustar textos del motor");
#endif
            CorregirEscalas();
            ConfigurarTextos();

#if UNITY_EDITOR
            foreach (UnityEngine.Object o in textos) UnityEditor.EditorUtility.SetDirty(o);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
            Debug.Log("MotorEventos: textos ajustados. Guarda la escena (Ctrl+S) para conservar el cambio.");
        }

        // =====================================================================
        //  HERRAMIENTAS DE PRUEBA (clic derecho sobre el componente en el Inspector)
        // =====================================================================

        // No borra datosBancarios.json ni datosBienestar.json: la partida real se conserva.
        [ContextMenu("Pruebas/Reiniciar cobro y medallas de los eventos")]
        private void ReiniciarPartida()
        {
            PlayerPrefs.DeleteKey(ClaveCobroDiferido);
            PlayerPrefs.DeleteKey("Medalla_companerismo");
            PlayerPrefs.DeleteKey("Medalla_buena_accion");
            PlayerPrefs.Save();
            Debug.Log("MotorEventos: cobro diferido y medallas de los eventos reiniciados (la partida se conserva).");
        }

        [ContextMenu("Pruebas/Mostrar estado en consola")]
        private void MostrarEstado()
        {
            Debug.Log($"Saldo {Dinero(LeerCuenta()?.saldoActual ?? 0)} · Bienestar {LeerBienestar():0} · " +
                      $"Cobro pendiente {PlayerPrefs.GetInt(ClaveCobroDiferido, 0) == 1} · " +
                      $"Compañerismo {PlayerPrefs.GetInt("Medalla_companerismo", 0) == 1} · " +
                      $"Una buena acción {PlayerPrefs.GetInt("Medalla_buena_accion", 0) == 1}");
        }
    }
}