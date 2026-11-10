using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonBancoPermanente : MonoBehaviour
{
    private void Awake()
    {
        // Opcional: Si quieres que este objeto persista entre escenas (si cambias de escena)
        // DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        // Este método se asegura de que el botón siempre se dibuje al final 
        // de la jerarquía de su contenedor, manteniéndose siempre al frente 
        // y evitando que los nuevos paneles que se activen lo tapen.
        transform.SetAsLastSibling();
    }
}