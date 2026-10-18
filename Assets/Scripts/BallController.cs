using System.Collections.Generic;
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

    private Vector3 velocity;
    private float radius;
    private Vector3 startPosition;
    private Vector3 offsetToPaddle;
    private bool launched;
    private bool removed;

    private static readonly List<BoxBody> hitBuffer = new List<BoxBody>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ExtraCount = 0;
        MainLost = false;
        SpeedFactor = 1f;
    }

    private void Awake()
    {
        // Kugel mit Radius 0.5 im lokalen Raum, wie vorher der SphereCollider
        radius = 0.5f * transform.lossyScale.x;
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

        Vector3 dir = new Vector3(Random.Range(-0.5f, 0.5f), 0f, 1f).normalized;
        velocity = dir * speed * SpeedFactor;
    }

    // Zusatzball: fliegt sofort los, klebt nie am Schläger
    public void StartAsExtra(Vector3 direction)
    {
        IsExtra = true;
        ExtraCount++;
        paddle = null;
        launched = true;
        velocity = direction.normalized * speed * SpeedFactor;
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
        velocity = Vector3.zero;
    }

    // LateUpdate: der Schläger hat sich in Update schon bewegt
    private void LateUpdate()
    {
        if (!launched) return;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // Teilschritte, damit der Ball auch bei langen Frames nicht durch Wände tunnelt
        float distance = velocity.magnitude * dt;
        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / (radius * 0.5f)));
        float stepDt = dt / steps;

        for (int i = 0; i < steps; i++)
        {
            transform.position += velocity * stepDt;

            ResolveCollisions();
            NormalizeVelocity();

            BodensensorTrigger sensor = BodensensorTrigger.Instance;
            if (sensor != null) sensor.Check(this, new Vector2(transform.position.x, transform.position.z), radius);

            // Ball wurde entfernt, deaktiviert oder zurückgesetzt
            if (removed || !launched || !gameObject.activeSelf) return;
        }
    }

    // Konstante Geschwindigkeit, Y bleibt 0, Richtung nie zu flach oder zu steil
    private void NormalizeVelocity()
    {
        Vector3 v = velocity;
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

        velocity = dir * speed * SpeedFactor;
    }

    private void ResolveCollisions()
    {
        Vector3 pos = transform.position;
        Vector2 p = new Vector2(pos.x, pos.z);

        // Wände und Blöcke (Kopie, weil Treffer BoxBodys deaktivieren)
        hitBuffer.Clear();
        hitBuffer.AddRange(CollisionWorld.Solids);

        foreach (BoxBody body in hitBuffer)
        {
            if (!body.isActiveAndEnabled) continue;

            body.GetRect(out Vector2 min, out Vector2 max);
            if (!CollisionWorld.CircleVsRect(p, radius, min, max, out Vector2 normal, out float penetration)) continue;

            p += normal * penetration;
            Bounce(normal);

            IBallHitReceiver receiver = body.GetComponent<IBallHitReceiver>();
            if (receiver != null) receiver.OnBallHit(this);
        }

        // Schläger
        PaddleController pad = PaddleController.Instance;
        if (pad != null)
        {
            pad.GetSegment(out Vector2 a, out Vector2 b);
            if (CollisionWorld.CircleVsCapsule(p, radius, a, b, pad.CapsuleRadius, out Vector2 normal, out float penetration))
            {
                p += normal * penetration;
                Bounce(normal);
            }
        }

        transform.position = new Vector3(p.x, pos.y, p.y);
    }

    // Nur reflektieren, wenn der Ball in die Fläche hineinfliegt (verhindert Doppelreflexion an Ecken)
    private void Bounce(Vector2 normal)
    {
        Vector3 n = new Vector3(normal.x, 0f, normal.y);
        if (Vector3.Dot(velocity, n) >= 0f) return;

        Vector3 reflected = Vector3.Reflect(velocity, n);
        reflected.y = 0f;
        velocity = reflected.normalized * speed * SpeedFactor;
    }
}
