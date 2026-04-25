using UnityEngine;

public class MaterialManipulation : MonoBehaviour
{
    public Material material;
    [Range(0f, 1f)]
    public float alfa = 1f;

    // Update is called once per frame
    void Update()
    {
        material.color = new Color(material.color.r, material.color.g, material.color.b, alfa);
    }
}
