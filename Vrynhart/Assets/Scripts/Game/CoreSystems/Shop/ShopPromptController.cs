using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

public class ShopPromptController : MonoBehaviour
{
    bool _isOpen = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Brokers.Default.Receive<ShopEvent>()
            .Subscribe(OnShopEvent)
            .AddTo(this);
    }

    void OnShopEvent(ShopEvent e)
    {
        if (e.Open)
        {
            if (!_isOpen)
            {
                SceneManager.LoadSceneAsync(Constants.Game.ShopScene, LoadSceneMode.Additive)
                    .completed += _ =>
                    {
                        var shop = FindObjectOfType<ShopController>();
                        shop.SetShopListings(e.ItemIds);
                        shop.SetDialogue(e.Conversation);
                    };
                _isOpen = true;
            }
        }
        else
        {
            SceneManager.UnloadSceneAsync(Constants.Game.ShopScene);
            _isOpen = false;
        }
    }
}
