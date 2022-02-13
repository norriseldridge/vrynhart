using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TransitionView : MonoBehaviour
{
    [SerializeField]
    Image _image;

    [SerializeField]
    float _speed;

    public void FadeToBlack() => StartCoroutine(ToBlack());

    IEnumerator ToBlack()
    {
        Color color = Color.black;
        color.a = 0;
        _image.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime * _speed;
            _image.color = color;
            yield return null;
        }

        MessageBroker.Default.Publish(new TransitionCompleteEvent(TransitionType.Start));
    }

    public void FadeFromBlack() => StartCoroutine(FromBlack());

    IEnumerator FromBlack()
    {
        Color color = Color.black;
        color.a = 1;
        _image.color = color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime * _speed;
            _image.color = color;
            yield return null;
        }

        MessageBroker.Default.Publish(new TransitionCompleteEvent(TransitionType.End));
    }
}
