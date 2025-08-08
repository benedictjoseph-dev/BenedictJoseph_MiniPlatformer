using UnityEngine;

public class Collectible : MonoBehaviour
{
    public AudioClip collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // only player can pick this up
        if (collision.CompareTag("Player"))
        {
            // play coin sfx at the pickup location
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // tell coin manager we got one
            CoinManager.instance.AddCoin();

            // remove the coin from the scene
            Destroy(gameObject);
        }
    }
}
