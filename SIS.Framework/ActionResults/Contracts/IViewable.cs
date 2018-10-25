namespace SIS.Framework.ActionResults.Contracts
{
    using SIS.Framework.ActionResults.Base;

    public interface IViewable : IActionResult
    {
        IRenderable View { get; set; }
    }
}
