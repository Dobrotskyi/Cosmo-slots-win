using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenSafeArea : MonoBehaviour
{
    private void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        var saveArea = Screen.safeArea;
        var anchorMin = saveArea.position;
        var anchorMax = anchorMin + saveArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}