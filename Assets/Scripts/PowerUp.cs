using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] private string displayName = "Power-Up";
    [SerializeField] private float duration = 8f;

    public static event System.Action<PowerUp> Started;
    public static event System.Action<PowerUp> Ended;

    public string DisplayName => displayName;
    public float Remaining { get; private set; }

    public void Begin()
    {
        Remaining = duration;
        OnBegin();
        Started?.Invoke(this);
    }

    private void Update()
    {
        Remaining -= Time.deltaTime;
        if (Remaining <= 0f) Finish();
    }

    private void Finish()
    {
        OnEnd();
        Ended?.Invoke(this);
        Destroy(gameObject);
    }

    protected abstract void OnBegin();
    protected abstract void OnEnd();
}