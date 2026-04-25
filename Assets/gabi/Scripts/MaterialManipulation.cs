using System.Collections.Generic;
using UnityEngine;

public class MaterialManipulation : MonoBehaviour
{
    public List<Material> materials;
    [Range(0f, 1f)]
    public float alfa = 0f;

    [Header("Stress values")]
    [SerializeField] private float alfaIncreaseRate = 0.05f;
    [SerializeField] private float stressMinVal = 20f;
    [SerializeField] private float stressMaxVal = 50f;

    private Collider objectCollider;
    private bool isCollisionEnabled = false;

    private void Awake()
    {
        objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
            objectCollider.enabled = isCollisionEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (alfa < 1f)
        {
            float stress = SanitySystem.Instance.Sanity;

            if (stress >= stressMinVal && stress <= stressMaxVal)
            {
                alfa += alfaIncreaseRate * Time.deltaTime;
                alfa = Mathf.Clamp(alfa, 0f, 1f);
            }

            foreach (Material material in materials)
                material.color = new Color(material.color.r, material.color.g, material.color.b, alfa);
        }
        else if (!isCollisionEnabled)
        {
            isCollisionEnabled = true;
            if (objectCollider != null)
                objectCollider.enabled = true;
        }
    }
}
