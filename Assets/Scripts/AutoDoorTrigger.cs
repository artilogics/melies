using UnityEngine;

public class DoorTriggerOpen : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Transform del pivot que rota la puerta (ej. Pivot1).")]
    public Transform doorPivot;

    [Header("Sonido")]
    [Tooltip("Clip de sonido que se reproducir� al abrir la puerta.")]
    public AudioClip soundOpen;
    [Tooltip("Volumen del sonido de apertura (0 = silencio, 1 = m�ximo).")]
    [Range(0f, 1f)]
    public float soundVolume = 1f;

    [Header("Configuraci�n")]
    [Tooltip("�ngulo en grados que girar� la puerta (usar negativo para abrir al otro lado).")]
    public float openAngle = 90f;
    [Tooltip("Velocidad de apertura (m�s alto = m�s r�pido).")]
    public float openSpeed = 3f;
    [Tooltip("Etiqueta del objeto que activa la puerta.")]
    public string activatorTag = "Player";

    private Quaternion closedRotation;
    private Quaternion targetRotation;
    private bool isOpening = false;

    void Start()
    {
        if (doorPivot == null)
        {
            Debug.LogError("DoorTriggerOpen: Falta asignar el doorPivot.", this);
            enabled = false;
            return;
        }

        // Guardamos la rotaci�n inicial (cerrada)
        closedRotation = doorPivot.localRotation;

        // Calculamos hacia d�nde abrir� la puerta
        Vector3 euler = doorPivot.localEulerAngles;
        euler.y += openAngle;
        targetRotation = Quaternion.Euler(euler);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(activatorTag))
        {
            if (!isOpening)
                StartCoroutine(OpenDoor());
        }
    }

    private System.Collections.IEnumerator OpenDoor()
    {
        isOpening = true;

        if (soundOpen != null)
            AudioSource.PlayClipAtPoint(soundOpen, transform.position, soundVolume);

        while (Quaternion.Angle(doorPivot.localRotation, targetRotation) > 0.1f)
        {
            doorPivot.localRotation = Quaternion.Slerp(
                doorPivot.localRotation,
                targetRotation,
                Time.deltaTime * openSpeed
            );
            yield return null;
        }

        doorPivot.localRotation = targetRotation;
    }
}
