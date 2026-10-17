using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Referenzen")]
    [Tooltip("Optional: Solange leer, bleibt der Ball an seiner Startposition.")]
    [SerializeField] private Transform paddle;

    [Header("Bewegung")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float minZShare = 0.3f;
    [SerializeField] private float minXShare = 0.2f;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 offsetToPaddle;
    private bool launched;
    private Vector3 lastVelocity;

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
        lastVelocity = Vector3.zero;
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

        if (Mathf.Abs(dir.x) < minXShare)
        {
            float sign = dir.x >= 0f ? 1f : -1f;
            dir.x = minXShare * sign;
            dir = dir.normalized;
        }

        rb.linearVelocity = dir * speed;
        lastVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!launched) return;

        Vector3 normal = collision.GetContact(0).normal;
        normal.y = 0f;
        if (normal.sqrMagnitude < 0.0001f) return;
        normal.Normalize();

        Vector3 reflected = Vector3.Reflect(lastVelocity, normal);
        reflected.y = 0f;

        rb.linearVelocity = reflected.normalized * speed;
    }
}