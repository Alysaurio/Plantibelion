using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        if (target == null) return;

        transform.position = target.position;
    }
}