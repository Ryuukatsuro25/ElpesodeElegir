using UnityEngine;

public class AlternarObjetos : MonoBehaviour
{
    [Tooltip("Objeto que se va a apagar")]
    public GameObject objetoApagar;

    [Tooltip("Objeto que se va a prender")]
    public GameObject objetoPrender;

    // Asigna este método al OnClick() del botón
    public void Alternar()
    {
        if (objetoApagar != null) objetoApagar.SetActive(false);
        if (objetoPrender != null) objetoPrender.SetActive(true);
    }
}