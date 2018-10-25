namespace SIS.Framework.Attributes.Methods
{
    using SIS.Framework.Attributes.Methods.Base;

    public class HttpPutAttribute : HttpMethodAttribute
    {
        public override bool IsValid(string requestMethod)
        {
            if (requestMethod.ToUpper() == "PUT")
            {
                return true;
            }

            return false;
        }
    }
}
