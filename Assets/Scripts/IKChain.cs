using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKChain : MonoBehaviour
{
    [Header("Chain")]
    public List<Transform> joints = new List<Transform>();

    [Header("Targets")]
    public Transform ancla;
    public Transform target;

    [Header("Settings")]
    public int iterations = 3;
    public float snapDistance = 0.01f;
    public float angleOffset = -90f;

    private List<float> boneLengths = new List<float>();

    float totalLength;

    void Start()
    {
        Initialize();
    }

    void LateUpdate()
    {
        if (joints.Count < 2) return;
        SolveFABRIK();
        RotateBones();
    }

    void Initialize()
    {
        boneLengths.Clear();
        totalLength = 0;
        for (int i = 0; i < joints.Count - 1; i++)
        {
            float length = Vector3.Distance(joints[i].position, joints[i + 1].position);
            boneLengths.Add(length);
            totalLength += length;
        }
    }

    void SolveFABRIK()
    {
        if (ancla == null || target == null) return;
        Vector3 targetPosition = target.position;

        float targetDistance = Vector3.Distance(ancla.position, targetPosition);
        // Target unreachable
        if (targetDistance > totalLength)
        {
            Vector3 dir = (ancla.position - targetPosition).normalized;
            joints[0].position = ancla.position;
            for (int i = 1; i < joints.Count; i++)
            {
                joints[i].position = joints[i - 1].position - dir * boneLengths[i - 1];
            }
            return;
        }

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            // FORWARD
            joints[0].position = ancla.position;
            for (int i = 1; i < joints.Count; i++)
            {
                joints[i].position = ConstraintDistance(
                    joints[i].position,
                    joints[i - 1].position,
                    boneLengths[i - 1]
                );
            }

            // BACKWARD
            joints[joints.Count - 1].position = targetPosition;
            for (int i = joints.Count - 2; i >= 0; i--)
            {
                joints[i].position = ConstraintDistance(
                    joints[i].position,
                    joints[i + 1].position,
                    boneLengths[i]
                );
            }

            if (Vector3.Distance(joints[0].position, ancla.position) < snapDistance)
            {
                break;
            }
        }
    }

    void RotateBones()
    {
        for (int i = 0; i < joints.Count - 1; i++)
        {
            Vector3 dir = joints[i + 1].position - joints[i].position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            joints[i].rotation = Quaternion.Euler(0, 0, angle + angleOffset);
        }
    }

    Vector3 ConstraintDistance(Vector3 point, Vector3 anchor, float distance)
    {
        return ((point - anchor).normalized * distance) + anchor;
    }
}
