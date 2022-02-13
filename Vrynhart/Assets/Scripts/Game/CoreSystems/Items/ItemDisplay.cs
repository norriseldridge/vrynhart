using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    [SerializeField]
    Image _image;

    [SerializeField]
    Text _text;

    public void SetCount(int count)
    {
        _text.text = count.ToString();
    }
}
