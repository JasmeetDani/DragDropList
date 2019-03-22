using UnityEngine;

public static class GeomUtils
{
    public static bool DoOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2)
    {
        // If one rectangle is on left side of other
        if (l1.x > r2.x || l2.x > r1.x)
            return false;

        // If one rectangle is above other
        if (l1.y < r2.y || l2.y < r1.y)
            return false;

        return true;
    }
}