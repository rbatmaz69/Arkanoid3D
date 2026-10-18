using UnityEngine;

public class GenericPowerUp : PowerUp
{
    protected override void OnBegin() { Debug.Log("Power-Up gestartet: " + DisplayName); }
    protected override void OnEnd() { Debug.Log("Power-Up beendet: " + DisplayName); }
}