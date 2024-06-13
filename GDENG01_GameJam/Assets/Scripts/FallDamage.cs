using UnityEngine;

public class FallDamage : MonoBehaviour
{
    [SerializeField] private float fallDamageThreshold = 10f; 
    [SerializeField] private PlayerController playerController; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && rb.velocity.y < -fallDamageThreshold)
        {
            float fallSpeed = Mathf.Abs(rb.velocity.y);
            playerController.TakeDamage(fallSpeed); 
        }
    }
}
