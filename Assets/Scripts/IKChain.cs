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
    private List<Vector3> positions = new List<Vector3>(); // posiciones "virtuales" de cada joint
    float totalLength;

    void Start()
    {
        Initialize();
    }

    void Initialize()
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
        if (joints.Count < 2 || target == null) return;

        if (modo == ModoIK.FABRIK)
            SolveFABRIK();
        else
            SolveRotar();

        ApplyPositionsAndRotations();
    }

    // --- Igual que tu script original (y el ejemplo de Paper.js) ---
    void SolveFABRIK()
    {
        positions[0] = ancla != null ? ancla.position : positions[0];
        Vector3 anclaPos = positions[0];

        for (int iter = 0; iter < iterations; iter++)
        {
            // Reach forward (hacia el target)
            positions[joints.Count - 1] = target.position;
            for (int i = joints.Count - 2; i >= 0; i--)
                positions[i] = Constrain(positions[i], positions[i + 1], boneLengths[i]);

            // Reach backward (vuelve al ancla)
            positions[0] = anclaPos;
            for (int i = 1; i < joints.Count; i++)
                positions[i] = Constrain(positions[i], positions[i - 1], boneLengths[i - 1]);
        }
    }

    // --- Modo alternativo: solo rota cada segmento hacia el siguiente, sin IK real ---
    // Útil para colas/tentáculos simples que "persiguen" al de adelante.
    void SolveRotar()
    {
        positions[0] = target.position; // la cabeza sigue al target directamente

        for (int i = 1; i < joints.Count; i++)
        {
            Vector3 dir = (positions[i] - positions[i - 1]).normalized;
            positions[i] = positions[i - 1] + dir * boneLengths[i - 1];
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