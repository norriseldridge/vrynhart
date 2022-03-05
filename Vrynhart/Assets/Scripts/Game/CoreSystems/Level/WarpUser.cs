using UniRx;
using UnityEngine;

public class WarpUser : MonoBehaviour
{
    public void SetPosition(Vector3 position)
    {
        transform.position = position;

        var mover = GetComponent<TileMover>();
        if (mover)
            Brokers.Default.Publish(new TileMoveCompleteEvent(mover));
    }
}
