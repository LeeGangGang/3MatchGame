using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] List<RectTransform> _rtList = new List<RectTransform>();

    [SerializeField] RectTransform _maskTopRt;
    [SerializeField] RectTransform _maskBotRt;

    private void Awake()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = safeArea.position;
        Vector2 maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rt.anchorMin = minAnchor;
        rt.anchorMax = maxAnchor;

        foreach (var childRT in _rtList)
        {
            childRT.anchorMin = minAnchor;
            childRT.anchorMax = maxAnchor;
        }

        if (_maskTopRt != null)
        {
            _maskTopRt.anchorMin = new Vector2(0f, maxAnchor.y);
            _maskTopRt.anchorMax = Vector2.one;
        }
        if (_maskBotRt != null)
        {
            _maskBotRt.anchorMin = Vector2.zero;
            _maskBotRt.anchorMax = new Vector2(1f, minAnchor.y);
        }
    }
}