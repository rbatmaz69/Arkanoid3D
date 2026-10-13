using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.WSA;

public class BallController : MonoBehaviour
{
    [Header("Referenzen")]
    [Tooltip("Optional: Solange leer, bleibt der Ball an seiner Startposition.")]
    [SerializeField] private Transform paddle;

    [Header("Bewegung")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float minZShare = 0.3f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 offsetToPaddle;
    private bool launched;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        startPosition = transform.position;

        if (paddle != null)
        {
            offsetToPaddle = transform.position - paddle.position;
        }

        rb.isKinematic = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (launched) return;

        if (paddle != null)
        {
            transform.position = paddle.position + offsetToPaddle;
        }
        else
        {
            transform.position = startPosition;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Launch();
        }
    }
    private void Launch()
    {
        launched = true;
        rb.isKinematic = false;

        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        rb.linearVelocity = dir * speed;
    }

    public void ResetBall()
    {
        launched = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
    private void FixedUpdate()
    {
        if (!launched) return;

        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        if (v.sqrMagnitude < 0.0001f) return;

        Vector3 dir = v.normalized;

        if (Mathf.Abs(dir.z) < minZShare)
        {
            dir.z = minZShare * (dir.z >= 0f ? 1f : -1f);
            dir = dir.normalized;
        }

        rb.linearVelocity = dir * speed;
    }
}
