namespace Trell.AddressablesPopupManager
{
    public enum ShowPopupLogic
    {
        Over, // Show over others popups
        Queue, // Show after current popup close
        Stack, // Hide current popup and add him to the Queue, show new
    }
}