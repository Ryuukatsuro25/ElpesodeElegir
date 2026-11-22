using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// =====================================================================
//  GUARDADO DEL JUEGO (JSON en Application.persistentDataPath)
//
//  En PC: C:\Users\<usuario>\AppData\LocalLow\DefaultCompany\ElPesode-Elegir
//
//  Un solo lugar para leer y escribir los JSON del juego sin que falle:
//   - Si el archivo NO existe  → devuelve NoExiste (el script decide qué crear).
//   - Si está VACÍO            → devuelve Vacio.
//   - Si está CORRUPTO         → guarda una copia "<archivo>.corrupto" para no
//                                perder nada y devuelve Corrupto.
//   - Al escribir, primero escribe un ".tmp" y luego lo copia encima: si el
//     juego se cierra a mitad de la escritura, el archivo bueno no se daña.
//
//  También trae las reglas compartidas de los dos indicadores:
//   - Dinero:    GuardadoJuego.RegistrarMovimiento(-30000, "Reparación");
//   - Bienestar: GuardadoJuego.CambiarBienestar(-20);   (siempre 0–100)
//
//  Los formatos son IDÉNTICOS a los de GestorBancario / AppBanco
//  (datosBancarios.json) y PopUpManager / GestorBienestar (datosBienestar.json).
// =====================================================================
public static class GuardadoJuego
{
    public const string ArchivoBanco = "datosBancarios.json";
    public const string ArchivoBienestar = "datosBienestar.json";

    public const float BienestarMinimo = 0f;
    public const float BienestarMaximo = 100f;
    public const float BienestarInicial = 50f;

    public const string DescripcionMesada = "Mesada semanal";

    public enum Resultado { Ok, NoExiste, Vacio, Corrupto }

    // ---------------- FORMATO DE LOS ARCHIVOS ----------------

    [Serializable]
    public class MovimientoGuardado
    {
        public string descripcion;
        public int monto;          // siempre positivo; el signo lo da esIngreso
        public bool esIngreso;
    }

    [Serializable]
    public class CuentaGuardada
    {
        public int saldoActual;
        public int ingresosTotales;
        public int gastosTotales;
        public List<MovimientoGuardado> historial = new List<MovimientoGuardado>();
    }

    [Serializable]
    public class BienestarGuardado
    {
        public float bienestarActual;
    }

    // Para no llenar la consola: cada archivo corrupto se avisa una vez por sesión
    private static readonly HashSet<string> corruptosAvisados = new HashSet<string>();

    public static string Ruta(string archivo) => Path.Combine(Application.persistentDataPath, archivo);

    // =====================================================================
    //  LECTURA Y ESCRITURA GENÉRICAS
    // =====================================================================

    /// <summary>
    /// Lee un JSON a la clase T. Nunca lanza excepción.
    /// datos queda en null si el resultado no es Ok.
    /// </summary>
    public static Resultado Leer<T>(string archivo, out T datos) where T : class
    {
        datos = null;
        string ruta = Ruta(archivo);

        if (!File.Exists(ruta)) return Resultado.NoExiste;

        string texto;
        try
        {
            texto = File.ReadAllText(ruta);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GuardadoJuego: no se pudo abrir {archivo} ({e.Message}).");
            return Resultado.Corrupto;
        }

        if (string.IsNullOrWhiteSpace(texto)) return Resultado.Vacio;

        try
        {
            datos = JsonUtility.FromJson<T>(texto);
        }
        catch (Exception)
        {
            datos = null;
        }

        if (datos == null)
        {
            RespaldarCorrupto(ruta, archivo);
            return Resultado.Corrupto;
        }

        return Resultado.Ok;
    }

