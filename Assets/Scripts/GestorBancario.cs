
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Transaccion
{
    public string descripcion;
    public int monto;
    public bool esIngreso;
}

[System.Serializable]
public class DatosBancarios
{
    public int saldoActual;
    public int ingresosTotales;
    public int gastosTotales;
    public List<Transaccion> historial = new List<Transaccion>();
}

public class GestorBancario : MonoBehaviour
{
    [Header("Conexiones de UI")]
    public TextMeshProUGUI textoSaldoActual;
    public TextMeshProUGUI textoIngresos;
    public TextMeshProUGUI textoGastos;
    public TextMeshProUGUI textoHistorial;

    [Header("Scroll del Historial")]
    public ScrollRect scrollHistorial;

    private DatosBancarios datos;
    private string rutaArchivo;

    void Awake()
    {
        rutaArchivo = Application.persistentDataPath + "/datosBancarios.json";
        CargarDatos();
    }

    public void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            datos = JsonUtility.FromJson<DatosBancarios>(json);

            if (datos == null)
            {
                datos = new DatosBancarios();
                GenerarMesadaInicial();
            }

            if (datos.historial == null)
            {
                datos.historial = new List<Transaccion>();
            }
        }
        else
        {
            datos = new DatosBancarios();
            GenerarMesadaInicial();
        }

        ActualizarUI();
    }

    private void GenerarMesadaInicial()
    {
        int multiplicador = Random.Range(8, 13);
        int montoMesada = multiplicador * 10000;

        Transaccion nuevaMesada = new Transaccion
        {
            monto = montoMesada,
            descripcion = "Mesada semanal",
            esIngreso = true
        };

        datos.historial.Add(nuevaMesada);
        datos.saldoActual += montoMesada;
        datos.ingresosTotales += montoMesada;

        GuardarDatos();
    }

    public void GuardarDatos()
    {
        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivo, json);
    }

    public void AgregarTransaccion(int monto, string descripcion, bool esIngreso)
    {
        if (datos == null)
        {
            datos = new DatosBancarios();
        }

        Transaccion nueva = new Transaccion
        {
            monto = monto,
            descripcion = descripcion,
            esIngreso = esIngreso
        };

        datos.historial.Add(nueva);

        if (esIngreso)
        {
            datos.saldoActual += monto;
            datos.ingresosTotales += monto;
        }
        else
        {
            datos.saldoActual -= monto;
            datos.gastosTotales += monto;
        }

        GuardarDatos();
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (datos == null)
            return;

        if (textoSaldoActual != null)
            textoSaldoActual.text = $"${datos.saldoActual:N0}";

        if (textoIngresos != null)
            textoIngresos.text = $"+${datos.ingresosTotales:N0}";

        if (textoGastos != null)
            textoGastos.text = $"-${datos.gastosTotales:N0}";

        if (textoHistorial != null)
        {
            textoHistorial.text = "";

            for (int i = datos.historial.Count - 1; i >= 0; i--)
            {
                Transaccion t = datos.historial[i];

                string colorHex = t.esIngreso ? "#32CD32" : "#FF0000";
                string signo = t.esIngreso ? "+" : "-";

                textoHistorial.text +=
                    $"{t.descripcion}:\n<color={colorHex}>{signo}${t.monto:N0}</color>\n\n";
            }
        }

        if (scrollHistorial != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollHistorial.verticalNormalizedPosition = 1f;
        }
    }

    public void BorrarDatosParaNuevaPartida()
    {
        if (File.Exists(rutaArchivo))
        {
            File.Delete(rutaArchivo);
            Debug.Log("Datos borrados. Reinicia el juego para ver una nueva mesada.");
        }
    }
}
