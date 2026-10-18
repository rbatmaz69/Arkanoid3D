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

    public static int ExtraCount;
    public static bool MainLost;
    public static float SpeedFactor = 1f;
    public bool IsExtra { get; private set; }

    private Rigidbody rb;
    private Vector3 startPosition;
    private Vector3 offsetToPaddle;
    private bool launched;
    private Vector3 lastVelocity;
    private bool removed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ExtraCount = 0;
        MainLost = false;
        SpeedFactor = 1f;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = transform.position;

        if (!IsExtra)
        {
            // Hauptball: Zähler sauber starten (z. B. nach Neustart)
            ExtraCount = 0;
            MainLost = false;
            SpeedFactor = 1f;
            rb.isKinematic = true;
        }

        if (paddle != null)
        {
            offsetToPaddle = transform.position - paddle.position;
        }
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
        rb.linearVelocity = dir * speed * SpeedFactor;
    }

    // Zusatzball: fliegt sofort los, klebt nie am Schläger
    public void StartAsExtra(Vector3 direction)
    {
        IsExtra = true;
        ExtraCount++;
        paddle = null;
        launched = true;
        rb.isKinematic = false;
        rb.linearVelocity = direction.normalized * speed * SpeedFactor;
        lastVelocity = rb.linearVelocity;
    }

    // Zusatzball sauber entfernen
    public void Remove()
    {
        if (removed) return;
        removed = true;
        if (IsExtra) ExtraCount--;
        Destroy(gameObject);
    }

    public void ResetBall()
    {
        // Alle Zusatzbälle entfernen
        BallController[] all = FindObjectsByType<BallController>(FindObjectsSortMode.None);
        foreach (BallController b in all)
        {
            if (b != this && b.IsExtra) Destroy(b.gameObject);
        }
        ExtraCount = 0;
        MainLost = false;

        gameObject.SetActive(true);
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

        rb.linearVelocity = dir * speed * SpeedFactor;
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

        rb.linearVelocity = reflected.normalized * speed * SpeedFactor;
    }
}