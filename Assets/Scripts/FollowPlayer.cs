using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    public void Initialize(Transform player, Vector3 directionOffset)
    {
        target = player;
        offset = directionOffset;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
