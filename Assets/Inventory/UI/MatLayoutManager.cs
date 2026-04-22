using System.Collections.Generic;
using UnityEngine;

public class MatLayoutManager : MonoBehaviour
{
    private const int MaxAttempts = 50;

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

        if (minX > maxX) minX = maxX = (zoneRect.xMin + zoneRect.xMax) * 0.5f;
        if (minY > maxY) minY = maxY = (zoneRect.yMin + zoneRect.yMax) * 0.5f;

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
                if (placed == null) continue;
                if (RectOverlapChecker.Overlaps(itemRect, placed))
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps) return candidate;
        }

        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }
}