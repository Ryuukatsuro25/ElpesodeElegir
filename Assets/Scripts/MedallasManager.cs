
using UnityEngine;

public class MedallasManager : MonoBehaviour
{
    [System.Serializable]
    public class Medalla
    {
        public string id;
        public GameObject imagenBlancoYNegro;
    }

    [Header("Medallas de la sala")]
    public Medalla[] medallas;

    [Header("Notificaciones de recompensa")]
    public NotificacionMedalla notificacionMedalla;

    private void Start()
    {
        ActualizarSala();
    }

    public void DesbloquearMedalla(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("El ID de la medalla está vacío.");
            return;
        }

        bool yaEstabaDesbloqueada = EstaDesbloqueada(id);

        PlayerPrefs.SetInt("Medalla_" + id, 1);
        PlayerPrefs.Save();

        ActualizarSala();

        Debug.Log("Medalla desbloqueada: " + id);

        // Mostrar la notificación solamente la primera vez
        // que se desbloquea esta medalla.
        if (!yaEstabaDesbloqueada)
        {
            if (notificacionMedalla != null)
            {
                notificacionMedalla.MostrarNotificacion(id);
            }
            else
            {
                Debug.LogWarning(
                    "No hay un componente NotificacionMedalla asignado."
                );
            }
        }
    }

    public bool EstaDesbloqueada(string id)
    {
        return PlayerPrefs.GetInt("Medalla_" + id, 0) == 1;
    }

    public void ActualizarSala()
    {
        if (medallas == null)
        {
            Debug.LogWarning(
                "No se ha configurado el arreglo de medallas."
            );
            return;
        }

        foreach (Medalla medalla in medallas)
        {
            if (medalla == null ||
                medalla.imagenBlancoYNegro == null ||
                string.IsNullOrEmpty(medalla.id))
            {
                continue;
            }

            bool desbloqueada = EstaDesbloqueada(medalla.id);

            // Ocultar la imagen en blanco y negro
            // cuando la medalla esté desbloqueada.
            medalla.imagenBlancoYNegro.SetActive(!desbloqueada);
        }
    }

    // PRUEBA TEMPORAL DESDE EL INSPECTOR
    [ContextMenu("PRUEBA - Desbloquear Companerismo")]
    private void PruebaDesbloquearCompanerismo()
    {
        DesbloquearMedalla("companerismo");
    }
}
