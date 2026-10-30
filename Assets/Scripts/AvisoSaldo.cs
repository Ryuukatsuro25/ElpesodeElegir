using UnityEngine;
using System.IO;
using TMPro;

// =====================================================================
//  AVISOS DE SALDO (INT5.1.2 — Escenario 2)
//
//  Revisa el saldo guardado en "datosBancarios.json" y, cuando BAJA:
//    - a "saldo bajo" (mayor que 0 y menor o igual al umbral)  -> panel "Saldo bajo"
//    - a 0 o menos                                             -> panel "Sin saldo"
//  Cada aviso se prende, dura unos segundos y se cierra solo.
//
//  Lee el JSON directamente, así que funciona con AppBanco o con
//  GestorBancario sin depender de sus nombres.
//
//  Ponlo en el objeto que tiene el banco ("GameObject"), que está siempre activo.
// =====================================================================
public class AvisoSaldo : MonoBehaviour
{
    // Solo el campo del JSON que este script necesita
    [System.Serializable]
    public class CuentaJson
    {
        public int saldoActual;
    }

    // El orden importa: de mejor a peor
    private enum EstadoSaldo { Normal, Bajo, SinSaldo }

    // ---------------- INSPECTOR ----------------

    [Header("Panel de saldo bajo")]
    public GameObject panelSaldoBajo;
    [Tooltip("Opcional. Si lo asignas, el script le escribe el mensaje")]
    public TextMeshProUGUI textoSaldoBajo;
    public string mensajeSaldoBajo = "Saldo bajo";

    [Header("Panel sin saldo")]
    public GameObject panelSinSaldo;
    [Tooltip("Opcional. Si lo asignas, el script le escribe el mensaje")]
    public TextMeshProUGUI textoSinSaldo;
    public string mensajeSinSaldo = "Sin saldo";

    [Header("Configuración")]
    [Tooltip("El saldo es 'bajo' cuando es mayor que 0 y menor o igual a este valor")]
    public int umbralSaldoBajo = 20000;
    [Tooltip("Segundos que dura cada aviso en pantalla")]
    public float duracion = 3f;
    [Tooltip("Cada cuántos segundos se revisa el saldo")]
    public float intervaloRevision = 0.5f;
    [Tooltip("Márcalo si también debe avisar al ENTRAR a la escena con el saldo ya bajo o en 0")]
    public bool avisarAlIniciar = false;
    [Tooltip("Opcional. Sonido al aparecer cualquier aviso. Ej: Consecuencia_Negativa")]
    public AudioClip sonidoAviso;

    // ---------------- VARIABLES INTERNAS ----------------

    private const string NOMBRE_ARCHIVO = "datosBancarios.json";
    private string rutaArchivo;
    private EstadoSaldo estadoAnterior = EstadoSaldo.Normal;

    // ---------------- CICLO DE UNITY ----------------

    void Start()
    {
        rutaArchivo = Path.Combine(Application.persistentDataPath, NOMBRE_ARCHIVO);

        // Los dos avisos empiezan apagados
        if (panelSaldoBajo != null) panelSaldoBajo.SetActive(false);
        if (panelSinSaldo != null) panelSinSaldo.SetActive(false);

        if (textoSaldoBajo != null) textoSaldoBajo.text = mensajeSaldoBajo;
        if (textoSinSaldo != null) textoSinSaldo.text = mensajeSinSaldo;

        // Estado con el que arranca la escena (no avisa, salvo que lo pidas)
        if (LeerSaldo(out int saldo))
        {
            EstadoSaldo inicial = CalcularEstado(saldo);
            if (avisarAlIniciar) MostrarAviso(inicial);
            estadoAnterior = inicial;
        }

        InvokeRepeating(nameof(RevisarSaldo), intervaloRevision, intervaloRevision);
    }

    // ---------------- LÓGICA ----------------

    private void RevisarSaldo()
    {
        if (!LeerSaldo(out int saldo)) return;

        EstadoSaldo actual = CalcularEstado(saldo);

        // Solo avisa cuando el saldo EMPEORA:
        // Normal -> Bajo, Normal -> Sin saldo, Bajo -> Sin saldo
        if (actual > estadoAnterior)
            MostrarAviso(actual);

        // Si el saldo sube (un ingreso), se "rearma" para volver a avisar después
        estadoAnterior = actual;
    }

    private EstadoSaldo CalcularEstado(int saldo)
    {
        if (saldo <= 0) return EstadoSaldo.SinSaldo;
        if (saldo <= umbralSaldoBajo) return EstadoSaldo.Bajo;
        return EstadoSaldo.Normal;
    }

    private void MostrarAviso(EstadoSaldo estado)
    {
        if (estado == EstadoSaldo.Normal) return;

        // Un solo aviso a la vez: "Sin saldo" reemplaza a "Saldo bajo"
        CerrarAvisos();

        GameObject panel = (estado == EstadoSaldo.SinSaldo) ? panelSinSaldo : panelSaldoBajo;
        if (panel == null)
        {
            Debug.LogWarning("AvisoSaldo: falta asignar el panel de '" +
                (estado == EstadoSaldo.SinSaldo ? mensajeSinSaldo : mensajeSaldoBajo) + "'.");
            return;
        }

        panel.SetActive(true);
        panel.transform.SetAsLastSibling(); // queda encima de lo demás

        ReproducirSonido();

        // Se cierra solo después de "duracion" segundos
        Invoke(nameof(CerrarAvisos), duracion);
    }

    // También se puede asignar a un botón "X" si quieres cerrarlo antes
    public void CerrarAvisos()
    {
        CancelInvoke(nameof(CerrarAvisos)); // no cancela la revisión del saldo

        if (panelSaldoBajo != null) panelSaldoBajo.SetActive(false);
        if (panelSinSaldo != null) panelSinSaldo.SetActive(false);
    }

    // ---------------- UTILIDADES ----------------

    // Lee el saldo del JSON; devuelve false si todavía no hay partida
    private bool LeerSaldo(out int saldo)
    {
        saldo = 0;
        if (!File.Exists(rutaArchivo)) return false;

        try
        {
            CuentaJson cuenta = JsonUtility.FromJson<CuentaJson>(File.ReadAllText(rutaArchivo));
            if (cuenta == null) return false;

            saldo = cuenta.saldoActual;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("AvisoSaldo: no se pudo leer datosBancarios.json. " + e.Message);
            return false;
        }
    }

    private void ReproducirSonido()
    {
        if (sonidoAviso == null) return;

        AudioSource fuente = GetComponent<AudioSource>();
        if (fuente == null) fuente = gameObject.AddComponent<AudioSource>();

        fuente.playOnAwake = false;
        fuente.PlayOneShot(sonidoAviso);
    }
}
