public class CameraShakeEvent
{
    public float ShakeTime { get; private set; }
    public float Intensity { get; private set; }
    public float Speed { get; private set; }

    public CameraShakeEvent(float shakeTime, float intensity, float speed)
    {
        ShakeTime = shakeTime;
        Intensity = intensity;
        Speed = speed;
    }
}
