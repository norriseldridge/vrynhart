using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantSlugBossDeath : BossDeath
{
    [SerializeField]
    List<SpriteRenderer> _swirls;

    protected override void Cleanup()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (_swirls[0].color.a > 0)
        {
            foreach (var s in _swirls)
                s.color = new Color(s.color.r, s.color.g, s.color.b, s.color.a - Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);
    }
}
