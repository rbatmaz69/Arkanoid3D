using UnityEngine;

public class BodensensorTrigger : MonoBehaviour
{
    public event System.Action BallLost;

    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();
        if (ball == null) return;

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