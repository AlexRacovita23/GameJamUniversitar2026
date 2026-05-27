using UnityEngine;
using System.Collections;

public class RockImpostorBaker : MonoBehaviour
{
    [Header("References")]
    public Material rockMaterial;
    public RockMeshLibrary meshLibrary;

    [Header("Atlas Settings")]
    [Range(2, 8)] public int framesPerAxis = 4;
    [Range(32, 256)] public int frameResolution = 128;
    [Range(0, 31)] public int rockVariantIndex = 0;

    [Header("Bake Options")]
    public bool autoBakeOnStart = true;
    public bool logBakeTime = true;

    public Texture2D Atlas { get; private set; }
    public Material[] ImpostorMaterials { get; private set; } = new Material[3];
    public Material ImpostorMaterial => ImpostorMaterials[0];
    public bool IsBaked { get; private set; }

    public event System.Action OnBakeComplete;

    private const string BakeLayerName = "ImpostorBake";

    private void Start()
    {
        if (autoBakeOnStart)
            StartCoroutine(BakeNextFrame());
    }

    private IEnumerator BakeNextFrame()
    {
        yield return null;
        BakeAtlas();
    }

    private static void DilateAtlas(Texture2D tex, int passes)
    {
        Color[] pixels = tex.GetPixels();
        int w = tex.width, h = tex.height;
        Color[] buffer = new Color[pixels.Length];

        for (int p = 0; p < passes; p++)
        {
            System.Array.Copy(pixels, buffer, pixels.Length);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int i = y * w + x;
                    if (pixels[i].a > 0.01f) continue;

                    float rSum = 0, gSum = 0, bSum = 0;
                    int count = 0;
                    if (x > 0 && pixels[i - 1].a > 0.01f) { var c = pixels[i - 1]; rSum += c.r; gSum += c.g; bSum += c.b; count++; }
                    if (x < w - 1 && pixels[i + 1].a > 0.01f) { var c = pixels[i + 1]; rSum += c.r; gSum += c.g; bSum += c.b; count++; }
                    if (y > 0 && pixels[i - w].a > 0.01f) { var c = pixels[i - w]; rSum += c.r; gSum += c.g; bSum += c.b; count++; }
                    if (y < h - 1 && pixels[i + w].a > 0.01f) { var c = pixels[i + w]; rSum += c.r; gSum += c.g; bSum += c.b; count++; }

                    if (count > 0)
                        buffer[i] = new Color(rSum / count, gSum / count, bSum / count, 0f);
                }

            System.Array.Copy(buffer, pixels, pixels.Length);
        }

        tex.SetPixels(pixels);
    }
    public void BakeAtlas()
    {
        if (meshLibrary == null)
            meshLibrary = FindFirstObjectByType<RockMeshLibrary>();

        if (meshLibrary == null || meshLibrary.Meshes == null || meshLibrary.Meshes.Length == 0)
        {
            Debug.LogError("[ImpostorBaker] No RockMeshLibrary found. Aborting.");
            return;
        }

        float t0 = Time.realtimeSinceStartup;
        Mesh mesh = meshLibrary.GetByIndex(rockVariantIndex);

        int bakeLayer = LayerMask.NameToLayer(BakeLayerName);
        if (bakeLayer < 0)
        {
            Debug.LogWarning($"[ImpostorBaker] Layer '{BakeLayerName}' not found — " +
                             "bake camera will see the whole scene. " +
                             "Add it in Project Settings → Tags and Layers.");
            bakeLayer = 0;
        }

        int atlasSize = framesPerAxis * frameResolution;
        Vector3 origin = new Vector3(0f, -9000f, 0f);

        var rockGO = new GameObject("_BakeRock") { layer = bakeLayer };
        rockGO.transform.position = origin;
        rockGO.AddComponent<MeshFilter>().mesh = mesh;
        rockGO.AddComponent<MeshRenderer>().material =
            rockMaterial != null ? rockMaterial : new Material(Shader.Find("Standard"));

        var camGO = new GameObject("_BakeCam");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 1.2f;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 20f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;
        cam.enabled = false;
        cam.cullingMask = 1 << bakeLayer;

        var rt = new RenderTexture(frameResolution, frameResolution, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 2
        };
        cam.targetTexture = rt;

        Atlas = new Texture2D(atlasSize, atlasSize, TextureFormat.RGBA32, false);
        Atlas.name = "RockImpostorAtlas";

        var frame = new Texture2D(frameResolution, frameResolution, TextureFormat.RGBA32, false);

        for (int row = 0; row < framesPerAxis; row++)
        {
            for (int col = 0; col < framesPerAxis; col++)
            {
                float u = (col + 0.5f) / framesPerAxis;
                float v = (row + 0.5f) / framesPerAxis;
                float theta = u * Mathf.PI * 2f;
                float phi = v * Mathf.PI;

                Vector3 dir = new Vector3(
                    Mathf.Sin(phi) * Mathf.Cos(theta),
                    Mathf.Cos(phi),
                    Mathf.Sin(phi) * Mathf.Sin(theta)
                ).normalized;

                camGO.transform.position = origin + dir * 4f;
                camGO.transform.LookAt(origin);
                cam.Render();

                RenderTexture.active = rt;
                frame.ReadPixels(new Rect(0, 0, frameResolution, frameResolution), 0, 0);
                frame.Apply();

                int destRow = framesPerAxis - 1 - row;
                Atlas.SetPixels(col * frameResolution, destRow * frameResolution,
                                frameResolution, frameResolution, frame.GetPixels());
            }
        }
        Destroy(frame);

        RenderTexture.active = null;
        //DilateAtlas(Atlas, passes: 2);
        Atlas.Apply();

        DestroyImmediate(rockGO);
        DestroyImmediate(camGO);
        rt.Release();

        string[] shaderNames = {
            "ParticleEffects/ImpostorBillboard",
            "ParticleEffects/ImpostorBillboardBlendH",
            "ParticleEffects/ImpostorBillboardBlendFull"
        };

        for (int s = 0; s < shaderNames.Length; s++)
        {
            var shader = Shader.Find(shaderNames[s]);
            if (shader == null) { Debug.LogError($"[ImpostorBaker] Shader not found: {shaderNames[s]}"); continue; }

            ImpostorMaterials[s] = new Material(shader) { name = $"RockImpostorMat_{s}_Runtime" };
            ImpostorMaterials[s].mainTexture = Atlas;
            ImpostorMaterials[s].SetFloat("_FramesPerAxis", framesPerAxis);
        }

        IsBaked = true;

        if (logBakeTime)
            Debug.Log($"[ImpostorBaker] Bake complete — " +
                      $"{atlasSize}x{atlasSize}px, {framesPerAxis}x{framesPerAxis} frames, " +
                      $"{(Time.realtimeSinceStartup - t0) * 1000f:F1}ms");

        OnBakeComplete?.Invoke();
    }
}