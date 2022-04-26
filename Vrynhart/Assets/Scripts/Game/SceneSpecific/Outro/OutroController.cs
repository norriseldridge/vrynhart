using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OutroController : MonoBehaviour
{
    [SerializeField]
    Image _backing;

    [SerializeField]
    Text _text;

    [SerializeField]
    RectTransform _credits;

    [SerializeField]
    float _creditsSpeed;

    [SerializeField]
    AudioClip _laughSfx;

    [SerializeField]
    AudioClip _music;

    // Start is called before the first frame update
    void Start()
    {
        Brokers.Audio.Publish(new MusicEvent(null));

        var data = GameSaveSystem.GetCachedSaveData();
        var bossIds = new string[] {
            // not accounted for is vrynhart but you get by killing him
            "werewolf_boss",
            "draven_fight",
            "skeleton_boss",
            "green_house_boss",
            "demon_boss",
            "cellar_boss",
            "bat_boss"
        };
        var killedAllBosses = bossIds.All(data.CompletedFlags.Contains);

        if (killedAllBosses)
            StartCoroutine(GoodEnding());
        else
            StartCoroutine(BadEnding());
    }

    IEnumerator FadeToBlack()
    {
        for (var i = 1f; i > 0; i -= 0.5f * Time.deltaTime)
        {
            _backing.color = new Color(i, i, i, 1);
            yield return null;
        }
    }

    IEnumerator RollCredits()
    {
        while (_credits.localPosition.y < 800)
        {
            _credits.localPosition += _creditsSpeed * Time.deltaTime * Vector3.up;
            yield return null;
        }
    }

    IEnumerator GoodEnding()
    {
        yield return FadeToBlack();

        // good dialogue
        var dialogue = new string[] {
            "\"You...\"",
            "\"...how?\"",
            "You can feel the souls of the\nfallen hunters before you resonate from within.",
            "One alone is weak...\nbut combined their strength cannot be stopped.",
            "Vrynhart's Curse is broken."
        };

        foreach (var line in dialogue)
        {
            _text.text = line;
            _text.color = Color.black;
            for (var i = 0f; i < 1; i += Time.deltaTime)
            {
                _text.color = new Color(i, i, i, 1);
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => CustomInput.GetKeyDown(CustomInput.Accept));
            yield return new WaitForEndOfFrame();
        }

        Brokers.Audio.Publish(new MusicEvent(_music, shouldFade: false));

        _text.text = "";

        yield return new WaitForSeconds(1.0f);

        // credits
        yield return RollCredits();

        // back to main menu
        SceneManager.LoadScene(Constants.Game.MainMenuScene);
    }

    IEnumerator BadEnding()
    {
        yield return FadeToBlack();

        // bad dialogue
        var dialogue = new string[] {
            "You...",
            "You... fool!",
            "You may have defeated my physical form!\nBut your body will serve just as well.",
            "Strike me down and invite me in.\n<i>That's the deal!</i>",
            "Lord Vrynhart's body is no more but <i>I'm</i> still here...\n<i>we're</i> still here!"
        };

        foreach (var line in dialogue)
        {
            _text.text = line;
            _text.color = Color.black;
            for (var i = 0f; i < 1; i += Time.deltaTime)
            {
                _text.color = new Color(i, i, i, 1);
                yield return null;
            }

            yield return new WaitForEndOfFrame();

            var waitTime = 3.0f;

            yield return new WaitUntil(() => {
                waitTime -= Time.deltaTime;
                return CustomInput.GetKeyDown(CustomInput.Accept) || (waitTime <= 0.0f);
            });
            yield return new WaitForEndOfFrame();
        }

        Brokers.Audio.Publish(new AudioEvent(_laughSfx));
        Brokers.Audio.Publish(new MusicEvent(_music, shouldFade: false));

        _text.text = "";

        yield return new WaitForSeconds(2.0f);

        // credits
        yield return RollCredits();

        //stop music
        Brokers.Audio.Publish(new MusicEvent(null));

        yield return new WaitForSeconds(2.0f);

        // back to main menu
        SceneManager.LoadScene(Constants.Game.MainMenuScene);
    }
}
