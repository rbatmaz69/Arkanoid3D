using UnityEngine;

public class SlowMotionPowerUp : PowerUp
{
    private static SlowMotionPowerUp active;

    private Light sun;
    private float originalAngle;
    private float lowAngle = 18f;
    private float angle;
    private float factorTarget = 0.6f;

    protected override void OnBegin()
    {
        // Erneutes Einsammeln: alten Effekt entfernen, nicht stapeln
        if (active != null && active != this)
        {
            originalAngle = active.originalAngle;
            angle = active.angle;
            Destroy(active.gameObject);
        }
        else
        {
            sun = FindFirstObjectByType<Light>();
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light l in lights)
            {
                if (l.type == LightType.Directional) sun = l;
            }
            originalAngle = sun.transform.eulerAngles.x;
            angle = originalAngle;
        }

        if (sun == null)
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light l in lights)
            {
                if (l.type == LightType.Directional) sun = l;
            }
        }

        active = this;
        BallController.SpeedFactor = factorTarget;
    }

    private void LateUpdate()
    {
        if (sun == null) return;

        float target = Remaining < 0.8f ? originalAngle : lowAngle;
        angle = Mathf.MoveTowards(angle, target, 30f * Time.deltaTime);
        SetAngle(angle);
    }

    private void SetAngle(float x)
    {
        Vector3 e = sun.transform.eulerAngles;
        sun.transform.rotation = Quaternion.Euler(x, e.y, e.z);
    }

    protected override void OnEnd()
    {
        if (active == this)
        {
            active = null;
            BallController.SpeedFactor = 1f;
            if (sun != null) SetAngle(originalAngle); // exakt zurück
        }
    }
}