    /// <summary>Guarda T como JSON. Devuelve false (y avisa) si no se pudo escribir.</summary>
    public static bool Escribir<T>(string archivo, T datos)
    {
        string ruta = Ruta(archivo);
        string temporal = ruta + ".tmp";

        try
        {
            File.WriteAllText(temporal, JsonUtility.ToJson(datos, true));
            File.Copy(temporal, ruta, true);
            File.Delete(temporal);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"GuardadoJuego: no se pudo guardar {archivo} ({e.Message}).");
            return false;
        }
    }

    public static void Borrar(string archivo)
    {
        try
        {
            string ruta = Ruta(archivo);
            if (File.Exists(ruta)) File.Delete(ruta);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GuardadoJuego: no se pudo borrar {archivo} ({e.Message}).");
        }
    }

    private static void RespaldarCorrupto(string ruta, string archivo)
    {
        if (!corruptosAvisados.Add(ruta)) return;

        try
        {
            File.Copy(ruta, ruta + ".corrupto", true);
        }
        catch (Exception)
        {
            // Si ni siquiera se puede copiar, solo se avisa
        }

        Debug.LogWarning($"GuardadoJuego: {archivo} estaba dañado. Se guardó una copia como {archivo}.corrupto " +
                         "y se usarán valores nuevos.");
    }

    // =====================================================================
    //  DINERO (datosBancarios.json)
    // =====================================================================

    /// <summary>
    /// Devuelve la cuenta guardada. Si no existe, está vacía o corrupta:
    ///  - crearSiFalta = true  → crea la partida con la mesada del juego y la guarda.
    ///  - crearSiFalta = false → devuelve null.
    /// </summary>
    public static CuentaGuardada LeerCuenta(bool crearSiFalta)
    {
        Resultado r = Leer(ArchivoBanco, out CuentaGuardada cuenta);

        if (r == Resultado.Ok)
        {
            Normalizar(cuenta);
            return cuenta;
        }

        if (!crearSiFalta) return null;

        cuenta = NuevaCuentaConMesada();
        Escribir(ArchivoBanco, cuenta);
        Debug.Log($"GuardadoJuego: no había partida ({r}); se creó una con mesada de {Pesos(cuenta.saldoActual)}.");
        return cuenta;
    }

    /// <summary>Misma regla que GestorBancario/AppBanco: 80.000 a 120.000 de 10.000 en 10.000.</summary>
    public static CuentaGuardada NuevaCuentaConMesada()
    {
        int mesada = UnityEngine.Random.Range(8, 13) * 10000;
        var cuenta = new CuentaGuardada { saldoActual = mesada, ingresosTotales = mesada };
        cuenta.historial.Add(new MovimientoGuardado { descripcion = DescripcionMesada, monto = mesada, esIngreso = true });
        return cuenta;
    }

    /// <summary>
    /// Suma (monto > 0) o resta (monto &lt; 0) dinero, lo agrega al historial y guarda.
    /// Relee el archivo justo antes de escribir para no pisar cambios de otros scripts.
    /// Si no hay partida, la crea con la mesada del juego (nunca una cuenta sin mesada).
    /// </summary>
    public static CuentaGuardada RegistrarMovimiento(int monto, string descripcion)
    {
        CuentaGuardada cuenta = LeerCuenta(crearSiFalta: true);
        if (monto == 0) return cuenta;

        bool esIngreso = monto > 0;
        int valor = Mathf.Abs(monto);

        cuenta.historial.Add(new MovimientoGuardado
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

        Escribir(ArchivoBanco, cuenta);
        return cuenta;
    }

    /// <summary>
    /// Relleno de la barra de INGRESOS (0..1): saldo actual dividido por el saldo más alto
    /// que ha tenido la partida. Es la regla que ya usaba la barra de ingresos del equipo,
    /// así la barra muestra lo mismo en todas las escenas.
    /// </summary>
    public static float PorcentajeIngresos(CuentaGuardada cuenta)
    {
        if (cuenta == null) return 0f;

        float maximo = 0f, simulado = 0f;
        if (cuenta.historial != null)
        {
            foreach (MovimientoGuardado m in cuenta.historial)
            {
                simulado += m.esIngreso ? m.monto : -m.monto;
                if (simulado > maximo) maximo = simulado;
            }
        }

        if (maximo <= 0f) maximo = Mathf.Max(cuenta.ingresosTotales, cuenta.saldoActual);
        return maximo > 0f ? Mathf.Clamp01(cuenta.saldoActual / maximo) : 0f;
    }

    private static void Normalizar(CuentaGuardada cuenta)
    {
        if (cuenta.historial == null) cuenta.historial = new List<MovimientoGuardado>();
        foreach (MovimientoGuardado m in cuenta.historial)
        {
            if (m.monto < 0) m.monto = -m.monto;   // el signo lo da esIngreso
            if (m.descripcion == null) m.descripcion = "";
        }
        if (cuenta.ingresosTotales < 0) cuenta.ingresosTotales = 0;
        if (cuenta.gastosTotales < 0) cuenta.gastosTotales = 0;
    }

    // =====================================================================
    //  BIENESTAR (datosBienestar.json, siempre entre 0 y 100)
    // =====================================================================

    /// <summary>Lee el bienestar. Si no hay archivo válido, devuelve valorSiFalta (sin escribir).</summary>
    public static float LeerBienestar(float valorSiFalta = BienestarInicial)
    {
        if (Leer(ArchivoBienestar, out BienestarGuardado datos) != Resultado.Ok)
            return LimitarBienestar(valorSiFalta);

        return LimitarBienestar(datos.bienestarActual);
    }

    public static float GuardarBienestar(float valor)
    {
        float limitado = LimitarBienestar(valor);
        Escribir(ArchivoBienestar, new BienestarGuardado { bienestarActual = limitado });
        return limitado;
    }

    /// <summary>Suma o resta bienestar al valor GUARDADO (no a una copia vieja) y lo guarda.</summary>
    public static float CambiarBienestar(float cambio, float valorSiFalta = BienestarInicial)
    {
        return GuardarBienestar(LeerBienestar(valorSiFalta) + cambio);
    }

    public static float LimitarBienestar(float valor)
    {
        if (float.IsNaN(valor) || float.IsInfinity(valor)) valor = BienestarInicial;
        return Mathf.Clamp(valor, BienestarMinimo, BienestarMaximo);
    }

    // ---------------- FORMATO ----------------

    private static readonly System.Globalization.NumberFormatInfo FormatoPesos =
        new System.Globalization.NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalSeparator = "," };

    /// <summary>85000 → "$85.000"; -5000 → "-$5.000" (igual en cualquier idioma del celular)</summary>
    public static string Pesos(int valor) =>
        (valor < 0 ? "-" : "") + "$" + Mathf.Abs(valor).ToString("N0", FormatoPesos);
}
