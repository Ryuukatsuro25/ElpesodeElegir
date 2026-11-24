
using UnityEngine;

[DisallowMultipleComponent]
public class SonidoClickUI : MonoBehaviour
{
    private static SonidoClickUI instancia;
    private AudioSource audioSource;

    private void Awake()
    {
        // Evitar que existan gestores duplicados
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;

        // Mantener el sonido al cambiar de escena
        DontDestroyOnLoad(gameObject);

        // Obtener el componente de audio
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    // Reproducir sonido al presionar un boton
    public void ReproducirClick()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        else
        {
            Debug.LogWarning(
                "SonidoClickUI: no se ha asignado el sonido click."
            );
        }
    }
}
