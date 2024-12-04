namespace WebApp.Entreo.Infrastructure.Auth
{
    public interface IUserAccessor
    {
        string GetCurrentUser();
        string GetCurrentUserId();
        string GetCurrentUsername();
        bool IsAuthenticated();
    }
}