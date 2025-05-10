using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// **could use some updating to make more smooth
public class MagnetBoost : MonoBehaviour
{
    private float magnetRadius = 30f;
    private float attractionSpeed = 300f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AttractXPToPlayer(other.transform));
            Destroy(gameObject);
        }
    }

    private IEnumerator AttractXPToPlayer(Transform playerTransform)
    {
        // get all XP gems
        Collider2D[] xpGems = Physics2D.OverlapCircleAll(transform.position, magnetRadius);

        List<Transform> gemsToAttract = new List<Transform>();

        foreach (Collider2D xpGem in xpGems)
        {
            if (xpGem.CompareTag("XP"))
            {
                gemsToAttract.Add(xpGem.transform);
            }
        }

        // move gems towards player
        bool allGemsAtPlayer = false;

        while (!allGemsAtPlayer)
        {
            allGemsAtPlayer = true; 

            foreach (Transform xpGem in gemsToAttract)
            {
                float distance = Vector2.Distance(xpGem.position, playerTransform.position);

                if (distance > 1.0f) // 1.0f gap so player can still collect as XP points
                {
                    xpGem.position = Vector2.MoveTowards(xpGem.position, playerTransform.position, attractionSpeed * Time.deltaTime);
                    allGemsAtPlayer = false; 
                }
            }

            yield return null; 
        }
    }
}
