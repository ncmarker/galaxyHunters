using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EnemyTypeID
{
    BASIC_ENEMY,
    SPEED_ENEMY,
    TANK_ENEMY,
    MINIBOSS_ENEMY,
    FINAL_BOSS_ENEMY,
    STRONG_ENEMY,
    EGG
}
// when adding, duplicate enemy prefab and change fields accordingly
// add to enemySpawner when you want them to spawn & add its type/prefab in SpawnEnemy()

public class Enemy : MonoBehaviour
{
    
    public string enemyName;
    public int health;
    public int damage;
    public float attackRange;
    public float speed;
    public int xpValue; 
    public XPType xpType;
    public GameObject damageNumberPrefab;
    public GameObject xpPrefab;
    public EnemyTypeID enemyTypeID;
    private bool isDestroyed = false; // for egg

    public Enemy(string enemyName, int health, int damage, float attackRange, float speed, int xpValue) {
        this.enemyName = enemyName;
        this.health = health;
        this.damage = damage;
        this.attackRange = attackRange;
        this.speed = speed;
        this.xpValue = xpValue;
    }
    void Start()
    {
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }

    public EnemyTypeID GetEnemyTypeID()
    {
        return enemyTypeID;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        StartCoroutine(FlashRed());

        // spawn damage numbers
        if (damageNumberPrefab) {
            GameObject damageText = Instantiate(damageNumberPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            damageText.GetComponent<DamageNumber>().Initialize(amount, Color.white);
        }

        // if an egg, update / kill differently
        if (enemyTypeID == EnemyTypeID.EGG)
        {
            HandleEggDamage();
            return;
        }
        
        // kill enemy
        if (health <= 0.0f) {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {
            spriteRenderer.color = new Color(0.984f, 0.588f, 0.588f); // #FB9696
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    public void Die() 
    {
        if (enemyTypeID == EnemyTypeID.MINIBOSS_ENEMY)
        {
            SpawnPowerupsOnDeath();
        }
        else if (enemyTypeID == EnemyTypeID.FINAL_BOSS_ENEMY)
        {
            SpawnPowerupsOnDeath();
            // GameManager.Instance.EndGame(true);
        }
        Destroy(gameObject);

        // spawn XP 80% of the time
        int chance = Random.Range(1,6);
        if (chance != 5) 
        {
            SpawnXP();
        }
    }

    // spawns XP of the specified type for that enemy
    void SpawnXP()
    {
        GameObject xp = Instantiate(xpPrefab, transform.position, Quaternion.identity);
        xp.GetComponent<XP>().Initialize(xpType);
    }

    private void SpawnPowerupsOnDeath()
    {
        GameObject healthBoost = Resources.Load<GameObject>("Prefabs/health_power");
        GameObject upgradeBoost = Resources.Load<GameObject>("Prefabs/upgrade_power");
        GameObject checkpoint = Resources.Load<GameObject>("Prefabs/checkpoint");
            
        // positions slightly outward from the boss's death position
        Vector2 bossPosition = transform.position;
        Vector2[] spawnOffsets = new Vector2[]
        {
            new Vector2(1f, 0f),  
            new Vector2(-1f, 0f), 
            new Vector2(0f, 1f) 
        };

        for (int i=0; i<spawnOffsets.Length; i++)
        {
            Vector2 spawnPosition = bossPosition + spawnOffsets[i];
            Instantiate(i < 1 ? upgradeBoost : healthBoost, spawnPosition, Quaternion.identity);
        }

        Instantiate(checkpoint, bossPosition + new Vector2(0f, -1f), Quaternion.identity);
    }

    // specific to enemy egg -- will handle damage and boss spawning 
    private void HandleEggDamage()
    {
        if (isDestroyed) return; // prevent duplicate boss spawns

        float percent = health / 100.0f; // 100.0f is starting egg health 
        SoundManager.Instance.PlaySound("glassBreak");
        
        if (health <= 0)
        {
            GameObject flash = Resources.Load<GameObject>("Prefabs/flash");
            Instantiate(flash, transform.position, Quaternion.identity);

            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();

            if (enemySpawner != null)
            {
                isDestroyed = true;
                enemySpawner.EggDestroyed(transform.position);
                Destroy(this);
            }
            else
            {
                Debug.LogError("No EnemySpawner found in the scene!");
            }

            Destroy(gameObject);
        }
        else if (percent <= 0.25)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/egg4");
        }
        else if (percent <= 0.50)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/egg3");
        }
        else if (percent <= 0.75)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/egg2");
        }
    }
}


