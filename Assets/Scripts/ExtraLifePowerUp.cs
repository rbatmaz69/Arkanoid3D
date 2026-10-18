using UnityEngine;

public class ExtraLifePowerUp : PowerUp
{
    protected override void OnBegin()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.AddLife();
    }

    protected override void OnEnd() { }
}