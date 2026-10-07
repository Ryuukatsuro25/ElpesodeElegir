using UnityEngine;

public class ControladorBotonesBancarios : MonoBehaviour
{
    [Header("Referencia al Gestor")]
    public GestorBancario referenciaGestor;

    void Start()
    {
        if (referenciaGestor == null)
        {
            referenciaGestor = FindObjectOfType<GestorBancario>();
        }
    }

    public void SumarCincoMil()
    {
        if (referenciaGestor != null)
        {
            referenciaGestor.AgregarTransaccion(5000, "Ingreso extra", true);
        }
        else
        {
            Debug.LogError("No se encontró el Gestor Bancario en la escena.");
        }
    }

    public void RestarCincoMil()
    {
        if (referenciaGestor != null)
        {
            referenciaGestor.AgregarTransaccion(5000, "Gasto extra", false);
        }
        else
        {
            Debug.LogError("No se encontró el Gestor Bancario en la escena.");
        }
    }
}