using UnityEngine;

public class RockMeshLibrary : MonoBehaviour
{
    [Header("Pool Settings")]
    [Range(4, 32)] public int rockVariants = 8;

    [Header("Shape Parameters")]
    [Range(0.05f, 0.5f)] public float minRoughness = 0.15f;
    [Range(0.05f, 0.5f)] public float maxRoughness = 0.35f;
    [Range(0.4f, 1.5f)] public float minSquash = 0.6f;
    [Range(0.4f, 1.5f)] public float maxSquash = 1.1f;
    [Range(0, 2)] public int subdivisions = 0;

    [Header("Debug")]
    public bool logGenerationTime = true;

    public Mesh[] Meshes { get; private set; }

    public static RockMeshLibrary Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        GenerateLibrary();
    }

    private void GenerateLibrary()
    {
        float t0 = Time.realtimeSinceStartup;
        Meshes = new Mesh[rockVariants];

        for (int i = 0; i < rockVariants; i++)
        {
            Meshes[i] = ProceduralRockMesh.Generate(
                seed: i * 1337,
                roughness: Random.Range(minRoughness, maxRoughness),
                squash: Random.Range(minSquash, maxSquash),
                subdivisions: subdivisions
            );
            Meshes[i].name = $"ProceduralRock_{i}";
        }

        if (logGenerationTime)
            Debug.Log($"[RockMeshLibrary] Generated {rockVariants} meshes in " +
                      $"{(Time.realtimeSinceStartup - t0) * 1000f:F1}ms");
    }

    public Mesh GetRandom() => Meshes[Random.Range(0, Meshes.Length)];
    public Mesh GetByIndex(int index) => Meshes[index % Meshes.Length];
}