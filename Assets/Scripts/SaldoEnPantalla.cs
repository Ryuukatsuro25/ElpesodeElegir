using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using TMPro;

// =====================================================================
//  SALDO DISPONIBLE EN CUALQUIER PANTALLA
//
//  Muestra el saldo actual (leído de datosBancarios.json) en un texto,
//  por ejemplo junto a las opciones del cumpleaños (EI4.5.3), para que
//  el estudiante compare el costo con lo que tiene antes de decidir.
//  Se actualiza solo cada medio segundo mientras el texto está visible.
// =====================================================================
public class SaldoEnPantalla : MonoBehaviour
{
    [System.Serializable]
    public class CuentaJson
    {
        public int saldoActual;
        public List<MovimientoJson> historial = new List<MovimientoJson>();
    }

    [System.Serializable]
    public class MovimientoJson
    {
        public string descripcion;
        public int monto;
        public bool esIngreso;
    }

    [Tooltip("Si lo dejas vacío, usa el TextMeshPro de este mismo objeto")]
    public TextMeshProUGUI texto;
    [Tooltip("{saldo} se reemplaza por el valor. Ej: Saldo disponible: {saldo}")]
    public string formato = "Saldo disponible: {saldo}";

    private static readonly NumberFormatInfo formatoPesos = new NumberFormatInfo
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ","
    };

    void OnEnable()
    {
        if (texto == null) texto = GetComponent<TextMeshProUGUI>();
        Actualizar();
        InvokeRepeating(nameof(Actualizar), 0.5f, 0.5f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Actualizar));
    }

    private void Actualizar()
    {
        if (texto == null) return;

        int saldo = 0;
        string ruta = Path.Combine(Application.persistentDataPath, "datosBancarios.json");
        if (File.Exists(ruta))
        {
            CuentaJson cuenta = JsonUtility.FromJson<CuentaJson>(File.ReadAllText(ruta));
            if (cuenta != null) saldo = cuenta.saldoActual;
        }

        texto.text = formato.Replace("{saldo}", "$" + saldo.ToString("N0", formatoPesos));
    }
}
