namespace SIS.Framework.Attributes.Methods
{
    using SIS.Framework.Attributes.Methods.Base;

    public class HttpPostAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToUpper() == "POST")
            {
                return true;
            }

            return false;
        }
    }
}
