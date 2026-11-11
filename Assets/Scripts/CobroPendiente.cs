using UnityEngine;

public class CobroPendiente : MonoBehaviour
{
    [Header("Conexiones")]
    [Tooltip("Arrastra aquí el objeto que tiene tu script GestorBancario")]
    public GestorBancario banco;

    [Header("Deuda")]
    public string claveDeuda = "deuda_computador";
    public int monto = 40000;
    public string descripcion = "Pago a papá: arreglo del computador + intereses";
    public float bienestar = -50f;

    [Header("Pantallas")]
    public GameObject pantallaActual;
    public GameObject pantallaCobro;
    public GameObject pantallaSinDeuda;

    public void Revisar()
    {
        if (pantallaActual != null) pantallaActual.SetActive(false);

        if (HayDeuda())
        {
            PlayerPrefs.DeleteKey(claveDeuda);
            PlayerPrefs.Save();

            // Reemplazamos la función inexistente por tu GestorBancario real
            if (banco != null)
            {
                // Pasamos el monto, la descripción, y 'false' porque es un egreso
                banco.AgregarTransaccion(monto, descripcion, false);
            }

            // Mantenemos esta asumiendo que GestorBienestar sí tiene el método Aplicar()
            GestorBienestar.Aplicar(bienestar);

            if (pantallaCobro != null) pantallaCobro.SetActive(true);
        }
        else if (pantallaSinDeuda != null)
        {
            pantallaSinDeuda.SetActive(true);
        }
    }

    public bool HayDeuda()
    {
        return PlayerPrefs.GetInt(claveDeuda, 0) == 1;
    }

    [ContextMenu("PRUEBA - Borrar deuda")]
    public void BorrarDeuda()
    {
        PlayerPrefs.DeleteKey(claveDeuda);
        PlayerPrefs.Save();
    }
}