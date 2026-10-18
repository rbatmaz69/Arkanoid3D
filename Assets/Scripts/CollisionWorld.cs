using System.Collections.Generic;
using UnityEngine;

// Eigene 2D-Kollisionsmathematik auf der X/Z-Ebene (Vector2.x = X, Vector2.y = Z).
public static class CollisionWorld
{
    public static readonly List<BoxBody> Solids = new List<BoxBody>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Solids.Clear();
    }

    // Kreis gegen Rechteck. normal zeigt vom Rechteck zum Kreis.
    public static bool CircleVsRect(Vector2 c, float r, Vector2 min, Vector2 max,
        out Vector2 normal, out float penetration)
    {
        Vector2 closest = new Vector2(Mathf.Clamp(c.x, min.x, max.x), Mathf.Clamp(c.y, min.y, max.y));
        Vector2 d = c - closest;
        float sq = d.sqrMagnitude;

        normal = Vector2.zero;
        penetration = 0f;

        if (sq > r * r) return false;

        if (sq > 1e-8f)
        {
            float dist = Mathf.Sqrt(sq);
            normal = d / dist;
            penetration = r - dist;
            return true;
        }

        // Mittelpunkt liegt im Rechteck: kleinste Austrittsachse
        float left = c.x - min.x;
        float right = max.x - c.x;
        float bottom = c.y - min.y;
        float top = max.y - c.y;
        float m = Mathf.Min(Mathf.Min(left, right), Mathf.Min(bottom, top));

        if (m == left) normal = Vector2.left;
        else if (m == right) normal = Vector2.right;
        else if (m == bottom) normal = Vector2.down;
        else normal = Vector2.up;

        penetration = m + r;
        return true;
    }

    // Kreis gegen Kapsel (Strecke a-b mit Radius segRadius).
    public static bool CircleVsCapsule(Vector2 c, float r, Vector2 a, Vector2 b, float segRadius,
        out Vector2 normal, out float penetration)
    {
        Vector2 ab = b - a;
        float lenSq = ab.sqrMagnitude;
        float t = lenSq > 1e-8f ? Mathf.Clamp01(Vector2.Dot(c - a, ab) / lenSq) : 0f;
        Vector2 closest = a + ab * t;

        Vector2 d = c - closest;
        float sq = d.sqrMagnitude;
        float sum = r + segRadius;

        normal = Vector2.zero;
        penetration = 0f;

        if (sq > sum * sum) return false;

        if (sq > 1e-8f)
        {
            float dist = Mathf.Sqrt(sq);
            normal = d / dist;
            penetration = sum - dist;
        }
        else
        {
            // Mittelpunkt exakt auf der Strecke: nach oben (+Z) herausschieben
            normal = Vector2.up;
            penetration = sum;
        }
        return true;
    }
}
