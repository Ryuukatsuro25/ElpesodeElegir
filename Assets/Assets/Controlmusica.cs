
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlMusica : MonoBehaviour
{
    public AudioSource musica;
    public Button boton;
    public TMP_Text textoBoton;

    private bool musicaActiva = true;

    public void AlternarMusica()
    {
        musicaActiva = !musicaActiva;

        if (musicaActiva)
        {
            musica.UnPause();
            textoBoton.text = "Música: ON";
        }
        else
        {
            musica.Pause();
            textoBoton.text = "Música: OFF";
        }
    }
}