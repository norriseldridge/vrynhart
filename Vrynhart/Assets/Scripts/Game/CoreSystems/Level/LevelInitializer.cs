using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

public class LevelInitializer : MonoBehaviour
{
    public static string StartingPositionIdentifier { get; set; } = null;

    string[] _uiScenes = {
        "PlayerUI"
    };

    async void Start()
    {
        var startId = StartingPositionIdentifier;
        StartingPositionIdentifier = null;

        var levelExiters = FindObjectsOfType<LevelExiter>();
        foreach (var exiter in levelExiters)
            exiter.enabled = false;

        var saveData = GameSaveSystem.GetCachedSaveData();

        var startPositions = FindObjectOfType<StartingPoint>();
        var player = FindObjectOfType<PlayerController>();
        var mover = player.GetComponent<TileMover>();
        mover.enabled = false;
        player.Initialize(saveData);

        mover.ObserveEveryValueChanged(m => m.IsMoving)
            .ObserveOnMainThread()
            .Where(moving => moving)
            .Take(1)
            .Subscribe(_ => {
                foreach(var exiter in levelExiters)
                    exiter.enabled = true;
            });

        // When we first load a game (aka the game is "saved")
        // we should just use that location, otherwise that means
        // we are coming into the scene from another scene (aka a LevelExit)
        // so we should position the player at the appropriate entrance
        if (!GameSaveSystem.IsSaved() && !string.IsNullOrEmpty(startId))
        {
            var start = startPositions.GetStartLocationByName(startId);
            player.gameObject.transform.position = start.position;
        }

#if UNITY_EDITOR
        if (!UnityEditor.EditorPrefs.GetBool("LoadInit"))
        {
            mover.enabled = true;
            return;
        }
#endif

        foreach (var scene in _uiScenes)
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);

        await StartCoroutine(Wait());

        var post = FindObjectsOfType<PostLevelInitialize>(true);
        foreach (var p in post)
            p.Initialize();

        // fade in
        MessageBroker.Default.Publish(new TransitionEvent(TransitionType.End));
        await MessageBroker.Default.Receive<TransitionCompleteEvent>().Where(e => e.Type == TransitionType.End).Take(1);
        mover.enabled = true;
    }

    IEnumerator Wait()
    {
        // give unity time to load up all the scenes
        // starts should run etc.
        for (var i = 0; i < 5; ++i)
            yield return new WaitForEndOfFrame();
    }
}
