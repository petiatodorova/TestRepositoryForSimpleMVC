namespace IRunesWebApp.Contracts
{
    public interface IUserCookieService
    {
        string GetUserCookie(string username);

        string GetUserData(string cookieContent);
    }
}
