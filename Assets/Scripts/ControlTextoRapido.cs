
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlTextoRapido : MonoBehaviour
{
    public Button boton;
    public TMP_Text textoBoton;

    private bool textoRapido;

    private void Start()
    {
        textoRapido = PlayerPrefs.GetInt("TextoRapido", 0) == 1;
        ActualizarBoton();

        boton.onClick.AddListener(AlternarTextoRapido);
    }

    public void AlternarTextoRapido()
    {
        textoRapido = !textoRapido;

        PlayerPrefs.SetInt("TextoRapido", textoRapido ? 1 : 0);
        PlayerPrefs.Save();

        ActualizarBoton();
    }

    private void ActualizarBoton()
    {
        textoBoton.text = textoRapido ? "ON" : "OFF";
    }
}
