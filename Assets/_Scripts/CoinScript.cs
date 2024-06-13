using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private CoinManager coinManager;

    // Set reference to the CoinManager
    public void SetCoinManager(CoinManager manager)
    {
        coinManager = manager;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Automatically tag the coin GameObject as "Coin"
        gameObject.tag = "Coin";
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Destroy the coin
            Destroy(gameObject);

            // Check if coinManager is set
            if (coinManager != null)
            {
                // Notify CoinManager that a coin was collected
                coinManager.CollectCoin(gameObject);
            }
        }
    }
}
