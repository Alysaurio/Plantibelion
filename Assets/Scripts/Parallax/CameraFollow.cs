using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;
    void Start()
    {

    }
    void Update()
    {
        Vector3 targetPos = target.transform.position;
        targetPos.z = -10;

        transform.position = Vector3.Lerp
            (transform.position,
            targetPos,
            followSpeed * Time.deltaTime);
    }
}