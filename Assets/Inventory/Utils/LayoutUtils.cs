using System.Collections.Generic;
using UnityEngine;

public static class LayoutUtils
{
    private const int MaxAttempts = 100;

    public static Vector2 GetValidPosition(
        RectTransform itemRect,
        RectTransform zone,
        List<RectTransform> placedItems,
        RectTransform exclusionZone = null)
    {
        Vector2 itemSize = itemRect.sizeDelta;
        Vector2 pivot = itemRect.pivot;
        Rect zoneRect = zone.rect;

        float minX = zoneRect.xMin + itemSize.x * pivot.x;
        float maxX = zoneRect.xMax - itemSize.x * (1f - pivot.x);
        float minY = zoneRect.yMin + itemSize.y * pivot.y;
        float maxY = zoneRect.yMax - itemSize.y * (1f - pivot.y);

        if (minX > maxX) minX = maxX = (zoneRect.xMin + zoneRect.xMax) * 0.5f;
        if (minY > maxY) minY = maxY = (zoneRect.yMin + zoneRect.yMax) * 0.5f;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            if (IsValidPosition(itemRect, candidate, placedItems, exclusionZone))
                return candidate;
        }

        for (float x = minX; x <= maxX; x += itemSize.x)
        {
            for (float y = minY; y <= maxY; y += itemSize.y)
            {
                Vector2 candidate = new Vector2(x, y);
                if (IsValidPosition(itemRect, candidate, placedItems, exclusionZone))
                    return candidate;
            }
        }

        return new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
    }

    private static bool IsValidPosition(
        RectTransform itemRect,
        Vector2 position,
        List<RectTransform> placedItems,
        RectTransform exclusionZone)
    {
        itemRect.localPosition = position;

        if (exclusionZone != null && Overlaps(itemRect, exclusionZone))
            return false;

        foreach (var placed in placedItems)
        {
            if (placed == null) continue;
            if (Overlaps(itemRect, placed))
                return false;
        }

        return true;
    }

    public static bool Overlaps(RectTransform a, RectTransform b)
    {
        Canvas.ForceUpdateCanvases();

        Rect rectA = GetScreenRect(a);
        Rect rectB = GetScreenRect(b);

        return rectA.Overlaps(rectB);
    }

    private static Rect GetScreenRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}