using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBarrasController : MonoBehaviour
{
    public static UIBarrasController Instance;

    [Header("Valores de Estado")]
    public float bienestarMental = 50f;
    public float bienestarMaximo = 100f;
    public float dinero = 0f;
    public float dineroMaximo = 300000f;

    [Header("Referencias de UI")]
    public Image barraBienestarImage;
    public Image barraDineroImage;

    private string rutaBienestar;
    private string rutaBancarios;

    // --- Estructuras para los JSON ---
    [System.Serializable]
    public class DatosBienestar
    {
        public float valor;
    }

    [System.Serializable]
    public class TransaccionItem
    {
        public string descripcion;
        public float monto;
        public bool esIngreso;
    }

    [System.Serializable]
    public class DatosBancarios
    {
        public float saldoActual;
        public float ingresosTotales;
        public float gastosTotales;
        public TransaccionItem[] historial;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional, mantiene el controlador entre escenas si lo deseas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rutaBienestar = Path.Combine(Application.persistentDataPath, "datosBienestar.json");
        rutaBancarios = Path.Combine(Application.persistentDataPath, "datosBancarios.json");

        CargarDatos();
    }

    private void Start()
    {
        ActualizarUI();
    }

    public void ModificarBienestar(float cantidad)
    {
        bienestarMental = Mathf.Clamp(bienestarMental + cantidad, 0f, bienestarMaximo);
        ActualizarUI();
        GuardarDatosBienestar();
    }

    // Método centralizado para registrar ingresos o gastos y actualizar el JSON bancario
    public void RegistrarTransaccion(string descripcion, float monto, bool esIngreso)
    {
        DatosBancarios banco = CargarDatosBancariosDesdeArchivo();

        if (esIngreso)
        {
            banco.saldoActual += monto;
            banco.ingresosTotales += monto;
        }
        else
        {
            banco.saldoActual -= monto;
            banco.gastosTotales += monto;
        }

        // Manejo seguro del historial en forma de arreglo
        List<TransaccionItem> listaHistorial = new List<TransaccionItem>();
        if (banco.historial != null)
        {
            listaHistorial.AddRange(banco.historial);
        }

        listaHistorial.Add(new TransaccionItem
        {
            descripcion = descripcion,
            monto = monto,
            esIngreso = esIngreso
        });

        banco.historial = listaHistorial.ToArray();

        // Guardamos los cambios en el archivo JSON
        GuardarDatosBancariosEnArchivo(banco);

        // Actualizamos la variable local y la UI
        dinero = banco.saldoActual;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (barraBienestarImage != null)
        {
            barraBienestarImage.fillAmount = bienestarMental / bienestarMaximo;
        }

        if (barraDineroImage != null)
        {
            barraDineroImage.fillAmount = Mathf.Clamp01(dinero / dineroMaximo);
        }
    }

    public void TraerBarrasAlFrente()
    {
        transform.SetAsLastSibling();
    }

    // Método con parámetro para que puedas escribir el valor directamente en el Inspector
    public void PrecioGasto(float montoPersonalizado)
    {
        RegistrarTransaccion("PrecioAGastar", montoPersonalizado, false);
    }

    private void GuardarDatosBienestar()
    {
        DatosBienestar datosB = new DatosBienestar { valor = bienestarMental };
        File.WriteAllText(rutaBienestar, JsonUtility.ToJson(datosB, true));
    }

    private DatosBancarios CargarDatosBancariosDesdeArchivo()
    {
        if (File.Exists(rutaBancarios))
        {
            string jsonD = File.ReadAllText(rutaBancarios);
            DatosBancarios bancoCargado = JsonUtility.FromJson<DatosBancarios>(jsonD);
            if (bancoCargado != null)
            {
                return bancoCargado;
            }
        }

        // Si el archivo no existe o está corrupto, devolvemos una estructura base limpia
        return new DatosBancarios
        {
            saldoActual = 100000f,
            ingresosTotales = 300000f,
            gastosTotales = 0f,
            historial = new TransaccionItem[0]
        };
    }

    private void GuardarDatosBancariosEnArchivo(DatosBancarios datos)
    {
        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaBancarios, json);
    }

    private void CargarDatos()
    {
        // Cargar Bienestar
        if (File.Exists(rutaBienestar))
        {
            string jsonB = File.ReadAllText(rutaBienestar);
            DatosBienestar datosB = JsonUtility.FromJson<DatosBienestar>(jsonB);
            if (datosB != null) bienestarMental = datosB.valor;
        }
        else
        {
            GuardarDatosBienestar();
        }

        // Cargar Bancarios
        DatosBancarios banco = CargarDatosBancariosDesdeArchivo();
        dinero = banco.saldoActual;

        // Si no existía el archivo bancario, lo creamos con los valores iniciales correctos
        if (!File.Exists(rutaBancarios))
        {
            GuardarDatosBancariosEnArchivo(banco);
        }
    }
}