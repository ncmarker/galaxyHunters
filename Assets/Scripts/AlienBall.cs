using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBall : MonoBehaviour
{
    private int damage;

    void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // destroy ball when outside screen bounds (viewport coordinates between 0 and 1)
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int dmg) 
    {
        damage = dmg;
    }

    // damage player when hit
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerHealthController>() != null)
        {
            other.GetComponent<PlayerHealthController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}