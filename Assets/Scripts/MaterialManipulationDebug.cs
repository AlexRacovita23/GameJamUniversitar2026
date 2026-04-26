using System.Collections.Generic;
using UnityEngine;

public class MaterialManipulationDebug : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alfa = 1f;

    private readonly List<Material> instanceMaterials = new List<Material>();
    private Renderer[] childRenderers;

    private void Awake()
    {
        childRenderers = GetComponents<Renderer>();

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
    }

    private void Update()
    {
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
