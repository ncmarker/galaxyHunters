using UnityEngine;

public class ReviveBoost : MonoBehaviour
{
    private GameObject player1;
    private GameObject player2;
    
    void Start()
    {
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
    }

    // checks if a player is dead when powerup picked up, if so will revive the dead player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bool player1Alive = player1.GetComponent<PlayerHealthController>().GetIsAlive();
            bool player2Alive = player2.GetComponent<PlayerHealthController>().GetIsAlive();

            if (player1Alive && !player2Alive)
            {
                RevivePlayer(player2, player1);
            }
            else if (!player1Alive && player2Alive)
            {
                RevivePlayer(player1, player2);
            }
            else 
            {
                GameManager.Instance.ShowTemporaryMessage("No dead players to revivie", 2f);
            }

            Destroy(gameObject);
        }
    }

    // brings the specified player back to life
    private void RevivePlayer(GameObject player, GameObject other)
    {
        SoundManager.Instance.PlaySound("revive");
        player.SetActive(true);
        PlayerHealthController healthController = player.GetComponent<PlayerHealthController>();

        if (healthController != null)
        {
            healthController.SetIsAlive(true); 
            healthController.MaxOutHealth();
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ActivateAllWeapons();
        }

        player.transform.position = other.transform.position;
    }
}
