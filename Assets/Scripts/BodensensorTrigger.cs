using UnityEngine;

public class BodensensorTrigger : MonoBehaviour
{
    public event System.Action BallLost;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<BallController>() == null) return;

        Debug.Log("Ball verloren");
        BallLost?.Invoke();
    }
}
