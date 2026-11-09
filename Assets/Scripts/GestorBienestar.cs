using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =====================================================================
//  BIENESTAR MENTAL COMPARTIDO ENTRE ESCENAS
//
//  - Guarda en "datosBienestar.json", el mismo archivo y formato que usa
//    PopUpManager, así el valor se conserva al cambiar de escena (GM1.1.2).
//  - Cambia con valores EXACTOS (+100, +50, -50...), no aleatorios.
//  - La barra sube o baja con animación y muestra un texto flotante
//    como "+100 SANIDAD" o "-50 SANIDAD" (INT5.2.2).
//
//  Ponlo una sola vez por escena, en un objeto siempre activo.
//  Otros scripts lo usan con:  GestorBienestar.Aplicar(-50);
// =====================================================================
public class GestorBienestar : MonoBehaviour
{
    [System.Serializable]
    public class DatosBienestarJson
    {
        public float bienestarActual;
    }

    [Header("Barra de bienestar (asigna UNA de las dos)")]
    [Tooltip("Slider de la barra. Desmarca 'Interactable' en el Slider")]
    public Slider barraSlider;
    [Tooltip("Image con Image Type = Filled")]
    public Image barraImagen;

    [Header("Texto flotante de variación (opcional)")]
    public TextMeshProUGUI textoVariacion;
    public string sufijo = " SANIDAD";
    public float duracionTexto = 2f;
    public float desplazamientoTexto = 40f;

    [Header("Configuración")]
    public float bienestarInicial = 50f;
    public float bienestarMaximo = 100f;
    public float duracionAnimacion = 1.2f;

    // Acceso desde otros scripts
    public static GestorBienestar Instancia { get; private set; }
    public float Actual => bienestar;

    private const string NOMBRE_ARCHIVO = "datosBienestar.json";
    private float bienestar;
    private Coroutine animacionBarra;
    private Coroutine animacionTexto;
    private Vector2 posicionTextoInicial;

    // ---------------- CICLO DE UNITY ----------------

    void Awake()
    {
        Instancia = this;
        bienestar = LeerArchivo(bienestarInicial, bienestarMaximo);

        if (textoVariacion != null)
        {
            posicionTextoInicial = textoVariacion.rectTransform.anchoredPosition;
            textoVariacion.gameObject.SetActive(false);
        }

        PintarBarra(bienestar / bienestarMaximo);
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    // ---------------- API ----------------

    // Úsalo desde cualquier script: GestorBienestar.Aplicar(-50);
    // Si en la escena no hay barra, igual guarda el cambio en el JSON.
    public static void Aplicar(float cantidad)
    {
        if (Instancia != null)
        {
            Instancia.Cambiar(cantidad);
            return;
        }

        float valor = LeerArchivo(50f, 100f);
        valor = Mathf.Clamp(valor + cantidad, 0f, 100f);
        GuardarArchivo(valor);
    }

    // También se puede llamar desde el On Click de un botón
    public void Cambiar(float cantidad)
    {
        float anterior = bienestar;
        bienestar = Mathf.Clamp(bienestar + cantidad, 0f, bienestarMaximo);
        GuardarArchivo(bienestar);

        if (animacionBarra != null) StopCoroutine(animacionBarra);
        animacionBarra = StartCoroutine(AnimarBarra(anterior / bienestarMaximo, bienestar / bienestarMaximo));

        if (textoVariacion != null && cantidad != 0f)
        {
            if (animacionTexto != null) StopCoroutine(animacionTexto);
            animacionTexto = StartCoroutine(MostrarVariacion(cantidad));
        }
    }

    // Para pruebas: deja el bienestar en el valor inicial
    public void Reiniciar()
    {
        bienestar = bienestarInicial;
        GuardarArchivo(bienestar);
        PintarBarra(bienestar / bienestarMaximo);
    }

    // ---------------- ANIMACIONES ----------------

    private IEnumerator AnimarBarra(float desde, float hasta)
    {
        float tiempo = 0f;
        while (tiempo < duracionAnimacion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(tiempo / duracionAnimacion));
            PintarBarra(Mathf.Lerp(desde, hasta, t));
            yield return null;
        }
        PintarBarra(hasta);
        animacionBarra = null;
    }

    private IEnumerator MostrarVariacion(float cantidad)
    {
        RectTransform rt = textoVariacion.rectTransform;
        Vector2 destino = posicionTextoInicial + Vector2.up * desplazamientoTexto;

        textoVariacion.text = (cantidad > 0 ? "+" : "") + cantidad.ToString("0") + sufijo;
        textoVariacion.color = cantidad > 0 ? new Color(0.2f, 0.75f, 0.25f) : new Color(0.9f, 0.2f, 0.2f);
        textoVariacion.gameObject.SetActive(true);

        float tiempo = 0f;
        while (tiempo < duracionTexto)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionTexto);
            rt.anchoredPosition = Vector2.Lerp(posicionTextoInicial, destino, t);
            textoVariacion.alpha = 1f - t;
            yield return null;
        }

        rt.anchoredPosition = posicionTextoInicial;
        textoVariacion.alpha = 1f;
        textoVariacion.gameObject.SetActive(false);
        animacionTexto = null;
    }

    private void PintarBarra(float porcentaje)
    {
        porcentaje = Mathf.Clamp01(porcentaje);
        if (barraSlider != null)
            barraSlider.value = Mathf.Lerp(barraSlider.minValue, barraSlider.maxValue, porcentaje);
        if (barraImagen != null)
            barraImagen.fillAmount = porcentaje;
    }

    // ---------------- JSON ----------------

    private static string Ruta()
    {
        return Path.Combine(Application.persistentDataPath, NOMBRE_ARCHIVO);
    }

    private static float LeerArchivo(float valorInicial, float maximo)
    {
        string ruta = Ruta();
        if (File.Exists(ruta))
        {
            DatosBienestarJson datos = JsonUtility.FromJson<DatosBienestarJson>(File.ReadAllText(ruta));
            if (datos != null) return Mathf.Clamp(datos.bienestarActual, 0f, maximo);
        }

        GuardarArchivo(valorInicial);
        return valorInicial;
    }

    private static void GuardarArchivo(float valor)
    {
        DatosBienestarJson datos = new DatosBienestarJson { bienestarActual = valor };
        File.WriteAllText(Ruta(), JsonUtility.ToJson(datos, true));
    }
}
