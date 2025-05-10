using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboLaser : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    private float duration = 20f;
    private int damage = 5;
    private float hitCooldown = 0.5f;

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        lineRenderer.sortingLayerName = "Default"; 
        lineRenderer.sortingOrder = 0;

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;

        edgeCollider.isTrigger = true;

        StartCoroutine(LaserRoutine());
        SoundManager.Instance.PlaySound("comboLaserBeam");
    }

    IEnumerator LaserRoutine()
    {
        float timer = 0f;

        while (timer < duration && player1 != null && player2 != null)
        {
            Vector3 start = player1.position;
            Vector3 end = player2.position;
            start.z = -0.5f;
            end.z = -0.5f;

            // lpdate line renderer
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // update edge collider
            Vector2[] points = new Vector2[2];
            points[0] = player1.position;
            points[1] = player2.position;
            edgeCollider.points = points;

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!lastHitTime.ContainsKey(other.gameObject) || Time.time - lastHitTime[other.gameObject] >= hitCooldown)
            {
                lastHitTime[other.gameObject] = Time.time;

                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    SoundManager.Instance.PlaySound("comboLaserBeamZap");
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
}
