using UnityEngine;

public class PowerUpFall : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float killZ = -9f;

    private void Update()
    {
        transform.position += Vector3.back * fallSpeed * Time.deltaTime;

        if (transform.position.z < killZ)
        {
            Destroy(gameObject);
        }
    }
}