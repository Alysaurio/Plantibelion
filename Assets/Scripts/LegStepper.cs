using System.Collections;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El cuerpo del personaje (origen del raycast)")]
    public Transform player;
    [Tooltip("Punto hijo del Player que solo marca la DIRECCIÓN en la que apunta esta pata")]
    public Transform home;
    [Tooltip("El Transform que tu IK usa como target real. NO debe ser hijo del Player")]
    public Transform target;

    [Header("Raycast")]
    public LayerMask groundMask;
    [Tooltip("Longitud del rayo desde el Player hacia Home")]
    public float rayLength = 1.5f;

    [Header("Paso")]
    [Tooltip("Distancia máxima antes de disparar un nuevo paso (o de retraer hacia Home si no hay suelo)")]
    public float maxDistance = 0.6f;
    public float arcHeight = 0.3f;
    public float stepDuration = 0.15f;

    private bool isMoving = false;

    void Start()
    {
        if (target != null && home != null)
            target.position = home.position;
    }

    void Update()
    {
        if (isMoving || player == null || home == null || target == null) return;

        Vector2 dir = (home.position - player.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(player.position, dir, rayLength, groundMask);

        if (hit.collider != null)
        {
            // Hay suelo: si el punto de choque está lejos de donde pisa ahora, da un paso
            float dist = Vector3.Distance(hit.point, target.position);
            if (dist > maxDistance)
                StartCoroutine(Step(hit.point));
        }
        else
        {
            // No hay suelo en el rango: si el pie quedó muy lejos del Player, lo retrae hacia Home (sin arco, para no brincar sobre Home)
            float distToPlayer = Vector3.Distance(target.position, player.position);
            if (distToPlayer > maxDistance)
                StartCoroutine(Step(home.position, useArc: false));
        }
    }

    IEnumerator Step(Vector3 destination, bool useArc = true)
    {
        isMoving = true;

        Vector3 start = target.position;
        float t = 0f;
        float heightToUse = useArc ? arcHeight : 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / stepDuration;
            float clamped = Mathf.Clamp01(t);
            Vector3 flat = Vector3.Lerp(start, destination, clamped);
            float arc = Mathf.Sin(clamped * Mathf.PI) * heightToUse;
            target.position = flat + Vector3.up * arc;
            yield return null;
        }

        target.position = destination;
        isMoving = false;
    }

    void OnDrawGizmos()
    {
        if (player == null || home == null) return;

        Vector2 dir = (home.position - player.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(player.position, dir, rayLength, groundMask);

        // Rayo Player -> Home: verde si golpea suelo, rojo si no
        Gizmos.color = hit.collider != null ? Color.green : Color.red;
        Gizmos.DrawLine(player.position, player.position + (Vector3)dir * rayLength);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(home.position, 0.08f); // Home (dirección)

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target.position, 0.07f); // posición actual del pie
        }
    }
}