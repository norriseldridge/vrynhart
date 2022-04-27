public class LeverEvent
{
    public string LeverID { get; private set; }
    public bool InSaveData { get; private set; }

    public LeverEvent(string leverId, bool inSaveData = false)
    {
        LeverID = leverId;
        InSaveData = inSaveData;
    }
}
