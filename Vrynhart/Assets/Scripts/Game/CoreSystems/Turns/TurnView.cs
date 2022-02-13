using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
    [SerializeField]
    Image _timerImage;

    public void SetTime(float time, float maxTime)
    {
        _timerImage.fillAmount = 1 - (time / maxTime);
    }
}
