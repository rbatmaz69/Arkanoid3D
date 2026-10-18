using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    public static PaddleController Instance { get; private set; }

    [Header("Bewegung")]
    [SerializeField] private float keyboardSpeed = 12f;

    [Header("Grenzen")]
    [SerializeField] private float minX = -7.4f;
    [SerializeField] private float maxX = 7.4f;

    [Header("Form (Kapsel entlang X)")]
    [Tooltip("Halbe Länge der Strecke zwischen den Kappenmittelpunkten (entspricht Kappe_Links/Rechts)")]
    [SerializeField] private float halfLength = 1.2f;
    [SerializeField] private float capsuleRadius = 0.34f;

    private float currentHalfLength;
    private float currentMinX;
    private float currentMaxX;

    public float CapsuleRadius => capsuleRadius;

    private void Awake()
    {
        Instance = this;
        currentHalfLength = halfLength;
        currentMinX = minX;
        currentMaxX = maxX;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetWidth(float factor)
    {
        // Grenzen enger, wenn der Schläger breiter wird
        float halfExtra = 1.54f * (factor - 1f);
        currentMinX = minX + halfExtra;
        currentMaxX = maxX - halfExtra;

        // Kapsel mitführen
        currentHalfLength = halfLength * factor;
    }

    // Endpunkte der Kapselstrecke in Weltkoordinaten (x = X, y = Z)
    public void GetSegment(out Vector2 a, out Vector2 b)
    {
        Vector3 p = transform.position;
        a = new Vector2(p.x - currentHalfLength, p.z);
        b = new Vector2(p.x + currentHalfLength, p.z);
    }

    private void Update()
    {
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) input -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) input += 1f;
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + input * keyboardSpeed * Time.deltaTime, currentMinX, currentMaxX);
        transform.position = pos;
    }
}
