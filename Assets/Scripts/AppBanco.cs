using UnityEngine;
using UnityEngine.UI;               // ScrollRect y Button
using System.IO;
using System.Collections.Generic;
using System.Globalization;         // Formato de pesos con punto ($100.000)
using TMPro;

// =====================================================================
//  APP DEL BANCO (celular): saldo, ingresos, gastos, historial y
//  botones que suman o restan dinero.
//
//  Todas sus clases están DENTRO de AppBanco, por eso no chocan con
//  GestorBancario, Transaccion o DatosBancarios de otros scripts.
//
//  Usa el mismo archivo "datosBancarios.json" y los mismos nombres de
//  campos, así que la barra de ingresos y la partida actual siguen
//  funcionando sin cambios.
// =====================================================================
public class AppBanco : MonoBehaviour
{
    // ---------------- TIPOS INTERNOS ----------------

    // Un movimiento del historial
    [System.Serializable]
    public class MovimientoBanco
    {
        public string descripcion;
        public int monto;
        public bool esIngreso;
    }

    // Lo que se guarda en el JSON
    [System.Serializable]
    public class CuentaBanco
    {
        public int saldoActual;
        public int ingresosTotales;
        public int gastosTotales;
        public List<MovimientoBanco> historial = new List<MovimientoBanco>();
    }

    // Lo que hace cada botón (lista desplegable en el Inspector)
    public enum AccionBoton { Sumar, Restar }

    // Configuración de UN botón de dinero
    [System.Serializable]
    public class BotonDinero
    {
        [Tooltip("Arrastra aquí el botón desde la jerarquía")]
        public Button boton;

        [Tooltip("Sumar = ingreso (+)   Restar = gasto (-)")]
        public AccionBoton accion = AccionBoton.Restar;

        [Tooltip("Valor en pesos, siempre positivo. Ej: 18000")]
        public int monto = 5000;

        [Tooltip("Texto que aparecerá en el historial. Ej: Camisa")]
        public string descripcion = "";
    }

    // ---------------- INSPECTOR ----------------

    [Header("Textos de la app")]
    public TextMeshProUGUI textoSaldoActual;
    public TextMeshProUGUI textoIngresos;
    public TextMeshProUGUI textoGastos;
    public TextMeshProUGUI textoHistorial;

    [Header("Scroll del Historial")]
    public ScrollRect scrollHistorial; // Panel que tiene el componente Scroll Rect

    [Header("Botones de dinero de esta escena")]
    [Tooltip("Agrega un elemento por cada botón que deba sumar o restar dinero")]
    public List<BotonDinero> botonesDinero = new List<BotonDinero>();

    // ---------------- VARIABLES INTERNAS ----------------

    private const string NOMBRE_ARCHIVO = "datosBancarios.json";

