using System.Collections.Generic;
using UnityEngine;

public class IKChain : MonoBehaviour
{
    public enum ModoIK { FABRIK, Rotar }
    public ModoIK modo = ModoIK.FABRIK;
    public List<Transform> joints = new List<Transform>();
    public Transform ancla;
    public Transform target;
    public int iterations = 3;
    public float angleOffset = -90f;

    private List<float> boneLengths = new List<float>();
    private List<Vector3> positions = new List<Vector3>();
    float totalLength;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        boneLengths.Clear();
        positions.Clear();
        totalLength = 0;

        for (int i = 0; i < joints.Count; i++)
            positions.Add(joints[i].position);

        for (int i = 0; i < joints.Count - 1; i++)
        {
            float len = Vector3.Distance(joints[i].position, joints[i + 1].position);
            boneLengths.Add(len);
            totalLength += len;
        }
    }

    void LateUpdate()
    {
        if (joints.Count < 2) return;

        if (modo == ModoIK.FABRIK)
        {
            if (target == null) return; // FABRIK sí necesita target
            SolveFABRIK();
        }
        else
        {
            SolveRotar(); // Rotar usa joints[0] directamente, no necesita target
        }

        ApplyPositionsAndRotations();
    }

    void SolveFABRIK()
    {
        positions[0] = ancla != null ? ancla.position : positions[0];
        Vector3 anclaPos = positions[0];

        for (int iter = 0; iter < iterations; iter++)
        {
            positions[joints.Count - 1] = target.position;
            for (int i = joints.Count - 2; i >= 0; i--)
                positions[i] = Constrain(positions[i], positions[i + 1], boneLengths[i]);

            positions[0] = anclaPos;
            for (int i = 1; i < joints.Count; i++)
                positions[i] = Constrain(positions[i], positions[i - 1], boneLengths[i - 1]);
        }
    }

    void SolveRotar()
    {
        // La cabeza (joint[0]) ya fue movida por AirSteering, solo leemos su posición
        positions[0] = joints[0].position;

        for (int i = 1; i < joints.Count - 1; i++)
        {
            Vector3 dir = (positions[i] - positions[i - 1]).normalized;
            positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
        }

        if (joints.Count >= 2)
        {
            Vector3 dir = (positions[joints.Count - 1] - positions[joints.Count - 2]).normalized;
            positions[joints.Count - 1] = positions[joints.Count - 2] + dir * boneLengths[boneLengths.Count - 1];
        }
    }

    void ApplyPositionsAndRotations()
    {
        for (int i = 0; i < joints.Count; i++)
            joints[i].position = positions[i];

        for (int i = 0; i < joints.Count - 1; i++)
        {
            Vector3 dir = positions[i + 1] - positions[i];
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            joints[i].rotation = Quaternion.Euler(0, 0, angle + angleOffset);
        }

        if (joints.Count >= 2)
            joints[joints.Count - 1].rotation = joints[joints.Count - 2].rotation;
    }

    Vector3 Constrain(Vector3 point, Vector3 anchor, float distance)
    {
        return (point - anchor).normalized * distance + anchor;
    }
}