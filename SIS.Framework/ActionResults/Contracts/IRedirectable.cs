namespace SIS.Framework.ActionResults.Contracts
{
    using SIS.Framework.ActionResults.Base;

    public interface IRedirectable : IActionResult
    {
        string RedirectUrl { get; }
    }
}
