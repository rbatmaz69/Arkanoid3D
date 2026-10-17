using UnityEngine;

public class Block : MonoBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        if(destroyed) return;
        if(collision.collider.GetComponent<BallController>() == null) return;

        destroyed = true;

        if (gameManager != null)
        {
            gameManager.AddScore(points);
            gameManager.BlockDestroyed();
        }

        Destroy(gameObject);
    }
}
