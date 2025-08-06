using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            CoinManager.instance.AddCoin();
            Destroy(gameObject);
        }
    }
}
