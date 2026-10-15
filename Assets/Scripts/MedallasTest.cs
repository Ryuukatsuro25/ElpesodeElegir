using UnityEngine;

public class Medallastest : MonoBehaviour
{
    [Tooltip("Arrastra aquí la imagen (GameObject) que quieres mostrar/ocultar")]
    public GameObject imagenParaAlternar;

    // Este método se enlazará al botón
    public void AlternarEstadoImagen()
    {
        if (imagenParaAlternar != null)
        {
            // Cambia el estado activo al opuesto del actual
            bool estadoActual = imagenParaAlternar.activeSelf;
            imagenParaAlternar.SetActive(!estadoActual);
        }
        else
        {
            Debug.LogWarning("No has asignado ninguna imagen al script.");
        }
    }
}
