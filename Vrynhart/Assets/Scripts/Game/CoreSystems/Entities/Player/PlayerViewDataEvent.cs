public class PlayerViewDataEvent
{
    PlayerViewData _viewData;
    public PlayerViewData ViewData => _viewData;
    public PlayerViewDataEvent(PlayerViewData viewData) => _viewData = viewData;
}
