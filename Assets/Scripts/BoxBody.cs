using UnityEngine;

// Achsparalleles Rechteck auf der X/Z-Ebene. Ersetzt den BoxCollider.
// center und size sind lokal wie beim BoxCollider, die Y-Komponente wird ignoriert.
public class BoxBody : MonoBehaviour
{
    [SerializeField] private Vector3 center;
    [SerializeField] private Vector3 size = Vector3.one;

    public Vector3 Center { get => center; set => center = value; }
    public Vector3 Size { get => size; set => size = value; }

    // Nur Hindernisse (Wände, Blöcke) melden sich an. Trigger-Zonen und Power-ups nutzen GetRect direkt.
    [SerializeField] private bool solid = true;
    public bool Solid
    {
        get => solid;
        set
        {
            if (solid == value) return;
            solid = value;

            // Zur Laufzeit sofort an- oder abmelden (Zonen setzen das in Awake, Reihenfolge ist egal)
            if (!Application.isPlaying || !isActiveAndEnabled) return;
            if (solid) Register();
            else CollisionWorld.Solids.Remove(this);
        }
    }

    private void OnEnable()
    {
        if (solid) Register();
    }

    private void Register()
    {
        if (!CollisionWorld.Solids.Contains(this)) CollisionWorld.Solids.Add(this);
    }

    private void OnDisable()
    {
        CollisionWorld.Solids.Remove(this);
    }

    public void GetRect(out Vector2 min, out Vector2 max)
    {
        Vector3 c = transform.TransformPoint(center);
        Vector3 s = transform.lossyScale;
        Vector2 half = new Vector2(Mathf.Abs(size.x * s.x), Mathf.Abs(size.z * s.z)) * 0.5f;
        min = new Vector2(c.x - half.x, c.z - half.y);
        max = new Vector2(c.x + half.x, c.z + half.y);
    }

    // Kreis (X/Z) gegen dieses Rechteck, z. B. für Trigger-Zonen.
    public bool Overlaps(Vector2 circleCenter, float radius)
    {
        GetRect(out Vector2 min, out Vector2 max);
        return CollisionWorld.CircleVsRect(circleCenter, radius, min, max, out _, out _);
    }

    private void OnDrawGizmos()
    {
        GetRect(out Vector2 min, out Vector2 max);
        Gizmos.color = solid ? Color.green : Color.yellow;
        Vector3 c = new Vector3((min.x + max.x) * 0.5f, transform.position.y, (min.y + max.y) * 0.5f);
        Gizmos.DrawWireCube(c, new Vector3(max.x - min.x, 0.1f, max.y - min.y));
    }
}
