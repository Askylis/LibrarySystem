namespace SkyHope.LibraryManager.WebApi.HttpModels.Response
{
    public enum CheckoutResponseType
    {
        Success = 1,
        Unavailable = 2,
        LateFeesOverdue = 3,
        TooManyBooks = 4
    }
}
