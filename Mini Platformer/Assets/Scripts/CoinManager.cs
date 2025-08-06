using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    public Collider2D levelEndCollider;

    [SerializeField] private int totalCoins = 5;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        totalCoins = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
        UpdateUI();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateUI();

        if (coinCount >= totalCoins)
        {
            UnlockLevelEnd();
        }
    }
    public int GetRequiredCoinCount()
    {
        return totalCoins;
    }

    void UnlockLevelEnd()
    {
        if (levelEndCollider != null)
            levelEndCollider.isTrigger = true; //  enables trigger once all coins collected
    }

    void UpdateUI()
    {
        coinText.text = $"Coins: {coinCount}";
    }
}
