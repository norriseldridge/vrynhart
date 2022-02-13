public class CameraBoundsChangeEvent
{
    public CameraBounds Bounds { get; private set; }
    public bool Entering { get; private set; }

    public CameraBoundsChangeEvent(CameraBounds bounds, bool entering = true)
    {
        Bounds = bounds;
        Entering = entering;
    }
}