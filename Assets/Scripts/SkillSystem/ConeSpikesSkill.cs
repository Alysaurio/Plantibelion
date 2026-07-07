using UnityEngine;
using UnityEngine.InputSystem;

// Dispara varias espinas repartidas en abanico, centradas en la dirección
// del mouse respecto al owner. Sigue el mismo patrón que ShockWaveSkill:
// hereda de BaseSkill y usa skillData para Damage/Speed/Range/Knockback.
public class ConeSpikesSkill : BaseSkill
{
    [Header("Prefab de cada espina individual")]
    public GameObject spikePrefab; // el prefab con el componente SpikeProjectile (NO el mismo que skillData.skillPrefab)

    [Header("Configuración del cono")]
    public int spikeCount = 3;
    public float angleStep = 20f;   // separación en grados entre espinas consecutivas
    public float angleJitter = 3f;  // aleatoriedad extra (+-) que se suma a cada espina

    public override void Activate()
    {
        if (skillData == null || spikePrefab == null)
        {
            Debug.LogWarning("[ConeSpikesSkill] Falta skillData o spikePrefab.");
            return;
        }

        Vector2 originPos = owner.transform.position;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 baseDir = (mouseWorldPos - originPos).normalized;

        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        // Con spikeCount = 3 y angleStep = 20, los offsets quedan en -20, 0, +20.
        // Con spikeCount par también reparte simétricamente alrededor del centro.
        float half = (spikeCount - 1) / 2f;

        for (int i = 0; i < spikeCount; i++)
        {
            float offset = (i - half) * angleStep + Random.Range(-angleJitter, angleJitter);
            float angle = (baseAngle + offset) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject obj = Instantiate(spikePrefab, originPos, Quaternion.identity);

            if (obj.TryGetComponent<SpikeProjectile>(out var spike))
            {
                spike.Initialize(skillData, owner, dir);
            }
            else
            {
                Debug.LogWarning("[ConeSpikesSkill] El prefab asignado no tiene el componente SpikeProjectile.");
            }
        }
    }
}