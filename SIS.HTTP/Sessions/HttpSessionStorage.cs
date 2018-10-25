namespace SIS.HTTP.Sessions
{
    using System.Collections.Concurrent;

    using SIS.HTTP.Sessions.Contracts;

    public class HttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private static readonly ConcurrentDictionary<string, IHttpSession> sessions
            = new ConcurrentDictionary<string, IHttpSession>();

        public static IHttpSession GetSession(string id)
        {
            return sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}
