/*using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float collectRadius = 1f;

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= collectRadius)
        {
           // CollectCoin();
        }
    }

  /*  private void CollectCoin()
    {
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(1); // Assuming each coin adds 1 to the score
        }

        Destroy(gameObject);
    }

   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }*/

