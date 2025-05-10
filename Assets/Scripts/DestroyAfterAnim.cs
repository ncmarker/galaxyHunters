using UnityEngine;

// class that will destroy an item after its animation completes 
public class DestroyAfterAnimation : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            Debug.LogWarning("No Animator found on flash effect. Destroying after 1 second as fallback.");
            Destroy(gameObject, 1f); 
        }
    }
}
