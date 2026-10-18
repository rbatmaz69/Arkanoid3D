using UnityEngine;

public class MultiBallPowerUp : PowerUp
{
    [SerializeField] private GameObject extraBallPrefab;

    protected override void OnBegin()
    {
        BallController main = FindFirstObjectByType<BallController>();
        if (main == null || extraBallPrefab == null) return;

        float[] angles = { -30f, 30f };
        foreach (float angle in angles)
        {
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            GameObject obj = Instantiate(extraBallPrefab, main.transform.position, Quaternion.identity);
            obj.GetComponent<BallController>().StartAsExtra(dir);
        }
    }

    protected override void OnEnd() { }
}