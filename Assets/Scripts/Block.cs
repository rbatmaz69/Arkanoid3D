using UnityEngine;

[RequireComponent(typeof(BoxBody))]
public class Block : MonoBehaviour, IBallHitReceiver
{
    [Header("Werte")]
    [SerializeField] private int points = 100;

    private GameManager gameManager;
    private bool destroyed;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.RegisterBlock();
        }
    }

    public void OnBallHit(BallController ball)
    {
        if(destroyed) return;

        destroyed = true;

        if (gameManager != null)
        {
            gameManager.AddScore(points);
            gameManager.BlockDestroyed();
        }

        GetComponent<BoxBody>().enabled = false;
        Destroy(gameObject);
    }
}
