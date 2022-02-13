using UnityEngine;

public class UseItemEvent
{
    public ItemRecord Item { get; private set; }
    public Vector3 CastPosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public UseItemEvent(ItemRecord item, Vector3 castPosition, Vector3 targetPosition)
    {
        Item = item;
        CastPosition = castPosition;
        TargetPosition = targetPosition;
    }
}
