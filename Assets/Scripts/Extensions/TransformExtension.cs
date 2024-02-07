using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static IEnumerable<Transform> GetActiveChildren(this Transform transform)
    {
        List<Transform> children = new();
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
                children.Add(child);

        return children;
    }
}
