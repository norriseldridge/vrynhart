public class QuickSelectItemListingClickEvent
{
    public QuickSelectItemListing QuickSelectItemListing { get; private set; }
    public QuickSelectItemListingClickEvent(QuickSelectItemListing quickSelectItemListing)
    {
        QuickSelectItemListing = quickSelectItemListing;
    }
}
