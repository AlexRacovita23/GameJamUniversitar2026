using System.Collections.Generic;
using UnityEngine;

public class PictogramMaterialManipulation : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alfa = 0f;

    [Header("Stress values")]
    [SerializeField] private float alfaMinIncreaseRate = 0.05f;
    [SerializeField] private float alfaMaxIncreaseRate = 0.1f;
    [SerializeField] private float stressMinVal = 20f;
    [SerializeField] private float stressMaxVal = 50f;

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
        alfa = 0f; // Start fully transparent
    }

    private void Update()
    {
        if (alfa < 1f)
        {
            float stress = SanitySystem.Instance.Sanity;

            if (stress >= stressMinVal && stress <= stressMaxVal)
            {
                float t = Mathf.InverseLerp(stressMinVal, stressMaxVal, stress);
                float alfaIncreaseRate = Mathf.Lerp(alfaMinIncreaseRate, alfaMaxIncreaseRate, t);
                alfa += alfaIncreaseRate * Time.deltaTime;
                alfa = Mathf.Clamp(alfa, 0f, 1f);
            }

            ApplyAlpha(alfa);
        }
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
