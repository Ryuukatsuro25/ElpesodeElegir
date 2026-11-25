using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIBarrasController : MonoBehaviour
{
    public static UIBarrasController Instance;

    [Header("Valores de Estado")]
    public float bienestarMental = 100f;
    public float bienestarMaximo = 100f;
    public float dinero = 100000f;
    public float dineroMaximo = 300000f;

    [Header("Valores Iniciales Nueva Partida")]
    public float bienestarInicial = 100f;
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

        CargarDatos();
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
        if (barraBienestarImage == null) barraBienestarImage = null;
        if (barraDineroImage == null) barraDineroImage = null;

        ActualizarUI();
        RefrescarGestorBancarioActivo();
    }

    private void Start()
    {
        ActualizarUI();
        RefrescarGestorBancarioActivo();
    }

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
        GuardarDatosBienestar();
        ActualizarUI();
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
        RefrescarGestorBancarioActivo();
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
        RefrescarGestorBancarioActivo();
    }

    private void RefrescarGestorBancarioActivo()
    {
        GestorBancario gestor = FindObjectOfType<GestorBancario>();
        if (gestor != null)
        {
            gestor.CargarDatos();
        }
    }

    private void GuardarDatosBienestar()
    {
        try
        {
            DatosBienestar datosB = new DatosBienestar { valor = bienestarMental };
            File.WriteAllText(rutaBienestar, JsonUtility.ToJson(datosB, true));
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error al guardar datosBienestar.json: {ex.Message}");
        }
    }

    private DatosBancarios CargarDatosBancariosDesdeArchivo()
    {
        if (File.Exists(rutaBancarios))
        {
            try
            {
                string jsonD = File.ReadAllText(rutaBancarios);
                DatosBancarios bancoCargado = JsonUtility.FromJson<DatosBancarios>(jsonD);
                if (bancoCargado != null)
                {
                    return bancoCargado;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error al leer datosBancarios.json: {ex.Message}");
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
    public void AplicarGasto(float monto)
    {
        if (UIBarrasController.Instance != null)
        {
            UIBarrasController.Instance.PrecioGasto(monto);
        }
    }

    private void GuardarDatosBancariosEnArchivo(DatosBancarios datos)
    {
        try
        {
            string json = JsonUtility.ToJson(datos, true);
            File.WriteAllText(rutaBancarios, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error al guardar datosBancarios.json: {ex.Message}");
        }
    }

    private void CargarDatos()
    {
        if (File.Exists(rutaBienestar))
        {
            try
            {
                string jsonB = File.ReadAllText(rutaBienestar);
                DatosBienestar datosB = JsonUtility.FromJson<DatosBienestar>(jsonB);
                if (datosB != null)
                {
                    bienestarMental = Mathf.Clamp(datosB.valor, 0f, bienestarMaximo);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error al leer datosBienestar.json: {ex.Message}");
            }
        }
        else
        {
            bienestarMental = Mathf.Clamp(bienestarInicial, 0f, bienestarMaximo);
            GuardarDatosBienestar();
        }

        DatosBancarios banco = CargarDatosBancariosDesdeArchivo();
        dinero = banco.saldoActual;

        if (!File.Exists(rutaBancarios))
        {
            GuardarDatosBancariosEnArchivo(banco);
        }

        ActualizarUI();
    }
}