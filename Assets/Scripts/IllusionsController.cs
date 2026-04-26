using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IllusionsController : MonoBehaviour
{
    public static IllusionsController Instance { get; private set; }
    public GameObject mandrakePrefab;
    public Transform player;

    [Header("Time to live")]
    [SerializeField] private float minTimeToLive = 15f;
    [SerializeField] private float maxTimeToLive = 30f;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnTime = 2f;
    [SerializeField] private float maxSpawnTime = 6f;

    [Header("Spawn Area")]
    [SerializeField] private float minSpawnRadius = 10f;
    [SerializeField] private float maxSpawnRadius = 20f;

    [Header("Limits")]
    [SerializeField] private int maxNPCs = 10;

    [Header("NavMesh")]
    [SerializeField] private bool useNavMesh = true;
    [SerializeField] private float navMeshSampleDistance = 5f;

    [SerializeField] private AnimationCurve sanityToSpawnChance;

    private int currentNPCCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (player == null || mandrakePrefab == null)
                continue;

            if (currentNPCCount >= maxNPCs)
                continue;

            float sanity01 = Mathf.Clamp01(SanitySystem.Instance.Sanity / 100f);
            float spawnChance = sanityToSpawnChance.Evaluate(1f - sanity01) * 100f;

            if (Random.Range(0f, 100f) < spawnChance)
            {
                TrySpawnNPC();
            }
        }
    }

    private void TrySpawnNPC()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 rawSpawnPosition = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        Vector3 finalSpawnPosition = rawSpawnPosition;

        if (useNavMesh)
        {
            if (!NavMesh.SamplePosition(rawSpawnPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
                return;

            finalSpawnPosition = hit.position;
        }

        GameObject npc = Instantiate(mandrakePrefab, finalSpawnPosition, Quaternion.identity);
        currentNPCCount++;

        MandrakeIllusion spawned = npc.GetComponent<MandrakeIllusion>();
        spawned.timeToDisappear = Random.Range(minTimeToLive, maxTimeToLive);
    }

    public void NotifyNPCDestroyed()
    {
        currentNPCCount = Mathf.Max(0, currentNPCCount - 1);
    }
}
