using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AirSteering : MonoBehaviour
{
    public Transform target;
    public float maxSpeed = 5f;
    public float steeringStrength = 3f;
    public float rotationSpeed = 5f;
    public float rotationOffset = 0f;
    public LayerMask obstacleLayers;

    [Header("Vision")]
    public float visionRange = 10f;

    [Header("Raycasts")]
    public float rayLength = 3f;
    [Header("Límites del mapa")]
    public float yMin = -3f;
    public float yMax = 20f;
    public float xMin = -50f;
    public float xMax = 50f;

    [Header("Patrulla")]
    public float patrolRadius = 6f;
    public float patrolStuckTimeout = 2f;
    public float patrolChangeInterval = 4f; // regenera destino cada X segundos aunque no se atasque

    [Header("Estados")]
    public float lostSightTimeout = 0.1f;
    public float searchTimeout = 6f;

    [Header("Colisión con player")]
    public float hitSpeedMultiplier = 1.5f;  // velocidad al estar cerca del player
    public float passThruDuration = 0.8f;    // segundos que pasa de largo tras golpear
    public float stopRadius = 1.5f;

    enum State { Chase, Search, Patrol, Charge }
    State state = State.Patrol;

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector2[] rays = new Vector2[16];

    private Vector2 heading = Vector2.right;
    private float lostSightTimer;
    private float searchTimer;
    private float patrolStuckTimer;
    private float patrolChangeTimer;
    private float chargeTimer;
    private Vector2 lastPos;
    private Vector2 lastKnownPlayerPos;
    private Vector2 patrolDestination;
    private Vector2 chargeDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        for (int i = 0; i < 16; i++)
        {
            float a = i * 22.5f * Mathf.Deg2Rad;
            rays[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }

        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }

        lastPos = transform.position;
        NewPatrolDestination();
    }

    void FixedUpdate()
    {
        UpdateState();

        Vector2 destination;
        float speed;

        switch (state)
        {
            case State.Chase:
                destination = target.position;
                float distToPlayer = Vector2.Distance(transform.position, destination);
                // Acelera al acercarse en vez de frenar
                speed = distToPlayer < stopRadius
                    ? maxSpeed * hitSpeedMultiplier
                    : maxSpeed;
                break;

            case State.Search:
                destination = lastKnownPlayerPos;
                speed = maxSpeed;
                break;

            case State.Charge:
                destination = (Vector2)transform.position + chargeDirection * 5f;
                speed = maxSpeed; // velocidad normal al pasar de largo
                break;

            default: // Patrol
                destination = patrolDestination;
                speed = maxSpeed;
                break;
        }

        Vector2 desired = Steer(destination) * speed;
        velocity = Vector2.Lerp(velocity, desired, steeringStrength * Time.fixedDeltaTime);
        rb.velocity = velocity;

        RotateTowardsMovement();
    }

    void UpdateState()
    {
        float moved = Vector2.Distance(transform.position, lastPos);
        lastPos = transform.position;
        bool sees = SeesPlayer();

        if (sees) lastKnownPlayerPos = target.position;

        switch (state)
        {
            case State.Chase:
                if (sees) { lostSightTimer = 0f; break; }
                lostSightTimer += Time.fixedDeltaTime;
                if (lostSightTimer >= lostSightTimeout)
                {
                    lostSightTimer = 0f;
                    searchTimer = 0f;
                    state = State.Search;
                    Debug.Log($"[{name}] SEARCH");
                }
                break;

            case State.Search:
                if (sees) { state = State.Chase; Debug.Log($"[{name}] CHASE"); break; }
                searchTimer += Time.fixedDeltaTime;
                if (Vector2.Distance(transform.position, lastKnownPlayerPos) < stopRadius || searchTimer >= searchTimeout)
                {
                    NewPatrolDestination();
                    searchTimer = 0f;
                    state = State.Patrol;
                    Debug.Log($"[{name}] PATROL");
                }
                break;

            case State.Patrol:
                if (sees) { state = State.Chase; Debug.Log($"[{name}] CHASE"); break; }

                patrolStuckTimer += moved < 0.05f ? Time.fixedDeltaTime : 0f;
                patrolChangeTimer += Time.fixedDeltaTime;

                bool stuck = patrolStuckTimer >= patrolStuckTimeout;
                bool timeout = patrolChangeTimer >= patrolChangeInterval;
                bool arrived = Vector2.Distance(transform.position, patrolDestination) < stopRadius;

                if (stuck || timeout || arrived)
                {
                    NewPatrolDestination();
                    patrolStuckTimer = 0f;
                    patrolChangeTimer = 0f;
                }
                break;

            case State.Charge:
                chargeTimer += Time.fixedDeltaTime;
                if (chargeTimer >= passThruDuration)
                {
                    chargeTimer = 0f;
                    if (sees) { state = State.Chase; Debug.Log($"[{name}] CHASE"); }
                    else { NewPatrolDestination(); state = State.Patrol; Debug.Log($"[{name}] PATROL"); }
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Contacto físico con player como respaldo por si no se activó por distancia
        if (!other.CompareTag("Player") || state == State.Charge) return;
        chargeDirection = velocity.magnitude > 0.1f ? velocity.normalized : (Vector2)transform.right;
        chargeTimer = 0f;
        state = State.Charge;
        Debug.Log($"[{name}] CHARGE");
    }

    Vector2 Steer(Vector2 destination)
    {
        destination.x = Mathf.Clamp(destination.x, xMin, xMax);
        destination.y = Mathf.Clamp(destination.y, yMin, yMax);

        Vector2 toDest = (destination - (Vector2)transform.position).normalized;

        // Detectar obstáculo más cercano en todas las direcciones
        float minObstacleDist = rayLength;
        Vector2 avoidDir = Vector2.zero;

        for (int i = 0; i < 16; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rays[i], rayLength, obstacleLayers);
            Debug.DrawRay(transform.position, rays[i] * rayLength, hit.collider != null ? Color.red : Color.green);
            if (hit.collider != null && hit.distance < minObstacleDist)
            {
                minObstacleDist = hit.distance;
                avoidDir = -rays[i]; // dirección opuesta al obstáculo más cercano
            }
        }

        // Urgencia: 0 = sin obstáculos, 1 = pared muy cerca
        float urgency = minObstacleDist < rayLength ? 1f - (minObstacleDist / rayLength) : 0f;

        // Dirección deseada: mezcla entre ir al destino y alejarse del obstáculo
        Vector2 desiredDir = Vector2.Lerp(toDest, avoidDir, urgency * 0.8f).normalized;

        // Velocidad angular: baja en espacio abierto, alta cerca de paredes
        float baseTurnSpeed = 90f;
        float maxTurnSpeed = 400f;
        float turnSpeed = Mathf.Lerp(baseTurnSpeed, maxTurnSpeed, urgency);

        // Rotar heading suavemente hacia la dirección deseada
        float currentAngle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
        float targetAngle = Mathf.Atan2(desiredDir.y, desiredDir.x) * Mathf.Rad2Deg;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeed * Time.fixedDeltaTime);
        heading = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));

        return heading;
    }

    bool SeesPlayer()
    {
        if (target == null) return false;
        Vector2 toPlayer = (Vector2)target.position - (Vector2)transform.position;
        float dist = toPlayer.magnitude;
        if (dist > visionRange) return false;
        Debug.DrawRay(transform.position, toPlayer, Color.magenta);
        return Physics2D.Raycast(transform.position, toPlayer.normalized, dist, obstacleLayers).collider == null;
    }

    void NewPatrolDestination()
    {
        // Punto aleatorio dentro de los límites del mapa
        float x = Random.Range(xMin + 2f, xMax - 2f);
        float y = Random.Range(yMin + 2f, yMax - 2f);
        patrolDestination = new Vector2(x, y);
    }

    void RotateTowardsMovement()
    {
        if (velocity.magnitude < 0.1f) return;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + rotationOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpAngle(transform.eulerAngles.z, angle, rotationSpeed * Time.fixedDeltaTime));
    }

    public void SetTarget(Transform t) => target = t;

    void OnDrawGizmosSelected()
    {
        // Visualiza el radio de detección del player (debe coincidir con el Circle Collider 2D)
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}