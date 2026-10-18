using UnityEngine;

[RequireComponent(typeof(BoxBody))]
public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private PowerUp effectPrefab;
    [SerializeField] private float spinSpeed = 120f;

    private BoxBody box;
    private bool collected;

    private void Awake()
    {
        box = GetComponent<BoxBody>();
        box.Solid = false; // Zone, kein Hindernis: der Ball soll hindurchfliegen
    }

    private void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime, 0f, 0f, Space.World);

        if (collected) return;

        PaddleController paddle = PaddleController.Instance;
        if (paddle == null) return;

        if (TouchesPaddle(paddle)) Collect();
    }

    // Schläger-Kapsel gegen Power-up-Rechteck: Punkt der Strecke, der dem Rechteck am nächsten liegt
    private bool TouchesPaddle(PaddleController paddle)
    {
        box.GetRect(out Vector2 min, out Vector2 max);
        paddle.GetSegment(out Vector2 a, out Vector2 b);

        Vector2 rectCenter = (min + max) * 0.5f;
        float t = Mathf.Clamp(rectCenter.x, a.x, b.x);
        Vector2 onSegment = new Vector2(t, a.y);

        return CollisionWorld.CircleVsRect(onSegment, paddle.CapsuleRadius, min, max, out _, out _);
    }

    private void Collect()
    {
        collected = true;

        if (effectPrefab != null)
        {
            PowerUp effect = Instantiate(effectPrefab);
            effect.Begin();
        }

        Destroy(gameObject);
    }
}
