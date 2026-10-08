using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ControladorBarraIngresos : MonoBehaviour
{
    [Header("Interfaz Visual")]
    public Image barraIngresos;

    [Header("Configuración de la Barra")]
    [Tooltip("Velocidad con la que la barra se desliza")]
    public float velocidadAnimacion = 3f;

    private string rutaArchivo;
    private float saldoObjetivo = 0f;
    private float dineroMaximo = 1f;

    void Start()
    {
        rutaArchivo = Application.persistentDataPath + "/datosBancarios.json";

        // Lee el JSON cada 0.5 segundos para mantener la barra sincronizada
        InvokeRepeating("LeerJSON", 0f, 0.5f);
    }

    void LeerJSON()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            DatosBancarios datos = JsonUtility.FromJson<DatosBancarios>(json);

            // 1. Calculamos el pico máximo histórico del saldo
            if (datos.historial != null && datos.historial.Count > 0)
            {
                float maxHistorico = 0f;
                float saldoSimulado = 0f;

                // Recorremos el historial desde el inicio para encontrar el punto más alto
                foreach (var transaccion in datos.historial)
                {
                    if (transaccion.esIngreso)
                        saldoSimulado += transaccion.monto;
                    else
                        saldoSimulado -= transaccion.monto;

                    // Si el saldo en ese momento superó el récord anterior, lo actualizamos
                    if (saldoSimulado > maxHistorico)
                    {
                        maxHistorico = saldoSimulado;
                    }
                }

                // Asignamos ese récord como nuestro nuevo tope
                dineroMaximo = maxHistorico;
            }

            // 2. Calcula el porcentaje basándose en ese pico histórico
            if (dineroMaximo > 0)
            {
                float porcentaje = (float)datos.saldoActual / dineroMaximo;
                saldoObjetivo = Mathf.Clamp01(porcentaje);
            }
        }
    }

    void Update()
    {
        if (barraIngresos != null)
        {
            // Mueve la barra de forma fluida hacia el nuevo porcentaje
            barraIngresos.fillAmount = Mathf.Lerp(barraIngresos.fillAmount, saldoObjetivo, Time.deltaTime * velocidadAnimacion);
        }
    }
}