using UnityEngine;

[RequireComponent(typeof(BoxBody))]
public class BodensensorTrigger : MonoBehaviour
{
    public static BodensensorTrigger Instance { get; private set; }

    public event System.Action BallLost;

    private BoxBody zone;

    private void Awake()
    {
        Instance = this;
        zone = GetComponent<BoxBody>();
        zone.Solid = false; // Zone, kein Hindernis: der Ball soll hindurchfliegen
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // Wird vom Ball nach jedem Bewegungsschritt aufgerufen (ersetzt OnTriggerEnter)
    public void Check(BallController ball, Vector2 ballCenter, float ballRadius)
    {
        if (!zone.Overlaps(ballCenter, ballRadius)) return;

        // Zusatzball: kein Lebensverlust
        if (ball.IsExtra)
        {
            ball.Remove();

            // Hauptball war schon weg, das war der letzte Ball
            if (BallController.MainLost && BallController.ExtraCount == 0)
            {
                BallController.MainLost = false;
                BallLost?.Invoke();
            }
            return;
        }

        // Hauptball weg, aber Zusatzbälle fliegen noch: warten
        if (BallController.ExtraCount > 0)
        {
            BallController.MainLost = true;
            ball.gameObject.SetActive(false);
            return;
        }

        BallLost?.Invoke();
    }
}
