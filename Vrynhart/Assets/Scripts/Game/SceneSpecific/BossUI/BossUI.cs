using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class BossUI : MonoBehaviour
{
    [System.Serializable]
    public struct BossDisplayData
    {
        public int Hp;
        public int MaxHp;
    }

    public static async Task Show(string bossName, BossDisplayData data)
    {
        var t = new TaskCompletionSource<bool>();
        SceneManager.LoadSceneAsync(Constants.Game.BossUIScene, LoadSceneMode.Additive)
            .completed += _ => {
                t.SetResult(true);
            };
        await t.Task;
        FindObjectOfType<BossUI>().Initialize(bossName, data);
    }

    public static void Close()
    {
        SceneManager.UnloadSceneAsync(Constants.Game.BossUIScene);
    }

    [SerializeField]
    Text _nameText;

    [SerializeField]
    Text _healthText;

    [SerializeField]
    Image _healthBar;

    [SerializeField]
    RectMask2D _fadeMask;

    [SerializeField]
    float _fadeSpeed;

    public void Initialize(string name, BossDisplayData data)
    {
        MessageBroker.Default.Receive<BossDisplayData>()
            .Subscribe(SetDisplay)
            .AddTo(this);

        _nameText.text = name;
        SetDisplay(data);
        StartCoroutine(FadeIn());
    }

    public void SetDisplay(BossDisplayData data)
    {
        var clamp = Mathf.Max(data.Hp, 0);
        _healthText.text = $"{clamp}/{data.MaxHp}";
        _healthBar.fillAmount = (float)clamp / data.MaxHp;
    }

    IEnumerator FadeIn()
    {
        var padding = _fadeMask.padding;
        while (padding.z > 0)
        {
            padding.z -= _fadeSpeed * Time.deltaTime;
            _fadeMask.padding = padding;
            yield return null;
        }
        _fadeMask.padding = Vector4.zero;

        var softness = _fadeMask.softness;
        while (softness.x > 0)
        {
            softness.x -= (int)(_fadeSpeed * Time.deltaTime);
            _fadeMask.softness = softness;
            yield return null;
        }
        _fadeMask.softness = Vector2Int.zero;
    }
}
