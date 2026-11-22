using UnityEngine;
using UnityEngine.SceneManagement;

namespace PesoDeElegir.EventosImprevistos
{
    /// <summary>
    /// Lleva al jugador a la escena de eventos imprevistos (DI) y recuerda a qué escena
    /// debe volver cuando termine el evento.
    ///
    /// Uso: ponlo en cualquier objeto de la escena del día. En el On Click del botón que hoy
    /// carga el día siguiente, elige IrAEventoImprevisto → Ir (string) y escribe el día
    /// siguiente, por ejemplo "Martes". El jugador verá UN evento y luego pasará a Martes.
    /// </summary>
    public class IrAEventoImprevisto : MonoBehaviour
    {
        private const string Clave = "EI_EscenaSiguiente";

        [Tooltip("Nombre exacto de la escena de eventos (debe estar en File > Build Settings)")]
        [SerializeField] private string escenaEventos = "DI";

        /// <param name="escenaSiguiente">Escena a la que se vuelve después del evento</param>
        public void Ir(string escenaSiguiente)
        {
            PlayerPrefs.SetString(Clave, escenaSiguiente ?? string.Empty);
            PlayerPrefs.Save();

            if (!Application.CanStreamedLevelBeLoaded(escenaEventos))
            {
                Debug.LogError($"IrAEventoImprevisto: la escena '{escenaEventos}' no está en File > Build Settings.", this);
                return;
            }
            SceneManager.LoadScene(escenaEventos);
        }

        /// <summary>La lee el motor al terminar; se borra para que no se use dos veces.</summary>
        public static string TomarEscenaSiguiente()
        {
            string escena = PlayerPrefs.GetString(Clave, string.Empty);
            PlayerPrefs.DeleteKey(Clave);
            return escena;
        }
    }
}