    // Punto para miles ($100.000) sin cambiar la configuración del resto del juego
    private static readonly NumberFormatInfo formatoPesos = new NumberFormatInfo
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ","
    };

    private CuentaBanco cuenta;
    private string rutaArchivo;

    // ---------------- CICLO DE UNITY ----------------

    void Awake()
    {
        rutaArchivo = Path.Combine(Application.persistentDataPath, NOMBRE_ARCHIVO);
        ConectarBotones();
    }

    // Se ejecuta al iniciar y cada vez que se vuelve a mostrar la app,
    // así siempre muestra los datos más recientes del JSON
    void OnEnable()
    {
        CargarDatos();
    }

    // ---------------- BOTONES CONFIGURABLES ----------------

    private void ConectarBotones()
    {
        foreach (BotonDinero config in botonesDinero)
        {
            if (config.boton == null)
            {
                Debug.LogWarning("AppBanco: hay un elemento en 'Botones Dinero' sin botón asignado.");
                continue;
            }

            BotonDinero copia = config; // cada botón usa SU propia configuración
            copia.boton.onClick.AddListener(() => EjecutarBoton(copia));
        }
    }

    private void EjecutarBoton(BotonDinero config)
    {
        bool esIngreso = config.accion == AccionBoton.Sumar;

        // Si no escribiste descripción, usa una genérica
        string texto = string.IsNullOrEmpty(config.descripcion)
            ? (esIngreso ? "Ingreso" : "Gasto")
            : config.descripcion;

        // Mathf.Abs evita problemas si alguien escribe el monto en negativo
        AgregarTransaccion(Mathf.Abs(config.monto), texto, esIngreso);
    }

    // ---------------- DATOS Y JSON ----------------

    // Lee el JSON y refresca la pantalla
    public void CargarDatos()
    {
        LeerArchivo();
        ActualizarUI();
    }

    // Lee el JSON; si no existe, crea la partida con su mesada
    private void LeerArchivo()
    {
        cuenta = null;

        if (File.Exists(rutaArchivo))
            cuenta = JsonUtility.FromJson<CuentaBanco>(File.ReadAllText(rutaArchivo));

        if (cuenta == null) // no hay archivo (o está vacío): partida nueva
        {
            cuenta = new CuentaBanco();
            GenerarMesadaInicial();
        }

        if (cuenta.historial == null)
            cuenta.historial = new List<MovimientoBanco>();
    }

    // HU-DOM9.1.5: 80.000, 90.000, 100.000, 110.000 o 120.000
    private void GenerarMesadaInicial()
    {
        int montoMesada = Random.Range(8, 13) * 10000;

        cuenta.historial.Add(new MovimientoBanco
        {
            monto = montoMesada,
            descripcion = "Mesada semanal",
            esIngreso = true
        });

        cuenta.saldoActual += montoMesada;
        cuenta.ingresosTotales += montoMesada;

        GuardarDatos();
    }

    public void GuardarDatos()
    {
        File.WriteAllText(rutaArchivo, JsonUtility.ToJson(cuenta, true));
    }

    // Devuelve la mesada sorteada al inicio (primer movimiento del historial)
    public int MesadaInicial()
    {
        if (cuenta == null) LeerArchivo();
        return cuenta.historial.Count > 0 ? cuenta.historial[0].monto : 0;
    }

    // Se puede llamar desde los botones de la lista o desde otros scripts
    public void AgregarTransaccion(int monto, string descripcion, bool esIngreso)
    {
        // Relee el JSON antes de escribir, por si otro script lo cambió
        LeerArchivo();

        cuenta.historial.Add(new MovimientoBanco
        {
            monto = monto,
            descripcion = descripcion,
            esIngreso = esIngreso
        });

        if (esIngreso)
        {
            cuenta.saldoActual += monto;
            cuenta.ingresosTotales += monto;
        }
        else
        {
            cuenta.saldoActual -= monto;
            cuenta.gastosTotales += monto;
        }

        GuardarDatos();
        ActualizarUI();
    }

    // ---------------- INTERFAZ ----------------

    private static string Pesos(int valor)
    {
        return "$" + valor.ToString("N0", formatoPesos);
    }

    private void ActualizarUI()
    {
        if (cuenta == null) return;

        // Los "if" evitan errores si algún texto no está asignado
        if (textoSaldoActual != null) textoSaldoActual.text = Pesos(cuenta.saldoActual);
        if (textoIngresos != null) textoIngresos.text = "+" + Pesos(cuenta.ingresosTotales);
        if (textoGastos != null) textoGastos.text = "-" + Pesos(cuenta.gastosTotales);

        if (textoHistorial != null)
        {
            string lista = "";

            // Del movimiento más reciente al más antiguo
            for (int i = cuenta.historial.Count - 1; i >= 0; i--)
            {
                MovimientoBanco m = cuenta.historial[i];
                string colorHex = m.esIngreso ? "#32CD32" : "#FF0000";
                string signo = m.esIngreso ? "+" : "-";

                lista += $"{m.descripcion}:\n<color={colorHex}>{signo}{Pesos(m.monto)}</color>\n\n";
            }

            textoHistorial.text = lista;
        }

        // Recalcula el tamaño del texto y sube el scroll al movimiento más reciente
        if (scrollHistorial != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollHistorial.verticalNormalizedPosition = 1f;
        }
    }

    // ---------------- DESARROLLO ----------------

    // Borra la partida y sortea una mesada nueva al instante (sin reiniciar Play)
    public void BorrarDatosParaNuevaPartida()
    {
        if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo);

        cuenta = new CuentaBanco();
        GenerarMesadaInicial();
        ActualizarUI();

        Debug.Log("AppBanco: nueva partida, se sorteó una nueva mesada.");
    }
}