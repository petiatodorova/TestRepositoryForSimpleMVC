namespace SIS.Framework.Attributes.Methods
{
    using SIS.Framework.Attributes.Methods.Base;

    public class HttpGetAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToUpper() == "GET")
            {
                return true;
            }

            return false;
        }
    }
}
