using UniRx;

public static class Brokers
{
    static MessageBroker _audioBroker;

    public static IMessageBroker Audio
    {
        get
        {
            if (_audioBroker == null)
                _audioBroker = new MessageBroker();
            return _audioBroker;
        }
    }

    public static IMessageBroker Default => MessageBroker.Default;
}
