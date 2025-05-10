using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombPower : Weapon
{
    private GameObject bombPrefab;
    private GameObject explosionPrefab;
    private float bombSpeed = 3f;
    private float explosionRadius = 1f;
    private float throwInterval = 2f;

    void Start()
    {
        type = WeaponType.BOMB;
        isMaxLevel = false;
        maxLevel = 3;
        damage = 20;
        bombPrefab = Resources.Load<GameObject>("Prefabs/bomb"); 
        explosionPrefab = Resources.Load<GameObject>("Prefabs/explode"); 
        Activate();
    }

    void Update()
    {
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }
    }

    public override void Activate()
    {
        isActive = true;
        StartCoroutine(ThrowBombs());
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();

        // destroy any leftover bombs/explosions that may have spawned while player died
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        GameObject[] explosions = GameObject.FindGameObjectsWithTag("Explosion");

        foreach (GameObject bomb in bombs)
        {
            Destroy(bomb);
        }

        foreach (GameObject explosion in explosions)
        {
            Destroy(explosion);
        }
    }

    public override void DoDamage() 
    {
    }

    public override void LevelUp()
    {
        if (currLevel < maxLevel)
        {
            currLevel++;
            StopAllCoroutines();
            StartCoroutine(ThrowBombs());
        }
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }

    // repeatedly throws bombs (# bombs = to 1+ current level)
    private IEnumerator ThrowBombs()
    {
        while (isActive)
        {
            for (int i = 0; i < currLevel+1; i++)
            {
                // throws in random direction
                Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                ThrowBomb(direction);

                // play sound
            }
            yield return new WaitForSeconds(throwInterval);
        }
    }

    private void ThrowBomb(Vector2 direction)
    {
        GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);

        StartCoroutine(HandleBombExplosion(bomb, direction));
    }


    // handles bomb movement and explosion
    private IEnumerator HandleBombExplosion(GameObject bomb, Vector2 direction)
    {
        // if bomb has already been destroyed (game paused) just exit
        if (!bomb) yield break;

        // throw a random distance
        float dist = Random.Range(1.5f, 3.5f);
        Vector3 targetPosition = transform.position + new Vector3(direction.x * dist, direction.y * dist, 0);

        while (Vector2.Distance(bomb.transform.position, targetPosition) > 0.1f)
        {
            bomb.transform.position = Vector2.MoveTowards(bomb.transform.position, targetPosition, bombSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        Destroy(bomb); 
        GameObject explosion = Instantiate(explosionPrefab, targetPosition, Quaternion.identity);

        // hurt all enemies within explosion radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(targetPosition, explosionRadius);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        // wait 0.5f and then destroy prefabs (did this way so that it still deletes even if game pauses)
        float timeToWait = 0.5f;
        float elapsed = 0f;

        while (elapsed < timeToWait)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        if (bomb) Destroy(bomb);
        Destroy(explosion); 
    }
}

