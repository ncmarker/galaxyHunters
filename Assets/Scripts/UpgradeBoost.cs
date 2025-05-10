using UnityEngine;

public class UpgradeBoost : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            XPManager.Instance.MaxOutXP();
            Destroy(gameObject);
        }
    }
}
