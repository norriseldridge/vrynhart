using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageView : MonoBehaviour
{
    [SerializeField]
    List<SpriteRenderer> _renderers;

    public void TakeHit() => StartCoroutine(FlashColor(Color.red));

    IEnumerator FlashColor(Color target)
    {
        var originalColors = new Dictionary<SpriteRenderer, Color>();
        _renderers.ForEach(r => originalColors[r] = r.color);

        for (var i = 0; i < 3; ++i)
        {
            _renderers.ForEach(r => r.color = target);
            yield return new WaitForSeconds(0.1f);

            _renderers.ForEach(r => r.color = originalColors[r]);
            yield return new WaitForSeconds(0.1f);
        }

        _renderers.ForEach(r => r.color = originalColors[r]);
    }
}
