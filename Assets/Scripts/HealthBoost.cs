using UnityEngine;

public class HealthBoost : MonoBehaviour
{
        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("heal");
            other.GetComponent<PlayerHealthController>().MaxOutHealth();
            Destroy(gameObject);
        }
    }
}
