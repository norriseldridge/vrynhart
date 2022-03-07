using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerButtonListNavigation : ControllerListNavigation<Button>
{
    protected override IEnumerable<Button> OnlyActive() => _elements.Where(e => e.gameObject.activeSelf);

    protected override void AcceptSelected()
    {
        GetSelected().onClick.Invoke();
    }

    protected override void Update()
    {
        if (!CustomInput.IsController())
            return;

        base.Update();

        var selected = GetSelected();
        RectTransform parent = (RectTransform)selected.transform.parent;
        if (parent)
        {
            var scrollRect = parent.GetComponentInParent<ScrollRect>();
            if (scrollRect)
            {
                scrollRect.ScrollTo(selected.transform);
            }
        }

        if (EventSystem.current.currentSelectedGameObject != selected.gameObject)
            EventSystem.current.SetSelectedGameObject(selected.gameObject);
    }
}
