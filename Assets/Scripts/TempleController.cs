using System;
using UnityEngine;

public class TempleController : MonoBehaviour
{
    [Header("Stress values")]
    [SerializeField] private float riseSpeed = 0.1f;
    [SerializeField] private float stressMinVal = 20f;
    [SerializeField] private float stressMaxVal = 50f;
    [SerializeField] private float maxHeight = 1.09f;


    // Update is called once per frame
    void Update()
    {
        float stress = SanitySystem.Instance.Sanity;

        if (stress >= stressMinVal && stress <= stressMaxVal && transform.position.y < maxHeight)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }

        if (transform.position.y >= maxHeight)
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z); 
        }
    }

    public void ActivateTemple()
    {
        if (transform.position.y < maxHeight)
        {
            Debug.Log("Temple is not fully risen yet. Current height: " + transform.position.y);
            return;
        }
        Debug.Log("Temple activated! Game Over!");
    }
}
