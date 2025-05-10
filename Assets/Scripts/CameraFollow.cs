using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    private Transform player1Transform;
    private Transform player2Transform;
    private float smoothSpeed = 5f;
    private GameObject p1;
    private GameObject p2;

    // vars for hurt player when too far away
    public Image tooFarHurt;
    private float maxDistance = 10f;  // update as we see fit (damage done)
    private float tooFarDamageTime = 2.0f;
    private bool isTooFar = false;
    const float TOO_FAR_COOLDOWN = 1.5f;

    // vars for player duo power
    private bool canUseDuoPower = true;
    private bool playersTouching = false;
    private float duoPowerCooldown = 60f;
    private float duoPowerRadius = 5f;
    private int duoPowerDamage = 15;
    private GameObject zapPrefab;
    private float lastUsedTime;


    void Start()
    {
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");
        player1Transform = p1.transform;
        player2Transform = p2.transform;

        zapPrefab = Resources.Load<GameObject>("Prefabs/zap");
    }

    // move camera to the midpoint between the players, or just one player if other dies
    void Update()
    {
        bool p1Alive = p1.GetComponent<PlayerHealthController>().GetIsAlive();
        bool p2Alive = p2.GetComponent<PlayerHealthController>().GetIsAlive();

        // both players died, game over
        if (!p1Alive && !p2Alive)
        {
            GameManager.Instance.EndGame(false);
        }

        if (p1Alive && p2Alive) 
        {
            Vector3 midpoint = (player1Transform.position + player2Transform.position) / 2f;
            transform.position = Vector3.Lerp(transform.position, new Vector3(midpoint.x, midpoint.y, transform.position.z), smoothSpeed * Time.deltaTime);
        }
        else if (p1Alive) 
        {
            Vector3 midpoint = player1Transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(midpoint.x, midpoint.y, transform.position.z), smoothSpeed * Time.deltaTime);
        }
        else if (p2Alive) 
        {
            Vector3 midpoint = player2Transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(midpoint.x, midpoint.y, transform.position.z), smoothSpeed * Time.deltaTime);
        }

        if (isTooFar)
        {
            tooFarDamageTime += Time.deltaTime;
        }
        HandlePlayerDistance(p1Alive, p2Alive);
        HandlePlayerComboPower(p1Alive, p2Alive);
    }

    // if players are too far away from one another warning message and damage done 
    void HandlePlayerDistance(bool p1Alive, bool p2Alive)
    {
        if (p1Alive && p2Alive) 
        {
            float distance = Vector3.Distance(player1Transform.position, player2Transform.position);

            if (distance > maxDistance)
            {
                SoundManager.Instance.PlaySound("warning");
                isTooFar = true;
                tooFarHurt.gameObject.SetActive(true);
                GameManager.Instance.uiMessages.color = Color.red;
                GameManager.Instance.uiMessages.text = "Warning: too far apart";
                if (tooFarDamageTime > TOO_FAR_COOLDOWN)
                {
                    p1.GetComponent<PlayerHealthController>().TakeDamage(5);
                    p2.GetComponent<PlayerHealthController>().TakeDamage(5);
                    tooFarDamageTime = 0.0f;
                }
            }
            else 
            {
                isTooFar = false;
                tooFarDamageTime = 2.0f;
                tooFarHurt.gameObject.SetActive(false);

                // reset if message is no longer needed
                if (GameManager.Instance.uiMessages.text == "Warning: too far apart")
                {
                    GameManager.Instance.uiMessages.text = "";
                }
            }
        }
        else 
        {
            tooFarHurt.gameObject.SetActive(false);

            // reset if message is no longer needed
            if (GameManager.Instance.uiMessages.text == "Warning: too far apart")
            {
                GameManager.Instance.uiMessages.text = "";
            }
        }
    }

    void HandlePlayerComboPower(bool p1Alive, bool p2Alive)
    {
        if (p1Alive && p2Alive) 
        {
            float playerDist = Vector2.Distance(p1.transform.position, p2.transform.position);
            // check if players are touching
            if (playerDist < 0.5f) 
            {
                playersTouching = true;
                float timeSinceLastUse = GameManager.Instance.GetCurrentTimeElapsed() - lastUsedTime;
                float timeUntilNextUse = Mathf.Max(0, duoPowerCooldown - timeSinceLastUse);
                int seconds = Mathf.FloorToInt(timeUntilNextUse);
                GameManager.Instance.uiMessages.color = Color.cyan;
                GameManager.Instance.uiMessages.text = "";
                GameManager.Instance.uiMessages.text = canUseDuoPower ? "Press 'space' to use combo power" : string.Format("{0:00}s until next use", seconds);
            }
            else
            {
                playersTouching = false;
                // reset the ui message as needed
                if (GameManager.Instance.uiMessages.text == "Press 'space' to use combo power" || GameManager.Instance.uiMessages.text.Contains("until next use"))
                {
                    GameManager.Instance.uiMessages.text = "";
                }
            }

            // trigger Duo Power when pressing Space
            if (playersTouching && canUseDuoPower && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(ActivateDuoPower());
            }
        }
        else 
        {
            // reset the ui message as needed
            if (GameManager.Instance.uiMessages.text == "Press 'space' to use combo power" || GameManager.Instance.uiMessages.text.Contains("until next use"))
            {
                GameManager.Instance.uiMessages.text = "";
            }
        }
    }

    // coroutine for player combo power
    IEnumerator ActivateDuoPower()
    {
        canUseDuoPower = false;
        lastUsedTime = GameManager.Instance.GetCurrentTimeElapsed();

        // zap at midpoint between players
        Vector3 zapPosition = (p1.transform.position + p2.transform.position) / 2;
        GameObject zap = Instantiate(zapPrefab, zapPosition, Quaternion.identity);

        SoundManager.Instance.PlaySound("zap_sound");
        
        // destroy prefab after animation
        float zapDuration = zap.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(zap, zapDuration);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, duoPowerRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(duoPowerDamage);
            }
        }

        yield return new WaitForSeconds(duoPowerCooldown);
        canUseDuoPower = true;
    }

}
