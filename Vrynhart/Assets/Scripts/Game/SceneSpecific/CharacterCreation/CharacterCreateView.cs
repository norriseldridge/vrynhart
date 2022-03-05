using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CharacterCreateView : MonoBehaviour
{
    [SerializeField]
    Button _head;

    [SerializeField]
    Button _hair;

    [SerializeField]
    Button _eyes;

    [SerializeField]
    ColorPicker _colorPicker;

    int _index;
    Button[] _parts;
    SaveData _saveData;

    void Start()
    {
        if (!DataStorage.TryLoad(GameSaveSystem.GetCurrentSaveFile(), out _saveData))
        {
            throw new System.Exception("You can't be in the character view right now! There is no save file made!");
        }

        _parts = new[] { _head, _hair, _eyes };
        Select(0);

        Brokers.Default.Receive<ColorChangeEvent>()
            .Subscribe(OnColorChange)
            .AddTo(this);
        Brokers.Default.Publish(new PlayerViewDataEvent(_saveData.ViewData));
    }

    public void Select(int index)
    {
        if (index < 0 || index >= _parts.Length)
        {
            Debug.LogError($"Tried to select a part for creation that does not exist. Provided index {index}.");
            return;
        }

        _index = index;

        for (int i = 0; i < _parts.Length; ++i)
        {
            _parts[i].targetGraphic.color = i == index ? Color.white : Color.black;
            _parts[i].GetComponentInChildren<Text>().color = i == index ? Color.black : Color.white;
        }

        switch (index)
        {
            case 0:
                _colorPicker.SetColor(_saveData.ViewData.Head);
                break;

            case 1:
                _colorPicker.SetColor(_saveData.ViewData.Hair);
                break;

            case 2:
                _colorPicker.SetColor(_saveData.ViewData.Eyes);
                break;
        }
    }

    void OnColorChange(ColorChangeEvent e)
    {
        switch (_index)
        {
            case 0:
                _saveData.ViewData.Head = e.Color;
                break;

            case 1:
                _saveData.ViewData.Hair = e.Color;
                break;

            case 2:
                _saveData.ViewData.Eyes = e.Color;
                break;
        }

        Brokers.Default.Publish(new PlayerViewDataEvent(_saveData.ViewData));
    }

    public async void OnClickDone()
    {
        _saveData.Scene = Constants.Game.StartingScene;
        DataStorage.Save(_saveData, GameSaveSystem.GetCurrentSaveFile());
        await TransitionController.TriggerTransitionAsTask();
        GameSaveSystem.LoadGame();
    }
}
