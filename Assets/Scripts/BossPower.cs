using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPower : Weapon
{
    private GameObject alienBallPrefab;
    private float shootInterval = 1.0f;
    private float alienBallSpeed = 6f;
    private List<Vector2> shootingDirections = new List<Vector2>();

    void Start()
    {
        type = WeaponType.LASER;
        isMaxLevel = false;
        maxLevel = 3;
        damage = 5;
        alienBallPrefab = Resources.Load<GameObject>("Prefabs/alienball"); 
        Activate();
    }

    void Update()
    {
        if (currLevel == maxLevel)
        {
            isMaxLevel = true;
        }

        UpdateShootingDirections();
    }

    public override void Activate()
    {
        isActive = true;
        UpdateShootingDirections();
        StartCoroutine(ShootAllBalls());
    }

    public override void Deactivate()
    {
        isActive = false;
        StopAllCoroutines();
    }

    public override void DoDamage() 
    {
    }

    public override void LevelUp()
    {
    }

    // shoot balls towards the player, and +/- 30 degrees in either direction
    private void UpdateShootingDirections()
    {
        shootingDirections.Clear();

        Transform target = GetComponent<EnemyController>().GetTarget();
        Vector2 shootDirection = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Vector2 direction1 = new Vector2(Mathf.Cos((angle + 30) * Mathf.Deg2Rad), Mathf.Sin((angle + 30) * Mathf.Deg2Rad));
        Vector2 direction2 = new Vector2(Mathf.Cos((angle - 30) * Mathf.Deg2Rad), Mathf.Sin((angle - 30) * Mathf.Deg2Rad));

        shootingDirections.Add(shootDirection);
        shootingDirections.Add(direction1);
        shootingDirections.Add(direction2);
    }

    private IEnumerator ShootAllBalls()
    {
        while (isActive)
        {
            foreach (Vector2 direction in shootingDirections)
            {
                ShootBall(direction);
            }
            // play shooting sound
            // SoundManager.Instance.PlayBackgroundMusic("laser_shoot"); // play once per interval

            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootBall(Vector2 direction)
    {
        GameObject alienBall = Instantiate(alienBallPrefab, transform.position, Quaternion.identity);

        // pass damage to ball instance
        AlienBall alienBallScript = alienBall.GetComponent<AlienBall>();
        if (alienBallScript != null)
        {
            alienBallScript.SetDamage(damage);
        }

        Rigidbody2D rb = alienBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * alienBallSpeed;
        }

        Destroy(alienBall, 4f);
    }

    public override bool GetIsMaxLevel()
    {
        return isMaxLevel;
    }
}

