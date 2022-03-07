using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour
{
    [SerializeField]
    string _firstScene;

    [SerializeField]
    Texture2D _cursor;

    void Start()
    {
        Cursor.SetCursor(_cursor, Vector2.zero, CursorMode.Auto);
        SceneManager.LoadSceneAsync(_firstScene);
    }

    void Update()
    {
        Cursor.visible = !CustomInput.IsController();
    }
}
