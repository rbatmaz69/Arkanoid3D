using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [Header("Bewegung")]
    [SerializeField] private float keyboardSpeed = 12f;

    [Header("Grenzen")]
    [SerializeField] private float minX = -7.4f;
    [SerializeField] private float maxX = 7.4f;

    private Rigidbody rb;
    private float currentMinX;
    private float currentMaxX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentMinX = minX;
        currentMaxX = maxX;
    }

    public void SetWidth(float factor)
    {
        // Grenzen enger, wenn der Schläger breiter wird
        float halfExtra = 1.54f * (factor - 1f);
        currentMinX = minX + halfExtra;
        currentMaxX = maxX - halfExtra;

        // Collider mitführen (Capsule liegt auf der X-Achse)
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        col.height = 3.08f * factor;
    }

    private void FixedUpdate()
    {
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) input -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) input += 1f;
        }

        float newX = rb.position.x + input * keyboardSpeed * Time.fixedDeltaTime;
        newX = Mathf.Clamp(newX, currentMinX, currentMaxX);

        rb.MovePosition(new Vector3(newX, rb.position.y, rb.position.z));
    }
}