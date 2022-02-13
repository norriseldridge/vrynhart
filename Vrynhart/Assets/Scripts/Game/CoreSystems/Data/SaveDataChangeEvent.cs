public class SaveDataChangeEvent
{
    public SaveData SaveData { get; private set; }
    public SaveDataChangeEvent(SaveData saveData)
    {
        SaveData = saveData;
    }
}
