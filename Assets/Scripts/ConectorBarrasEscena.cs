using UnityEngine;
using UnityEngine.UI;

public class ConectorBarrasEscena : MonoBehaviour
{
    [Header("Barras de esta escena")]
    public Image barraBienestar;
    public Image barraDinero;

    private void Start()
    {
        Conectar();
    }

    private void OnEnable()
    {
        Conectar();
    }

    public void Conectar()
    {
        if (UIBarrasController.Instance != null)
        {
            UIBarrasController.Instance.AsignarReferenciasUI(barraBienestar, barraDinero);
        }
    }
}