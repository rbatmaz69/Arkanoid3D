using UnityEngine;

[RequireComponent(typeof(BoxBody))]
public class StableBlock : MonoBehaviour, IBallHitReceiver
{
    [Header("Werte")]
    [SerializeField] private int hitsLeft = 3;
    [SerializeField] private int points = 300;

    [Header("Ringe")]
    [SerializeField] private GameObject[] rings;

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
        if (destroyed) return;

        hitsLeft--;

        if (hitsLeft >= 0 && hitsLeft < rings.Length)
        {
            rings[hitsLeft].SetActive(false);
        }

        if (hitsLeft <= 0)
        {
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
}