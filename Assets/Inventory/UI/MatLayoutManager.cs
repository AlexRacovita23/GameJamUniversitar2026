using System.Collections.Generic;
using UnityEngine;

public class MatLayoutManager : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryZone;
    [SerializeField] private RectTransform craftingZone;

    private const int MaxAttempts = 30;

    public Vector2 GetValidPosition(
        RectTransform itemRect,
        RectTransform zone,
        List<RectTransform> placedItems)
    {
        Vector2 itemSize = itemRect.sizeDelta;

        Rect zoneRect = zone.rect;

        float minX = zoneRect.xMin + itemSize.x * 0.5f;
        float maxX = zoneRect.xMax - itemSize.x * 0.5f;
        float minY = zoneRect.yMin + itemSize.y * 0.5f;
        float maxY = zoneRect.yMax - itemSize.y * 0.5f;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            itemRect.localPosition = candidate;

            bool overlaps = false;
            foreach (var placed in placedItems)
            {
                if (RectOverlapChecker.Overlaps(itemRect, placed))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps) return candidate;
        }

        return itemRect.localPosition;
    }
}