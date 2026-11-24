# El Pe$o de Elegir

Simulador financiero de la vida universitaria para Android, desarrollado en Unity por **MediaWeb Studio** (Diseño Multimedia 2, Universidad Autónoma de Occidente, Cali).

El estudiante vive una semana universitaria, de domingo a viernes. Recibe una mesada aleatoria entre $80.000 y $120.000 y debe cubrir con ella alimentación, transporte, compras, vida académica y vida social, mientras intenta ahorrar $20.000. Cada decisión cambia dos indicadores que siempre están a la vista: **ingresos** y **bienestar mental**. Ninguna opción se presenta como "la correcta": el jugador vive las consecuencias de sus elecciones en un entorno seguro y aprende de forma práctica a ahorrar y a prepararse para los imprevistos.

**Público objetivo:** estudiantes de primer semestre de Ingeniería Multimedia de la UAO.

---

## Funcionalidades

| Épica | Qué incluye |
|---|---|
| **GM1 – Gamificación** | Barras de ingresos y bienestar, metas diarias, medallas con pantalla emergente, sala de medallas y mensajes del personaje. |
| **DC2 – Decisiones cotidianas** | Desayuno, transporte, almuerzo y compras, con su efecto en el saldo y en el bienestar. |
| **DF3 – Decisiones formales** | Situaciones académicas y sociales: salidas, grupos de trabajo, materiales, descanso y la tarea del viernes. |
| **EI4 – Eventos imprevistos** | Daño del computador, cumpleaños de un compañero y ayuda a la vecina. Se presenta un evento por visita, con cobro diferido si el jugador pide prestado. |
| **INT5 – Interacción táctil** | Banco simulado con saldo e historial, menú de pausa (texto rápido, música, guardar y salir) y confirmación de decisiones. |
| **Control de la experiencia** | Menú principal, tutorial, mesada del domingo, consejos del padre, resumen financiero y reflexión al cierre de la semana. |

## Tecnologías

- **Unity 2022.3.62f3** (LTS) con el módulo *Android Build Support*
- **C#** para la lógica del juego
- **TextMeshPro** y **uGUI** para la interfaz
- **JSON** con `JsonUtility` y **PlayerPrefs** para la persistencia local
- **Figma** para el prototipo de alta fidelidad, **Jira** para la gestión ágil y **GitHub** para el versionamiento

## Requisitos

| Para | Requisito |
|---|---|
| Jugar | Teléfono Android 8.0 o superior, 2 GB de RAM. No necesita conexión a internet. |
| Desarrollar | Unity Hub con Unity **2022.3.62f3** y *Android Build Support* (SDK, NDK y OpenJDK). |

## Estructura del proyecto

```
Assets/
├── Scenes/                    Menú, opciones, tutorial, medallas y escena de eventos (DI)
│   └── dias/                  DomingoTut, PrimeraDecisionDomingo, Lunes, Martes, Miercoles, jueves, viernes
├── Scripts/                   Lógica del juego
│   ├── GestorBancario.cs      Mesada, saldo e historial de movimientos
│   ├── AppBanco.cs            Interfaz del banco en el celular del personaje
│   ├── GestorBienestar.cs     Indicador de bienestar mental (0–100)
│   ├── GestorPaneles*.cs      Flujo de pantallas de cada día
│   ├── MedallasManager.cs     Desbloqueo y consulta de medallas
│   ├── GuardadoJuego.cs       Lectura y escritura segura de los JSON
│   └── EventosImprevistos/    Motor de eventos y su archivo de datos
│       ├── MotorEventosImprevistos.cs
│       ├── IrAEventoImprevisto.cs
│       └── EventosImprevistos.json
├── Assets/                    UI, botones, fondos, personaje, medallas y audio
├── prefab/                    Menú hamburguesa, misiones y tutorial
└── TextMesh Pro/
```

La arquitectura separa tres capas:
- **Presentación:** escenas, Canvas y prefabs.
- **Lógica:** los scripts en C#.
- **Datos:** el JSON de eventos, que es de solo lectura, y los archivos de partida en el dispositivo.

## Cómo ejecutar el proyecto

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Ryuukatsuro25/ElpesodeElegir.git
   ```
2. Abre Unity Hub → **Add** → selecciona la carpeta del proyecto. Ábrelo con **Unity 2022.3.62f3**.
3. Si Unity pide importar *TMP Essentials*, acepta.
4. Abre la escena del menú principal en `Assets/Scenes` y pulsa **Play**.
5. Para verlo como en el celular, usa la vista **Simulator** con un dispositivo vertical (por ejemplo, Samsung Galaxy Note10).

## Cómo generar el APK

1. Ve a **File → Build Settings** y elige **Android** → **Switch Platform**.
2. Verifica que estén en la lista, en orden, el menú, el tutorial, las escenas de `dias/` y la escena `DI`.
3. Pulsa **Build** y guarda el archivo `.apk`.
4. Pasa el APK al celular, permite instalar aplicaciones de origen desconocido y ábrelo.

El APK listo para instalar también está en la carpeta pública de la entrega, dentro de la subcarpeta `apk`.

## Datos guardados

La partida se guarda en `Application.persistentDataPath`. En Windows, dentro del editor, esa ruta es `C:\Users\<usuario>\AppData\LocalLow\DefaultCompany\ElPesode-Elegir\`.

| Archivo | Contenido |
|---|---|
| `datosBancarios.json` | Saldo, ingresos, gastos e historial de movimientos |
| `datosBienestar.json` | Bienestar mental (0–100) |
| `progresoEventos.json` | Eventos imprevistos ya jugados y cobro pendiente |
| PlayerPrefs | Medallas desbloqueadas y configuración (música y texto rápido) |

**Para empezar desde cero en las pruebas**, tienes dos opciones:
- en el Inspector, clic derecho sobre el componente `MotorEventosImprevistos` → **Pruebas → Empezar partida nueva**;
- o borra los tres archivos JSON de esa carpeta.

## Ramas

| Rama | Uso |
|---|---|
| `main` | Versión estable entregada |
| `pre-main` | Integración del trabajo del equipo al final de cada sprint |
| Una rama por integrante | Desarrollo de las historias de usuario asignadas |

## Equipo – MediaWeb Studio

| Integrante | Rol |
|---|---|
| Salomón David Ramírez Marcus | Desarrollador UX/UI – decisiones cotidianas y menú de pausa |
| Juan José Osorio Reyes | Desarrollador de sistemas de juego – economía, banco y eventos imprevistos |
| Juan Esteban Galindo Mina | Desarrollador de narrativa y decisiones formales |
| Juan David Ríos Gallego | Desarrollador de flujo de la experiencia y persistencia |
| Juan Pablo Rueda Sánchez | Desarrollador de gamificación y recompensas |

**Docente:** Nicolás Llanos Neuta · Diseño Multimedia 2 · Universidad Autónoma de Occidente, 2026.

## Enlaces

- Prototipo de alta fidelidad (Figma): https://www.figma.com/design/UeaoSOlw0VqWMHxMn7JzZM/AltaFidelidad?node-id=0-1&p=f
- Demo, APK y videos (Google Drive): https://drive.google.com/drive/folders/1EmSn4wwMQVeah_EaAhenx4eNfgI2wmGP?usp=sharing
