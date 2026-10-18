using UnityEngine;

public class WidePaddlePowerUp : PowerUp
{
    private static WidePaddlePowerUp active;

    private PaddleController paddle;
    private Transform body;
    private Transform capLeft;
    private Transform capRight;
    private float factor = 1f;
    private float target = 1f;

    protected override void OnBegin()
    {
        Debug.Log("Breit gestartet");
        // Erneutes Einsammeln: alten Effekt entfernen, nicht stapeln
        if (active != null && active != this)
        {
            float old = active.factor;
            active.ForceStop();
            factor = old;
        }
        active = this;

        paddle = FindFirstObjectByType<PaddleController>();
        body = paddle.transform.Find("Körper");
        capLeft = paddle.transform.Find("Kappe_Links");
        capRight = paddle.transform.Find("Kappe_Rechts");
        target = 1.5f;
    }

    private void LateUpdate()
    {
        if (paddle == null) return;

        // Kurz vor Ablauf zurück auf Normalmaß
        if (Remaining < 0.4f) target = 1f;

        factor = Mathf.MoveTowards(factor, target, 2f * Time.deltaTime);
        Apply(factor);
    }

    private void Apply(float f)
    {
        body.localScale = new Vector3(0.6f, 1.2f * f, 0.6f);
        capLeft.localPosition = new Vector3(-1.2f * f, 0f, 0f);
        capRight.localPosition = new Vector3(1.2f * f, 0f, 0f);
        paddle.SetWidth(f);
    }

    private void ForceStop()
    {
        target = 1f;
        paddle = null; // diese Instanz steuert nichts mehr
        Destroy(gameObject);
    }

    protected override void OnEnd()
    {
        if (paddle != null)
        {
            Apply(1f); // exakt Normalmaß
        }
        if (active == this) active = null;
    }
}