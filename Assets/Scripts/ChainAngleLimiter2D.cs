using UnityEngine;
using UnityEngine.U2D.IK;

[DefaultExecutionOrder(1000)] // se ejecuta después de IKManager2D/FabrikSolver2D
public class ChainAngleLimiter2D : MonoBehaviour
{
    [Tooltip("Arrastra aquí el mismo componente FabrikSolver2D que usas")]
    public Solver2D solver;

    [Header("Límites de ángulo (grados), relativos a la pose de reposo")]
    [Tooltip("Ángulo mínimo permitido (puede ser negativo)")]
    public float minAngle = -45f;

    [Tooltip("Ángulo máximo permitido")]
    public float maxAngle = 45f;

    Transform[] m_Bones;
    Quaternion[] m_RestLocalRotations;

    void Start()
    {
        CacheRestPose();
    }

    void OnValidate()
    {
        // Evita que minAngle quede por encima de maxAngle en el Inspector
        if (minAngle > maxAngle)
            minAngle = maxAngle;
    }

    void CacheRestPose()
    {
        var chain = solver.GetChain(0);
        int count = chain.transformCount;
        m_Bones = new Transform[count];
        m_RestLocalRotations = new Quaternion[count];

        for (int i = 0; i < count; i++)
        {
            m_Bones[i] = chain.transforms[i];
            m_RestLocalRotations[i] = m_Bones[i].localRotation;
        }
    }

    void LateUpdate()
    {
        if (m_Bones == null) return;

        for (int i = 0; i < m_Bones.Length - 1; i++)
        {
            Transform bone = m_Bones[i];
            Quaternion rest = m_RestLocalRotations[i];

            Quaternion delta = Quaternion.Inverse(rest) * bone.localRotation;
            delta.ToAngleAxis(out float angle, out Vector3 axis);
            if (axis.z < 0) angle = -angle;
            angle = Mathf.DeltaAngle(0f, angle); // normaliza a [-180, 180]

            if (angle < minAngle || angle > maxAngle)
            {
                float clamped = Mathf.Clamp(angle, minAngle, maxAngle);
                bone.localRotation = rest * Quaternion.AngleAxis(clamped, Vector3.forward);
            }
        }
    }
}