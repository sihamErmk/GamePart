using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class PlayerMoving : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 3f;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Animator animator;
    [SerializeField] private Tilemap floorTilemap;

    private Pathfinding pathfinding;
    private List<Node> path;
    private Transform treasure;
    private CoinCollector coinCollector;
    private Vector3 destination;
    private Vector2 movement;

    private bool isAttacking = false;
    private bool isDead = false;

    private TreasureManager treasureManager;
    private CoinManager coinManager;
    [SerializeField] private float maxDistanceToCollectCoin = 5f;
    private bool automaticMovement = false;
    public Enemy enemy;
    private float enemyDetectionRadius;
    
    private void Start()
    {
        pathfinding = GetComponent<Pathfinding>();
        coinCollector = new CoinCollector(floorTilemap.size.y, floorTilemap.size.x);
        treasureManager = FindObjectOfType<TreasureManager>();
        coinManager = FindObjectOfType<CoinManager>();

        if (coinManager == null)
        {
            Debug.LogError("CoinManager component not found!");
            return;
        }

        Vector3Int playerStart = PlacePlayerInRandomPosition();
        treasureManager.GenerateTreasure(playerStart);
        FindTreasure();
        ComputeCoinCollection();
        pathfinding.SetCoinManager(coinManager);
        automaticMovement = false;
    }
    public void UpdateTreasurePosition(Vector3Int newPosition)
    {
        // Find a new path to the updated treasure position
        Vector3Int playerTile = (Vector3Int)floorTilemap.WorldToCell(transform.position);
        path = pathfinding.FindPath(playerTile, newPosition);

        // Optional: You may want to visualize the new path or update some UI elements
        // For example:
        if (path != null && path.Count > 0)
        {
            Debug.Log("New path to treasure updated.");
            // You can visualize the new path using debug draw or update UI elements
        }
        else
        {
            Debug.LogWarning("No path found to the new treasure position.");
            // Handle the case when no path is found to the new treasure position
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FindTreasureManually();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            automaticMovement = !automaticMovement;
            Debug.Log("Automatic Movement: " + automaticMovement);

            if (automaticMovement)
            {
                TriggerPathfinding();
                // FindTreasure();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
        else
        {
            HandleManualMovement();
        }

        if (enemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.playerTransform.position);
            animator.SetBool("isslicing", distanceToEnemy <= enemy.detectionRange);
        }
    }

    private void TriggerPathfinding()
    {
        Vector3Int playerTile = (Vector3Int)floorTilemap.WorldToCell(transform.position);
        FindTreasure();
        path = pathfinding.FindPath(playerTile, GetTreasurePosition());
        StartCoroutine(MoveToNextCoin());
    }

    private void HandleManualMovement()
    {
        if (isDead || isAttacking) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        Vector2 currentPosition = transform.position;
        Vector2 newPosition = currentPosition + movement * playerSpeed * Time.deltaTime;
        playerBody.MovePosition(newPosition);

        animator.SetFloat("Horizontal", movement.x);

        Vector2 direction = (newPosition - currentPosition).normalized;
        RaycastHit2D hit = Physics2D.Raycast(currentPosition, direction, 0.5f, LayerMask.GetMask("Walls"));
        if (hit.collider != null)
        {
            Vector2 adjustedPosition = hit.point - (direction * 0.5f);
            playerBody.MovePosition(adjustedPosition);
        }

        CollectCoinsAtPosition(newPosition);
    }

    private void FixedUpdate()
    {
        if (isDead || isAttacking) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        if (path != null && path.Count > 0)
        {
            Vector2 direction = (destination - transform.position).normalized;
            playerBody.MovePosition(playerBody.position + direction * playerSpeed * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, destination) < 0.1f)
            {
                path.RemoveAt(0);

                if (path.Count > 0)
                {
                    destination = path[0].worldPosition;
                }
                else
                {
                    destination = transform.position;
                }
            }

            animator.SetFloat("Horizontal", direction.x);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Walls"));
            if (hit.collider != null)
            {
                Vector2 newPosition = hit.point - (direction * 0.5f);
                playerBody.MovePosition(newPosition);
            }

            CollectCoinsAtPosition(transform.position);
        }
    }

    private void CollectCoinsAtPosition(Vector3 position)
    {
        Collider2D[] nearbyCoins = Physics2D.OverlapCircleAll(position, maxDistanceToCollectCoin, LayerMask.GetMask("Coins"));

        foreach (Collider2D coinCollider in nearbyCoins)
        {
            if (coinCollider.CompareTag("Coin"))
            {
                CollectCoin(coinCollider.gameObject);
            }
        }
    }

    private Vector3Int PlacePlayerInRandomPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int randomCell;
        Vector3 randomPosition;

        do
        {
            randomCell = GetRandomPosition(bounds);
            randomPosition = floorTilemap.CellToWorld(randomCell) + new Vector3(0.5f, 0.5f, 0f);
        } while (!floorTilemap.HasTile(randomCell));

        transform.position = randomPosition;
        return randomCell;
    }

    private void FindTreasureManually()
    {
        // Implement manual treasure detection logic here
        // Update the 'treasure' variable accordingly
    }
    public void SetTreasure(Transform newTreasure)
    {
        treasure = newTreasure;
    }


    private void FindTreasure()
    {
        if (!automaticMovement)
        {
            Debug.Log("Automatic movement is not enabled. Treasure will not be found.");
            return;
        }

        Vector3Int playerTile = (Vector3Int)floorTilemap.WorldToCell(transform.position);
        Vector3Int treasureTile = (Vector3Int)floorTilemap.WorldToCell(treasure.position);

        // Use pathfinding to find a path to the treasure
        path = pathfinding.FindPath(playerTile, treasureTile);

        if (path != null && path.Count > 0)
        {
            Debug.Log("Path to treasure found.");
            // You can handle further movement logic using the path
            StartCoroutine(MoveToNextCoin());
        }
        else
        {
            Debug.LogWarning("No path found to the treasure.");
            // Handle the case when no path is found to the treasure
        }
    }


    private Vector3Int GetTreasurePosition()
    {
        if (treasure != null)
        {
            return (Vector3Int)floorTilemap.WorldToCell(treasure.position);
        }
        else
        {
            Debug.LogError("Treasure not found!");
            return Vector3Int.zero;
        }
    }

    private void ComputeCoinCollection()
    {
        foreach (var position in FindObjectsOfType<CoinScript>())
        {
            Vector3Int coinTile = (Vector3Int)floorTilemap.WorldToCell(position.transform.position);
            if (coinTile.x >= 0 && coinTile.x >= 0 && coinTile.x < floorTilemap.size.y && coinTile.y >= 0 && coinTile.y < floorTilemap.size.x)
            {
                coinCollector.SetCoin(coinTile.x, coinTile.y, 1);
            }
            else
            {
                Debug.LogWarning($"Coin position out of bounds: {coinTile}");
            }
        }

        coinCollector.ComputeMaxCoins();
    }

    private IEnumerator MoveToNextCoin()
    {
        Vector3Int playerTile = (Vector3Int)floorTilemap.WorldToCell(transform.position);

        Vector3Int treasureTile = (Vector3Int)floorTilemap.WorldToCell(treasure.position);

        List<Node> path = GetMaxCoinPath(playerTile, treasureTile);

        if (path != null && path.Count > 0)
        {
            foreach (Node node in path)
            {
                Vector3 coinPosition = floorTilemap.CellToWorld(new Vector3Int(node.gridPosition.x, node.gridPosition.y, 0));
                Collider2D[] nearbyCoins = Physics2D.OverlapCircleAll(coinPosition, 0.5f, LayerMask.GetMask("Coins"));

                foreach (Collider2D coinCollider in nearbyCoins)
                {
                    float distanceToCoin = Vector2.Distance(transform.position, coinCollider.transform.position);
                    if (distanceToCoin < maxDistanceToCollectCoin)
                    {
                        CollectCoin(coinCollider.gameObject);
                        continue; // Move to the next coin
                    }
                }

                destination = node.worldPosition;

                while (Vector2.Distance(transform.position, destination) > 0.1f)
                {
                    Vector2 direction = (destination - transform.position).normalized;
                    playerBody.MovePosition(playerBody.position + direction * playerSpeed * Time.fixedDeltaTime);

                    animator.SetFloat("Horizontal", direction.x);

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Walls"));
                    if (hit.collider != null)
                    {
                        Vector2 newPosition = hit.point - (direction * 0.5f);
                        playerBody.MovePosition(newPosition);
                    }

                    yield return null;
                }
            }

            List<Node> backtrackPath = GetMaxCoinPath(treasureTile, playerTile);
            if (backtrackPath != null && backtrackPath.Count > 0)
            {
                foreach (Node node in backtrackPath)
                {
                    destination = node.worldPosition;

                    while (Vector2.Distance(transform.position, destination) > 0.1f)
                    {
                        Vector2 direction = (destination - transform.position).normalized;
                        playerBody.MovePosition(playerBody.position + direction * playerSpeed * Time.fixedDeltaTime);

                        animator.SetFloat("Horizontal", direction.x);

                        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Walls"));
                        if (hit.collider != null)
                        {
                            Vector2 newPosition = hit.point - (direction * 0.5f);
                            playerBody.MovePosition(newPosition);
                        }

                        yield return null;
                    }
                }
            }

            destination = treasure.position;
            while (Vector2.Distance(transform.position, destination) > 0.1f)
            {
                Vector2 direction = (Vector2)(destination - transform.position).normalized;
                playerBody.MovePosition(playerBody.position + direction * playerSpeed * Time.fixedDeltaTime);

                animator.SetFloat("Horizontal", direction.x);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Walls"));
                if (hit.collider != null)
                {
                    Vector2 newPosition = hit.point - (direction * 0.5f);
                    playerBody.MovePosition(newPosition);
                }

                yield return null;
            }

            Debug.Log("Player reached the treasure!");
        }
        else
        {
            Debug.LogWarning("No path found to the treasure!");
        }

        yield return null;
    }

    private List<Node> GetMaxCoinPath(Vector3Int startTile, Vector3Int targetTile)
    {
        List<Node> maxCoinPath = null;
        int maxCoinCount = 0;

        List<List<Node>> allPaths = pathfinding.FindAllPaths(startTile, targetTile);

        foreach (var path in allPaths)
        {
            int coinCount = 0;
            foreach (Node node in path)
            {
                if (coinCollector.GetCoin(node.gridPosition.x, node.gridPosition.y) == 1)
                {
                    coinCount++;
                }
            }

            if (coinCount > maxCoinCount)
            {
                maxCoinPath = path;
                maxCoinCount = coinCount;
            }
        }

        return maxCoinPath;
    }

    private Vector3Int GetRandomPosition(BoundsInt bounds)
    {
        int x = Random.Range(bounds.xMin, bounds.xMax);
        int y = Random.Range(bounds.yMin, bounds.yMax);
        return new Vector3Int(x, y, 0);
    }

    private void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        // coinCollector.IncrementScore();
        // Debug.Log("Coin collected! Current score: " + coinCollector.GetScore());
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        animator.SetTrigger("Attack");

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("Player died!");
    }

    private void UpdateAnimationTriggers()
    {
        animator.SetBool("isruningleft", IsRunningLeftCondition());
        animator.SetBool("isruningright", IsRunningRightCondition());
        animator.SetBool("isslicing", IsSlicingCondition());
        if (isDead)
        {
            animator.SetTrigger("dead");
        }
    }

    private bool IsRunningLeftCondition()
    {
        return movement.x < 0 && !isAttacking && !isDead;
    }

    private bool IsRunningRightCondition()
    {
        return movement.x > 0 && !isAttacking && !isDead;
    }

    private bool IsSlicingCondition()
    {
        return isAttacking && !isDead;
    }
}

