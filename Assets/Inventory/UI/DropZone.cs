using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public event Action<DraggableItem> OnItemDropped;

    public void OnDrop(PointerEventData e)
    {
        var draggable = e.pointerDrag?.GetComponent<DraggableItem>();
        if (draggable == null) return;

        draggable.WasAcceptedByDropZone = true;
        OnItemDropped?.Invoke(draggable);
    }
}