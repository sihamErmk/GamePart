using UnityEngine;

public class CoinCollector
{
    private int[,] dp;
    private int[,] coins;
    private int rows, cols;

    // Constructor that accepts two arguments
    public CoinCollector(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        dp = new int[rows, cols];
        coins = new int[rows, cols];
    }

    // Function to set the coins in the grid
    public void SetCoin(int x, int y, int value)
    {
        if (x >= 0 && x < rows && y >= 0 && y < cols)
        {
            coins[x, y] = value;
        }
        else
        {
            Debug.LogWarning($"Attempted to set coin outside bounds at ({x}, {y})");
        }
    }

    // Function to compute the maximum coins that can be collected
    public void ComputeMaxCoins()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                dp[i, j] = coins[i, j];
                if (i > 0) dp[i, j] = Mathf.Max(dp[i, j], dp[i - 1, j] + coins[i, j]);
                if (j > 0) dp[i, j] = Mathf.Max(dp[i, j], dp[i, j - 1] + coins[i, j]);
            }
        }
    }

    // Function to get the maximum coins collected up to cell (x, y)
    public int GetMaxCoins(int x, int y)
    {
        if (x >= 0 && x < rows && y >= 0 && y < cols)
        {
            return dp[x, y];
        }
        else
        {
            Debug.LogWarning($"Attempted to access dp array outside bounds at ({x}, {y})");
            return 0;
        }
    }

    // Function to get the value of the coin at cell (x, y)
    public int GetCoin(int x, int y)
    {
        if (x >= 0 && x < rows && y >= 0 && y < cols)
        {
            return coins[x, y];
        }
        else
        {
            Debug.LogWarning($"Attempted to access coins array outside bounds at ({x}, {y})");
            return 0;
        }
    }
}
