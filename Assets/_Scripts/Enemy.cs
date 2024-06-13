using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public Transform playerTransform;
    private Animator animator;
    private int attackCount = 0; // Counter for the number of attacks

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetBool("isslicing", true);
            Debug.Log("Enemy is attacking the player!");

            // Increment the attack counter
            attackCount++;

            // Check if the attack count reaches 3
            if (attackCount >= 3)
            {
                // Trigger the player's death
                PlayerMoving player = collision.gameObject.GetComponent<PlayerMoving>();
                if (player != null)
                {
                    player.Die();
                }
            }
        }
    }
}
