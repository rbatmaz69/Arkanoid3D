
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PaddleController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Bewegung")]
    [SerializeField] private float keyboardSpeed = 12f;

    [Header("Grenzen")]
    [SerializeField] private float minX = -6.5f;
    [SerializeField] private float maxX = 6.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) input -=1f;
            if (Keyboard.current.rightArrowKey.isPressed) input += 1f;
        }
        float newX = rb.position.x + input * keyboardSpeed * Time.fixedDeltaTime;
        newX = Mathf.Clamp(newX, minX, maxX);

        rb.MovePosition(new Vector3(newX, rb.position.y, rb.position.z));
    }
}
