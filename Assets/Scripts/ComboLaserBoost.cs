using UnityEngine;

public class ComboLaserBoost : MonoBehaviour
{
    private GameObject comboPrefab;
    void Start()
    {
        comboPrefab = Resources.Load<GameObject>("Prefabs/comboLaser");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // if both players not alive, do nothing 
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length < 2) 
            {
                GameManager.Instance.uiMessages.color = Color.black;
                GameManager.Instance.ShowTemporaryMessage("Need both players alive to use combo laser", 3f);
                return;
            }

            GameObject laserGO = Instantiate(comboPrefab, Vector3.zero, Quaternion.identity);

            ComboLaser laserScript = laserGO.GetComponent<ComboLaser>();
            laserScript.player1 = players[0].transform;
            laserScript.player2 = players[1].transform;

            SoundManager.Instance.PlaySound("heal"); // heal sound is similar enough
            Destroy(gameObject);
        }
    }
}
