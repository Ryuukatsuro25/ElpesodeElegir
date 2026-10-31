
using UnityEngine;
using UnityEngine.UI;

public class NotificacionMedalla : MonoBehaviour
{
    [System.Serializable]
    public class Notificacion
    {
        public string idMedalla;
        public Sprite imagenNotificacion;
    }

    [Header("Panel de recompensa")]
    public GameObject panelRecompensa;

    [Header("Imagen donde se muestra la notificacion")]
    public Image imagenRecompensa;

    [Header("Notificaciones disponibles")]
    public Notificacion[] notificaciones;

    private void Awake()
    {
        if (panelRecompensa != null)
        {
            panelRecompensa.SetActive(false);
        }
    }

    public void MostrarNotificacion(string idMedalla)
    {
        if (panelRecompensa == null || imagenRecompensa == null)
        {
            Debug.LogWarning(
                "Falta asignar el panel o la imagen de la notificacion."
            );
            return;
        }

        foreach (Notificacion notificacion in notificaciones)
        {
            if (notificacion == null ||
                notificacion.imagenNotificacion == null)
            {
                continue;
            }

            if (notificacion.idMedalla == idMedalla)
            {
                imagenRecompensa.sprite =
                    notificacion.imagenNotificacion;

                panelRecompensa.SetActive(true);

                Debug.Log("Notificacion mostrada: " + idMedalla);
                return;
            }
        }

        Debug.LogWarning(
            "No se encontro una notificacion para la medalla: " + idMedalla
        );
    }

    public void CerrarNotificacion()
    {
        if (panelRecompensa != null)
        {
            panelRecompensa.SetActive(false);
        }
    }

    // PRUEBA TEMPORAL DESDE EL INSPECTOR
    [ContextMenu("PRUEBA - Mostrar Chequeando el dinero")]
    private void PruebaMostrarNotificacion()
    {
        MostrarNotificacion("chequeando_dinero");
    }
}
