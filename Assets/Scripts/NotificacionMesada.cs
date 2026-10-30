using UnityEngine;
using UnityEngine.UI;               // Button
using System.IO;
using System.Collections.Generic;
using System.Globalization;         // Formato de pesos con punto ($100.000)
using TMPro;

// =====================================================================
//  NOTIFICACIÓN DE RECEPCIÓN DEL INGRESO SEMANAL
//
//  Al iniciar la escena prende un panel con el mensaje de la mesada.
//  El monto NO se sortea aquí: se lee de "datosBancarios.json", donde
//  el GestorBancario guardó la mesada aleatoria (80.000 a 120.000).
//
//  Ponlo en el MISMO objeto que tiene el GestorBancario ("GameObject"),
//  no dentro del panel: si el panel empieza apagado, el script no correría.
// =====================================================================
public class NotificacionMesada : MonoBehaviour
{
    // ---------------- TIPOS INTERNOS (solo para leer el JSON) ----------------

    [System.Serializable]
    public class MovimientoJson
    {
        public string descripcion;
        public int monto;
        public bool esIngreso;
    }

    [System.Serializable]
    public class CuentaJson
    {
        public int saldoActual;
        public int ingresosTotales;
        public int gastosTotales;
        public List<MovimientoJson> historial = new List<MovimientoJson>();
    }

    // ---------------- INSPECTOR ----------------

    [Header("Panel de la notificación")]
    public GameObject panelNotificacion;
    public TextMeshProUGUI textoNotificacion;
    [Tooltip("Opcional. Botón que cierra la notificación (ej: Aceptar)")]
    public Button botonCerrar;

    [Header("Mensaje")]
    [Tooltip("Escribe {monto} donde debe ir la mesada. Ej: $100.000")]
    [TextArea(3, 6)]
    public string mensaje =
        "¡Felicidades! Has recibido {monto}. Cuídalos y distribúyelos como consideres que es pertinente. " +
        "Recuerda no gastarlo todo, pero también recuerda disfrutar un poco.";

    [Header("Opcional")]
    [Tooltip("Segundos para que se cierre sola. 0 = solo se cierra con el botón")]
    public float cerrarDespuesDe = 0f;
    [Tooltip("Sonido al aparecer. Ej: Notificacion_Bancaria")]
    public AudioClip sonidoNotificacion;

    // ---------------- VARIABLES INTERNAS ----------------

    private const string NOMBRE_ARCHIVO = "datosBancarios.json";
    private const string DESCRIPCION_MESADA = "Mesada semanal";

    // Punto para miles ($100.000) sin cambiar la configuración del resto del juego
    private static readonly NumberFormatInfo formatoPesos = new NumberFormatInfo
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ","
    };

    // ---------------- CICLO DE UNITY ----------------

    // Start corre después del Awake del GestorBancario,
    // así que el JSON con la mesada ya existe en este punto
    void Start()
    {
        if (panelNotificacion == null || textoNotificacion == null)
        {
            Debug.LogError("NotificacionMesada: asigna el panel y el texto en el Inspector.");
            return;
        }

        int mesada = LeerMesada();

        // Arma el mensaje con el monto
        textoNotificacion.text = mensaje.Replace("{monto}", Pesos(mesada));

        // Prende el panel y lo pone encima del celular y los botones
        panelNotificacion.SetActive(true);
        panelNotificacion.transform.SetAsLastSibling();

        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(CerrarNotificacion);

        ReproducirSonido();

        if (cerrarDespuesDe > 0f)
            Invoke(nameof(CerrarNotificacion), cerrarDespuesDe);
    }

    // También puedes asignarlo a mano en el On Click de un botón
    public void CerrarNotificacion()
    {
        CancelInvoke(nameof(CerrarNotificacion));

        if (panelNotificacion != null)
            panelNotificacion.SetActive(false);
    }

    // ---------------- MESADA ----------------

    // Lee la mesada guardada en datosBancarios.json
    // (en PC: C:\Users\<usuario>\AppData\LocalLow\DefaultCompany\ElPesode-Elegir)
    private int LeerMesada()
    {
        string ruta = Path.Combine(Application.persistentDataPath, NOMBRE_ARCHIVO);
        CuentaJson cuenta = null;

        if (File.Exists(ruta))
            cuenta = JsonUtility.FromJson<CuentaJson>(File.ReadAllText(ruta));

        // Respaldo: si no hay GestorBancario en la escena y no existe el JSON,
        // crea la partida con el mismo formato y el mismo rango de mesada
        if (cuenta == null || cuenta.historial == null || cuenta.historial.Count == 0)
        {
            Debug.LogWarning("NotificacionMesada: no había datosBancarios.json; se creó una mesada nueva.");

            int monto = Random.Range(8, 13) * 10000; // 80.000, 90.000, 100.000, 110.000 o 120.000

            cuenta = new CuentaJson();
            cuenta.historial.Add(new MovimientoJson
            {
                descripcion = DESCRIPCION_MESADA,
                monto = monto,
                esIngreso = true
            });
            cuenta.saldoActual = monto;
            cuenta.ingresosTotales = monto;

            File.WriteAllText(ruta, JsonUtility.ToJson(cuenta, true));
            return monto;
        }

        // Busca el movimiento "Mesada semanal"
        foreach (MovimientoJson m in cuenta.historial)
            if (m.esIngreso && m.descripcion == DESCRIPCION_MESADA)
                return m.monto;

        // Partidas viejas sin esa descripción: usa el primer ingreso
        foreach (MovimientoJson m in cuenta.historial)
            if (m.esIngreso)
                return m.monto;

        return 0;
    }

    // ---------------- UTILIDADES ----------------

    private void ReproducirSonido()
    {
        if (sonidoNotificacion == null) return;

        AudioSource fuente = GetComponent<AudioSource>();
        if (fuente == null) fuente = gameObject.AddComponent<AudioSource>();

        fuente.playOnAwake = false;
        fuente.PlayOneShot(sonidoNotificacion);
    }

    private static string Pesos(int valor)
    {
        return "$" + valor.ToString("N0", formatoPesos);
    }
}
