using System.Collections.Generic;
using UnityEngine;

public class MaterialManipulation : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alfa = 1f;
    [SerializeField] private float transparencyThreshold = 10f;

    private MandrakeIllusion illusionScript;
    private Collider objectCollider;
    private bool isCollisionEnabled = true;

    private readonly List<Material> instanceMaterials = new List<Material>();
    private Renderer[] childRenderers;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
            objectCollider.enabled = isCollisionEnabled;

        childRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in childRenderers)
        {
            Material[] mats = renderer.materials; // creates per-renderer instances
            foreach (Material mat in mats)
            {
                if (mat != null && !instanceMaterials.Contains(mat))
                {
                    instanceMaterials.Add(mat);
                }
            }
        }
    }

    private void Start()
    {
        illusionScript = GetComponent<MandrakeIllusion>();
    }

    private void Update()
    {
        float timeToLive = illusionScript != null ? illusionScript.timeToDisappear : 0f;

        if (timeToLive < transparencyThreshold)
        {
            alfa = Mathf.Lerp(0f, 1f, timeToLive / transparencyThreshold);
            objectCollider.enabled = timeToLive > 1f; // Disable collider when almost invisible
        }
        else
            alfa = 1f;

        ApplyAlpha(alfa);
    }

    private void ApplyAlpha(float alpha)
    {
        foreach (Material mat in instanceMaterials)
        {
            if (mat == null)
                continue;

            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }
}
