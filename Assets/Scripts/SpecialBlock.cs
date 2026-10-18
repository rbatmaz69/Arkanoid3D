using UnityEngine;

[RequireComponent(typeof(BoxBody))]
public class SpecialBlock : MonoBehaviour, IBallHitReceiver
{
    [Header("Werte")]
    [SerializeField] private int points = 200;

    [Header("Power-Ups")]
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private float dropHeight = 0.65f;

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

        destroyed = true;

        SpawnPowerUp();

        if (gameManager != null)
        {
            gameManager.AddScore(points);
            gameManager.BlockDestroyed();
        }

        GetComponent<BoxBody>().enabled = false;
        Destroy(gameObject);
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.Log("Kein Power-Up zugewiesen");
            return;
        }

        int index = Random.Range(0, powerUpPrefabs.Length);
        Debug.Log("Power-Up Nummer: " + index);

        Vector3 spawnPosition = new Vector3(transform.position.x, dropHeight, transform.position.z);
        Instantiate(powerUpPrefabs[index], spawnPosition, Quaternion.identity);
    }
}