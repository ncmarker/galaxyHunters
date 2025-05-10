using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class RollingBall : MonoBehaviour
{
    private float speed = 8f;   
    private float rotationSpeed = 90f; 
    private int mDamage;
    private Vector2 mDirection;

    public void SetDamage(int damage) { mDamage = damage; }
    public void SetDirection(Vector2 direction) { mDirection = direction; }

    void Update()
    {
        // rotate
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        
        // move manually at constant speed
        transform.position += (Vector3)(mDirection * speed * Time.deltaTime);

        // deflect off screen bounds
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        bool bounced = false;

        if (viewPos.x < 0f)
        {
            mDirection.x *= -1;
            bounced = true;
            viewPos.x = 0f;
            viewPos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;
            transform.position = Camera.main.ViewportToWorldPoint(viewPos);
        }
        else if (viewPos.x > 1f)
        {
            mDirection.x *= -1;
            bounced = true;
            viewPos.x = 1f;
            viewPos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;
            transform.position = Camera.main.ViewportToWorldPoint(viewPos);
        }

        if (viewPos.y < 0f)
        {
            mDirection.y *= -1;
            bounced = true;
            viewPos.y = 0f;
            viewPos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;
            transform.position = Camera.main.ViewportToWorldPoint(viewPos);
        }
        else if (viewPos.y > 1f)
        {
            mDirection.y *= -1;
            bounced = true;
            viewPos.y = 1f;
            viewPos.z = Mathf.Abs(Camera.main.transform.position.z) - 1.0f;
            transform.position = Camera.main.ViewportToWorldPoint(viewPos);
        }


        if (bounced)
        {
            mDirection.Normalize();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.GetComponent<Enemy>() != null) 
        {
            Enemy otherEnemy = other.GetComponent<Enemy>();
            otherEnemy.TakeDamage(mDamage);
        }
    }
}
