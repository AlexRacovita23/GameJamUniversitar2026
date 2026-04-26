using System;
using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    public Action exitBorder;
    public Action enterBorder;

    private void OnTriggerExit(Collider other)
    {
        exitBorder?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        enterBorder?.Invoke();
    }
}
