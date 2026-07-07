using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        public bool isAerial;
    }

    [Header("Enemigos")]
    public List<EnemyEntry> enemies = new();
    public int maxEnemies = 10;

    [Header("Rango del mapa (eje Y)")]
    public float yMin = -3f;
    public float yMax = 20f;

    [Header("Tasa de spawn (enemigos por minuto)")]
    public float spawnRateMin = 1f;  
    public float spawnRateMax = 5f;  

    [Header("Spawn")]
    public float spawnDistance = 15f;  
    public float overlapCheckRadius = 1f; // radio para verificar que no haya colliders
    public LayerMask overlapLayers;    // paredes

    private Transform player;
    private float spawnTimer = 0f;
    private List<GameObject> activeEnemies = new();

    void Awake()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        activeEnemies.RemoveAll(e => e == null);

        if (activeEnemies.Count >= maxEnemies) return;

        float t = Mathf.InverseLerp(yMin, yMax, player.position.y);
        float spawnRate = Mathf.Lerp(spawnRateMin, spawnRateMax, t);
        float interval = 60f / spawnRate; // segundos entre spawns

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= interval)
        {
            TrySpawn(t);
            spawnTimer = 0f;
        }
    }

    void TrySpawn(float heightT)
    {
        var candidates = new List<(EnemyEntry entry, float weight)>();

        foreach (var e in enemies)
        {
            float weight = e.isAerial ? heightT : (1f - heightT);
            weight = Mathf.Clamp01(weight);
            if (weight > 0f) candidates.Add((e, weight));
        }

        if (candidates.Count == 0) return;

        // Elegir enemigo por peso
        EnemyEntry chosen = PickWeighted(candidates);
        if (chosen == null) return;

        // Buscar posición de spawn fuera de pantalla
        Vector2 spawnPos = GetSpawnPosition();
        if (spawnPos == Vector2.zero) return;

        GameObject obj = Instantiate(chosen.prefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(obj);
    }

    EnemyEntry PickWeighted(List<(EnemyEntry entry, float weight)> candidates)
    {
        float total = 0f;
        foreach (var c in candidates) total += c.weight;

        float roll = Random.Range(0f, total);
        float accumulated = 0f;

        foreach (var c in candidates)
        {
            accumulated += c.weight;
            if (roll <= accumulated) return c.entry;
        }

        return candidates[candidates.Count - 1].entry;
    }

    Vector2 GetSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * spawnDistance;
            Vector2 candidate = (Vector2)player.position + offset;
            candidate.y = Mathf.Clamp(candidate.y, yMin, yMax);
            // Verificar
            if (Physics2D.OverlapCircle(candidate, overlapCheckRadius, overlapLayers) == null)
                return candidate;
        }

        return Vector2.zero; // no era posicion libre
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(player.position, spawnDistance);
    }
}