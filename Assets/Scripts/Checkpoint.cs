using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool pickedUp = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return; // prevent double pick up 
        if (!other.CompareTag("Player")) return; // only the player triggers this

        pickedUp = true; 

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();

        if (enemySpawner != null)
        {
            SoundManager.Instance.PlaySound("checkpoint");
            GameManager.Instance.CheckpointCompleted();
        }
        else
        {
            Debug.LogError("No EnemySpawner found in the scene!");
        }

        Destroy(gameObject);
    }
}
