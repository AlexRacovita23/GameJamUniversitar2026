using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"=== Right Click Raycast Results ({results.Count} hits) ===");
            foreach (var result in results)
            {
                Debug.Log($"  Hit: {result.gameObject.name} (depth: {result.depth})");
            }

            if (results.Count == 0)
            {
                Debug.LogWarning("No UI elements hit! Check Canvas has GraphicRaycaster.");
            }
        }
    }
}