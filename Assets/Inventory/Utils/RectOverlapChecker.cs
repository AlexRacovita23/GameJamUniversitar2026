using UnityEngine;

public static class RectOverlapChecker
{
    public static bool Overlaps(RectTransform a, RectTransform b, float padding = 8f)
    {
        Rect rectA = GetScreenRect(a);
        Rect rectB = GetScreenRect(b);

        rectA = Shrink(rectA, padding);
        rectB = Shrink(rectB, padding);

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

    private static Rect Shrink(Rect rect, float amount)
    {
        return new Rect(
            rect.x + amount,
            rect.y + amount,
            rect.width - amount * 2,
            rect.height - amount * 2
        );
    }
}