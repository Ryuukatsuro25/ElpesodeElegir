using UnityEngine;

public class TutorialCelular : MonoBehaviour
{
    // Esta estructura agrupa el panel y la flecha de cada paso
    [System.Serializable]
    public struct PasoTutorial
    {
        public GameObject panelTexto;
        public GameObject flecha;
    }

    [Header("Lista de pasos del tutorial")]
    public PasoTutorial[] pasos;

    [Header("Objeto extra a apagar al final (ej. Celular entero o fondo)")]
    public GameObject objetoFinal;

    private int indiceActual = 0;

    void Start()
    {
        // Al iniciar, apagamos todos los pasos excepto el primero
        if (pasos.Length > 0)
        {
            for (int i = 0; i < pasos.Length; i++)
            {
                bool esPrimerPaso = (i == 0);

                if (pasos[i].panelTexto != null)
                    pasos[i].panelTexto.SetActive(esPrimerPaso);

                if (pasos[i].flecha != null)
                    pasos[i].flecha.SetActive(esPrimerPaso);
            }
        }
    }

    public void SiguientePaso()
    {
        // Si hay un paso siguiente disponible
        if (indiceActual < pasos.Length - 1)
        {
            // 1. Apagamos el panel y flecha actuales
            if (pasos[indiceActual].panelTexto != null)
                pasos[indiceActual].panelTexto.SetActive(false);

            if (pasos[indiceActual].flecha != null)
                pasos[indiceActual].flecha.SetActive(false);

            // 2. Avanzamos el índice
            indiceActual++;

            // 3. Prendemos el panel y flecha del nuevo paso
            if (pasos[indiceActual].panelTexto != null)
                pasos[indiceActual].panelTexto.SetActive(true);

            if (pasos[indiceActual].flecha != null)
                pasos[indiceActual].flecha.SetActive(true);
        }
        // Si estamos en el último paso y le damos a "Siguiente"
        else if (indiceActual == pasos.Length - 1)
        {
            // Apagamos el último panel y su flecha
            if (pasos[indiceActual].panelTexto != null)
                pasos[indiceActual].panelTexto.SetActive(false);

            if (pasos[indiceActual].flecha != null)
                pasos[indiceActual].flecha.SetActive(false);

            // Apagamos el objeto extra (como el celular de fondo o una imagen oscura)
            if (objetoFinal != null)
            {
                objetoFinal.SetActive(false);
            }
        }
    }
}