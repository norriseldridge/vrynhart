using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField]
    Transform _camera;

    [SerializeField]
    Transform _carriage;

    [SerializeField]
    AudioSource[] _carriageSfx;

    [SerializeField]
    Transform[] _fogs;

    [SerializeField]
    Transform[] _castleFogs;

    [SerializeField]
    Text _title;

    [SerializeField]
    Image _black;

    [SerializeField]
    Text _storyText;

    [SerializeField]
    Image _textFader;

    [SerializeField]
    SimpleConversation _introStory;

    [SerializeField]
    AudioClip _theme;

    [SerializeField]
    AudioSource _wind;

    [SerializeField]
    AudioClip _nya;

    void Start()
    {
        _title.enabled = false;
        _wind.volume = PlayerPrefs.GetFloat(Constants.Prefs.SFXVolume, 0.8f) * 0.7f;
        StartCoroutine(PlayCutScene());
    }

    IEnumerator PlayCutScene()
    {
        // hide mouse
        Cursor.visible = false;

        // play the theme
        MessageBroker.Default.Publish(new MusicEvent(_theme));

        yield return new WaitForSeconds(3.0f); // let the music play for a bit first / give transition time

        yield return PrintStory();
        StartCoroutine(ZoomOutCamera());
        StartCoroutine(MoveFog());
        StartCoroutine(MoveCastleFog());
        StartCoroutine(FadeFromBlack());
        yield return CarriageRunsIn();
        StartCoroutine(CarriageRunsOut()); // don't care to wait for this to finish
        yield return PanToCastle();
        yield return RevealTitle();
        yield return FadeToBlack();
        StartCoroutine(FadeWindVolume());
        yield return new WaitForSeconds(1);

        // un-hide mouse
        Cursor.visible = true;

        SceneManager.LoadSceneAsync("CharacterCreateScene");
    }

    IEnumerator ZoomOutCamera()
    {
        var zoomSpeed = 0.3f;
        var cam = _camera.GetComponent<Camera>();
        while (cam.orthographicSize < 4)
        {
            cam.orthographicSize += Time.deltaTime * zoomSpeed;
            yield return null;
        }
        cam.orthographicSize = 4;
    }

    IEnumerator MoveFog()
    {
        var sway = 0.012f;
        var swaySpeed = 0.11f;
        var scaleAmount = 0.07f;
        var scaleSpeed = 0.04f;
        var color = Color.white;
        var alphaAmount = 0.1f;
        var alphaSpeed = 0.3f;
        var count = _fogs.Length;

        SpriteRenderer[] fogSprites = new SpriteRenderer[count];
        for (int i = 0; i < count; ++i)
            fogSprites[i] = _fogs[i].GetComponent<SpriteRenderer>();

        while (gameObject)
        {
            for (int i = 0; i < count; ++i)
            {
                var direction = i % 2 == 0 ? -1 : 1;
                _fogs[i].position += Vector3.left * sway * direction * Mathf.Sin(Time.time + i * swaySpeed);

                var scale = Mathf.Cos(Time.time + 1 + i * scaleSpeed) * scaleAmount;
                _fogs[i].localScale = Vector3.one + (Vector3.one * scale);

                color.a = 0.7f - Mathf.Sin(Time.time * alphaSpeed + i) * alphaAmount;
                fogSprites[i].color = color;
            }
            yield return null;
        }
    }

    IEnumerator MoveCastleFog()
    {
        var speed = 0.01f;
        var amount = 0.008f;
        while (gameObject)
        {
            foreach (var fog in _castleFogs)
            {
                fog.transform.position += Vector3.right * Mathf.Sin(Time.time * speed) * amount;
            }
            yield return null;
        }
    }

    IEnumerator CarriageRunsIn()
    {
        foreach (var sfx in _carriageSfx)
            sfx.volume = 0;

        var carriageSpeed = 2.5f;
        var sfxSpeed = 0.135f;
        while (_carriage.transform.position.x > 0)
        {
            foreach (var sfx in _carriageSfx)
                sfx.volume += Time.deltaTime * sfxSpeed;

            _carriage.transform.position += Vector3.left * carriageSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CarriageRunsOut()
    {
        MessageBroker.Default.Publish(new AudioEvent(_nya, 0.3f));
        var carriageSpeed = 2.5f;
        var sfxSpeed = 0.35f;
        while (_carriage.transform.position.x > -12)
        {
            foreach (var sfx in _carriageSfx)
                sfx.volume -= Time.deltaTime * sfxSpeed;

            _carriage.transform.position += Vector3.left * carriageSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PanToCastle()
    {
        var cameraSpeed = 2;
        while (_camera.transform.position.y < 17)
        {
            _camera.transform.position += Vector3.up * cameraSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator RevealTitle()
    {
        _title.enabled = true;

        var color = _title.color;
        color.a = 0;
        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            _title.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(3);
    }

    IEnumerator FadeFromBlack()
    {
        var speed = 1.0f;
        var color = Color.black;
        color.a = 1;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * speed;
            _black.color = color;
            yield return null;
        }
    }

    IEnumerator PrintStory()
    {
        var charDelay = 0.14f;
        var maxDelay = 5.5f;
        foreach (var line in _introStory.Dialogue)
        {
            _storyText.text = line;
            var delay = Mathf.Clamp(_storyText.text.Length * charDelay, 0, maxDelay);
            StartCoroutine(FadeInText());
            yield return new WaitForSeconds(delay);
            _storyText.text = "";
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator FadeInText()
    {
        var color = new Color(0, 0, 0, 1);
        for (var i = 0.0f; i < 0.25f; i += Time.deltaTime)
        {
            color.a -= 2 * Time.deltaTime;
            _textFader.color = color;
            yield return null;
        }

        color.a = 0;
        _textFader.color = color;
    }

    IEnumerator FadeToBlack()
    {
        var speed = 0.8f;
        var color = Color.black;
        color.a = 0;
        while (color.a < 1)
        {
            color.a += Time.deltaTime * speed;
            _black.color = color;
            yield return null;
        }
    }

    IEnumerator FadeWindVolume()
    {
        var speed = 0.6f;
        while (_wind.volume > 0)
        {
            _wind.volume -= Time.deltaTime * speed;
            yield return null;
        }
    }
}
