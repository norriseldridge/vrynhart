using UnityEngine.UI;

public class SelectEvent
{
    public Selectable Selectable { get; private set; }
    public SelectEvent(Selectable selectable)
    {
        Selectable = selectable;
    }
}
