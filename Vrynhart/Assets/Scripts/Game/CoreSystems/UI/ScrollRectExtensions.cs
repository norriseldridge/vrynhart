using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectExtensions
{
    public static void ScrollTo(this ScrollRect scrollRect, Transform target)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = target.localPosition;
        scrollRect.content.localPosition = new Vector2(
            scrollRect.content.localPosition.x, // 0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
    }
}
