using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class ControllerSliderListNavigation : ControllerListNavigation<ControllerSliderAdjuster>
{
    protected override IEnumerable<ControllerSliderAdjuster> OnlyActive() => _elements.Where(e => e.gameObject.activeSelf);

    protected override void Update()
    {
        if (!CustomInput.IsController())
            return;

        base.Update();

        var active = GetSelected();
        foreach (var slider in GetValidElements())
        {
            slider.enabled = slider == active;
        }

        if (EventSystem.current.currentSelectedGameObject != active.gameObject)
            EventSystem.current.SetSelectedGameObject(active.gameObject);
    }
}
