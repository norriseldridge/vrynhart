using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    PlayerView _view;

    [SerializeField]
    Light2D _lanternLight;

    [SerializeField]
    Health _health;

    [SerializeField]
    TileMover _mover;

    [SerializeField]
    ItemUseTarget _itemTarget;

    [SerializeField]
    BloodSpawner _bloodSpawnerSource;

    [Header("Audio")]
    [SerializeField]
    AudioClip _itemPickup;

    [SerializeField]
    AudioClip _hit;

    [SerializeField]
    AudioClip _heal;

    [SerializeField]
    AudioClip _death;

    Camera _camera;
    public Health Health => _health;
    Vector2 _momentum;
    TileMap _tileMap;

    public Inventory Inventory => _inventory;
    Inventory _inventory = new Inventory();
    public ItemRecord EquippedItem => _equippedItem;
    ItemRecord _equippedItem;
    public List<string> QuickItems
    {
        get => _quickItems;
        set => _quickItems = value;
    }
    List<string> _quickItems;
    public int QuickSelectIndex => _quickSelectIndex;
    int _quickSelectIndex;

    public void Initialize(SaveData saveData)
    {
        _camera = FindObjectOfType<Camera>();
        _tileMap = FindObjectOfType<TileMap>();
        PopulateWithSaveData(saveData);
        Brokers.Default.Publish(new PlayerInputEvent(transform.position)); // get the move indicator in position

        Brokers.Default.Receive<TurnProgressionEvent>()
            .Subscribe(OnTurnProgression)
            .AddTo(this);

        Brokers.Default.Receive<ItemPickUpEvent>()
            .Subscribe(OnItemPickedUp)
            .AddTo(this);

        Brokers.Default.Receive<PlayerEquippedItemChangeEvent>()
            .Subscribe(OnPlayerEquippedItemChangeEvent)
            .AddTo(this);

        Brokers.Default.Receive<HealthChangeEvent>()
            .Where(e => e.Health == _health)
            .Subscribe(e =>
            {
                if (e.Change > 0)
                    Brokers.Audio.Publish(new AudioEvent(_heal, 0.7f));
                else
                    Brokers.Audio.Publish(new AudioEvent(_hit));
            })
            .AddTo(this);

        _health.ObserveEveryValueChanged(health => health.IsAlive)
            .Where(alive => !alive)
            .Subscribe(_ => OnDied())
            .AddTo(this);

        _itemTarget.transform.SetParent(null);
    }

    void PopulateWithSaveData(SaveData saveData)
    {
        if (saveData != null)
        {
            _inventory.Set(saveData.Inventory);
            _health.SetHealth(saveData.Health);
            _quickItems = saveData.QuickItems;
            _quickSelectIndex = saveData.QuickSelectIndex;

            if (_quickSelectIndex >= 0)
            {
                if (_quickItems.Count > _quickSelectIndex)
                {
                    var id = _quickItems[_quickSelectIndex];
                    if (!string.IsNullOrEmpty(id))
                        _equippedItem = ItemsLookup.GetItem(id);
                }
            }

            transform.position = new Vector3(saveData.X, saveData.Y, 0);
            Brokers.Default.Publish(new PlayerViewDataEvent(saveData.ViewData));
        }
    }

    void Update()
    {
        if (!_health.IsAlive)
            return;

        if (PauseController.IsPaused)
            return;

        HandleIFrames();
        HandlePause();
        ToggleQuickSelectItem();
        HandleLantern();
        HandleUseEquippedItem();
        HandleMove();
    }

    void HandleIFrames()
    {
        _view.SetIFrameState(_health.HasActiveIFrames);
    }

    void HandlePause()
    {
        if (CustomInput.GetKeyDown(CustomInput.Start))
            PauseController.Open();
    }

    void ToggleQuickSelectItem()
    {
        if (_quickItems.Count == 0)
            return;

        if (CustomInput.GetKeyDown(CustomInput.Toggle))
        {
            _quickSelectIndex++;
            if (_quickSelectIndex >= _quickItems.Count)
                _quickSelectIndex = 0;

            _equippedItem = ItemsLookup.GetItem(_quickItems[_quickSelectIndex]);
            Brokers.Default.Publish(new PlayerEquippedItemChangeEvent(_equippedItem));
        }
    }

    void HandleLantern()
    {
        var hasOil = _inventory.GetCount("oil") > 0;
        _lanternLight.enabled = hasOil && EquippedItem != null && EquippedItem.Id == "lantern";
    }

    void HandleUseEquippedItem()
    {
        if (EquippedItem == null || !EquippedItem.ShowUseIndicator || PauseController.IsPaused)
        {
            _itemTarget.Hide();
            return;
        }

        // TODO accept controller input here
        Vector3 target = transform.position;
        if (CustomInput.IsController())
        {
            var x = Input.GetAxis("Mouse X");
            var y = Input.GetAxis("Mouse Y");
            var useRange = EquippedItem.UseRange;
            var dx = x * useRange;
            var dy = -y * useRange;
            target += new Vector3(Mathf.RoundToInt(dx), Mathf.RoundToInt(dy), 0);
        }
        else
        {
            var worldSpace = _camera.ScreenToWorldPoint(Input.mousePosition);
            target = new Vector3(Mathf.RoundToInt(worldSpace.x), Mathf.RoundToInt(worldSpace.y), Mathf.RoundToInt(worldSpace.z));
        }

        // update the view
        if (Vector2.Distance(transform.position, target) <= EquippedItem.UseRange)
            _itemTarget.Set(EquippedItem, target);
        else
            _itemTarget.Hide();

        // attempt to use the item
        if (CustomInput.GetKeyDown(CustomInput.Use))
            Brokers.Default.Publish(new UseItemEvent(EquippedItem, transform.position, target));
    }

    void HandleMove()
    {
        if (!_mover.enabled || _mover.IsMoving.Value)
            return;

        var momentum = Vector2.zero;
        if (Input.GetAxis("Vertical") > CustomInput.CONTROLLER_AXIS_THRESHOLD)
            momentum.y = 1;

        if (Input.GetAxis("Vertical") < -CustomInput.CONTROLLER_AXIS_THRESHOLD)
            momentum.y = -1;

        if (Input.GetAxis("Horizontal") < -CustomInput.CONTROLLER_AXIS_THRESHOLD)
            momentum.x = -1;

        if (Input.GetAxis("Horizontal") > CustomInput.CONTROLLER_AXIS_THRESHOLD)
            momentum.x = 1;

        if (!_mover.IsMoving.Value && momentum.magnitude > 0)
        {
            // if we're trying to move diagonal, move sideways
            if (Mathf.Abs(momentum.x) > 0 && Mathf.Abs(momentum.y) > 0)
            {
                momentum.y = 0;
            }

            // update the player view to match the input even if we don't move
            if (Mathf.Abs(momentum.x) > 0)
                _view.SetDirection(momentum.x < 0 ? PlayerFacing.Left : PlayerFacing.Right);

            var target = _mover.GetPositionInDirection(momentum);
            if (_momentum != momentum && _tileMap.IsFloorAt(target))
            {
                _momentum = momentum;
                Brokers.Default.Publish(new PlayerInputEvent(target));
                Brokers.Default.Publish(new TurnProgressionEvent());
            }
        }

        // update the animation
        _view.SetState(_mover.IsMoving.Value ? PlayerState.Run : PlayerState.Idle);
    }

    // event handling
    void OnTurnProgression(TurnProgressionEvent e)
    {
        if (_momentum.magnitude > 0)
        {
            _mover.TryMove(_momentum);
        }

        _momentum = Vector2.zero;
    }

    void OnItemPickedUp(ItemPickUpEvent e)
    {
        Brokers.Audio.Publish(new AudioEvent(_itemPickup, 0.25f));
        _inventory.AddItem(e.ItemId, e.Count);
    }

    void OnPlayerEquippedItemChangeEvent(PlayerEquippedItemChangeEvent e)
    {
        if (_quickSelectIndex >= 0 && _quickSelectIndex < QuickItems.Count)
            _equippedItem = ItemsLookup.GetItem(QuickItems[_quickSelectIndex]);
    }

    void OnDied()
    {
        Instantiate(_bloodSpawnerSource, transform.position, Quaternion.Euler(Vector3.zero));
        Destroy(gameObject);
        Brokers.Audio.Publish(new AudioEvent(_death, 0.8f));
        Brokers.Default.Publish(new PlayerDiedEvent());
        SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
    }
}
