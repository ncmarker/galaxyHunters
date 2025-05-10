using UnityEngine;

public class Sword : MonoBehaviour
{
    private int mDamage;

    public void SetDamage(int damage) { mDamage = damage; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.GetComponent<Enemy>() != null) 
        {
            Enemy otherEnemy = other.GetComponent<Enemy>();
            otherEnemy.TakeDamage(mDamage);
        }
    }
}
