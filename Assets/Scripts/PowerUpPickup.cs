using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private PowerUp effectPrefab;
    [SerializeField] private float spinSpeed = 120f;

    private void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime, 0f, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PaddleController>() == null) return;

        if (effectPrefab != null)
        {
            PowerUp effect = Instantiate(effectPrefab);
            effect.Begin();
        }

        Destroy(gameObject);
    }
}