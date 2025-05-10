using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private float moveSpeed;
    private int health;
    private int damage;
    private float targetUpdateInterval = 1.0f;
    private float targetUpdateTimer = 0.0f;
    private Rigidbody2D rb;
    private Transform target;
    private EnemyTypeID enemyTypeID;

    public void Initialize()
    {
        Enemy enemy = GetComponent<Enemy>();
        
        moveSpeed = enemy.GetSpeed();
        health = enemy.GetHealth();
        damage = enemy.GetDamage();
        enemyTypeID = enemy.GetEnemyTypeID();

        rb = GetComponent<Rigidbody2D>();

        UpdateTarget();
    }

    void FixedUpdate()
    {
        targetUpdateTimer -= Time.deltaTime;
        if (targetUpdateTimer <= 0.0f) 
        {
            targetUpdateTimer = targetUpdateInterval;
            UpdateTarget();
        }

        if (target != null) 
        {
            Vector2 direction = (target.position - transform.position).normalized;

            // handle facing direction 
            Vector3 newScale = transform.localScale;
            if (direction.x < 0) 
            {
                if (enemyTypeID == EnemyTypeID.MINIBOSS_ENEMY)
                {
                    newScale.x = Mathf.Abs(newScale.x);
                }
                else 
                {
                    newScale.x = -Mathf.Abs(newScale.x);
                }
            }
            else 
            {
                if (enemyTypeID == EnemyTypeID.MINIBOSS_ENEMY)
                {
                    newScale.x = -Mathf.Abs(newScale.x);
                }
                else 
                {
                    newScale.x = Mathf.Abs(newScale.x);
                }
            }
            transform.localScale = newScale;

            if (!rb)
            {
                Debug.Log("NO RB FOR ");
            }

            rb.velocity = direction * moveSpeed;
        }
    }


    // updates the bot's target to the player it is closest to (updated to work with 2+ players if needed)
    private void UpdateTarget() 
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return; 

        Transform closestPlayer = players[0].transform;
        float closestDistance = Vector2.Distance(transform.position, closestPlayer.position);

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }
        target = closestPlayer;
    }

    public Transform GetTarget()
    {
        return target;
    }
}
