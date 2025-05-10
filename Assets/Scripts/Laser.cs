using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private int damage;

    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // If the laser is outside the screen bounds (viewport coordinates between 0 and 1)
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int dmg) 
    {
        damage = dmg;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.GetComponent<Enemy>() != null) 
        {
            Enemy otherEnemy = other.GetComponent<Enemy>();
            otherEnemy.TakeDamage(damage);
            Destroy(gameObject); 
        }
    }
}