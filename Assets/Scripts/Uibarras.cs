using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIBarrasController : MonoBehaviour
{
    public static UIBarrasController Instance;

    [Header("Valores de Estado")]
    public float bienestarMental = 50f;
    public float bienestarMaximo = 100f;
    public float dinero = 0f;
    public float dineroMaximo = 300000f;

    [Header("Valores Iniciales Nueva Partida")]
    public float bienestarInicial = 50f;
    public float saldoInicial = 100000f;
    public float ingresosIniciales = 300000f;

    [Header("Referencias de UI (Escena Activa)")]
    public Image barraBienestarImage;
    public Image barraDineroImage;

    private string rutaBienestar;
    private string rutaBancarios;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rutaBienestar = Path.Combine(Application.persistentDataPath, "datosBienestar.json");
        rutaBancarios = Path.Combine(Application.persistentDataPath, "datosBancarios.json");

        // Reinicia siempre al iniciar el juego para que bienestar vuelva a 100 y saldo a su valor inicial
        ReiniciarPartida();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActualizarUI();
    }

    private void Start()
    {
        ActualizarUI();
    }

    // Permite que cada escena (Día 1, Martes, etc.) registre sus propias imágenes de barra
    public void AsignarReferenciasUI(Image nuevaBarraBienestar, Image nuevaBarraDinero)
    {
        if (nuevaBarraBienestar != null)
            barraBienestarImage = nuevaBarraBienestar;

        if (nuevaBarraDinero != null)
            barraDineroImage = nuevaBarraDinero;

        ActualizarUI();
    }

    public void ModificarBienestar(float cantidad)
    {
        bienestarMental = Mathf.Clamp(bienestarMental + cantidad, 0f, bienestarMaximo);
        ActualizarUI();
        GuardarDatosBienestar();
    }

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

        GuardarDatosBancariosEnArchivo(banco);

        dinero = banco.saldoActual;
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        if (barraBienestarImage != null && bienestarMaximo > 0f)
        {
            barraBienestarImage.fillAmount = Mathf.Clamp01(bienestarMental / bienestarMaximo);
        }

        if (barraDineroImage != null && dineroMaximo > 0f)
        {
            barraDineroImage.fillAmount = Mathf.Clamp01(dinero / dineroMaximo);
        }
    }

    public void TraerBarrasAlFrente()
    {
        transform.SetAsLastSibling();
    }

    public void PrecioGasto(float montoPersonalizado)
    {
        RegistrarTransaccion("PrecioAGastar", montoPersonalizado, false);
    }

    // Útil al iniciar una nueva partida desde el menú principal
    public void ReiniciarPartida()
    {
        bienestarMental = Mathf.Clamp(bienestarInicial, 0f, bienestarMaximo);
        dinero = saldoInicial;

        GuardarDatosBienestar();

        DatosBancarios bancoLimpio = new DatosBancarios
        {
            saldoActual = saldoInicial,
            ingresosTotales = ingresosIniciales,
            gastosTotales = 0f,
            historial = new TransaccionItem[0]
        };

        GuardarDatosBancariosEnArchivo(bancoLimpio);
        ActualizarUI();
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

        return new DatosBancarios
        {
            saldoActual = saldoInicial,
            ingresosTotales = ingresosIniciales,
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
        if (File.Exists(rutaBienestar))
        {
            string jsonB = File.ReadAllText(rutaBienestar);
            DatosBienestar datosB = JsonUtility.FromJson<DatosBienestar>(jsonB);
            if (datosB != null) bienestarMental = datosB.valor;
        }
        else
        {
            bienestarMental = bienestarInicial;
            GuardarDatosBienestar();
        }

        DatosBancarios banco = CargarDatosBancariosDesdeArchivo();
        dinero = banco.saldoActual;

        if (!File.Exists(rutaBancarios))
        {
            GuardarDatosBancariosEnArchivo(banco);
        }
    }
}