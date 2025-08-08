using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public int coinCount = 0;
    public TextMeshProUGUI coinText;
    public Collider2D levelEndCollider;

    [SerializeField] private int totalCoins = 5;

    private void Awake()
    {
        // singleton so we dont spawn duplicates
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // count how many coins are actually in the scene at start
        totalCoins = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        UpdateUI();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateUI();

        // open the level exit if we have all the coins
        if (coinCount >= totalCoins)
            UnlockLevelEnd();
    }

    public int GetRequiredCoinCount()
    {
        return totalCoins;
    }

    private void UnlockLevelEnd()
    {
        // switch to trigger so player can go through
        if (levelEndCollider != null)
            levelEndCollider.isTrigger = true;
    }

    private void UpdateUI()
    {
        coinText.text = $"COINS: {coinCount}";
    }
}
