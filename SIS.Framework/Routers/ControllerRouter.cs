namespace SIS.Framework.Routers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    using SIS.Framework.ActionResults.Base;
    using SIS.Framework.ActionResults.Contracts;
    using SIS.Framework.Attributes.Methods.Base;
    using SIS.Framework.Controllers;

    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Responses.Contracts;

    using SIS.WebServer.Api.Contracts;
    using SIS.WebServer.Results;

    public class ControllerRouter : IHttpHandler
    {
        public ControllerRouter()
        {
        }

        public IHttpResponse Handle(IHttpRequest request)
        {
            var controllerName = string.Empty;
            var actionName = string.Empty;
            var requestMethod = request.RequestMethod.ToString();

            if (request.Url == "/")
            {
                controllerName = "Home";
                actionName = "Index";
            }
            else
            {
                var requestUrlSplit = request.Url.Split("/", StringSplitOptions.RemoveEmptyEntries);

                controllerName = requestUrlSplit[0];
                actionName = requestUrlSplit[1];
            }

            var controller = GetController(controllerName, request);

            var action = this.GetAction(requestMethod, controller, actionName);

            if (controller == null || action == null)
            {
                return new HttpResponse(HttpResponseStatusCode.NotFound);
            }

            return this.PrepareResponse(controller, action, request);
        }

        private IHttpResponse PrepareResponse(
                IActionResult actionResult)
        {
            string invocationResult = actionResult.Invoke();

            if (actionResult is IViewable)
            {
                return new HtmlResult(invocationResult, HttpResponseStatusCode.OK);
            }

            if (actionResult is IRedirectable)
            {
                return new RedirectResult(invocationResult);
            }

            throw new InvalidOperationException("The type of result is not viewable or redirectable.");
        }

        private object[] MapActionParameters(
                MethodInfo action, 
                IHttpRequest request, 
                Controller controller)
        {
            ParameterInfo[] actionParametersInfo = action.GetParameters();
            object[] mappedActionParameters = new object[actionParametersInfo.Length];

            for (int index = 0; index < actionParametersInfo.Length; index++)
            {
                ParameterInfo actionParameter = actionParametersInfo[index];

                if (actionParameter.ParameterType.IsPrimitive 
                    || actionParameter.ParameterType == typeof(string))
                {
                    mappedActionParameters[index] = ProcessPrimitiveParameter(actionParameter, request);
                    if (mappedActionParameters[index] is null)
                    {
                        break;
                    }
                }
                else
                {
                    var bindingModel = ProcessBindingModelParameters(actionParameter, request);
                    controller.ModelState.IsValid = this.IsValidModel(bindingModel, actionParameter.ParameterType);
                    mappedActionParameters[index] = bindingModel;
                }

            }

            return mappedActionParameters;
            
        }

        private bool? IsValidModel(object bindingModel, Type bindingModelType)
        {
            var properties = bindingModelType.GetProperties();

            foreach (var property in properties)
            {
                var propertyValidationAttributes = property
                        .GetCustomAttributes()
                        .Where(attr => attr is ValidationAttribute)
                        .Cast<ValidationAttribute>()
                        .ToList();

                foreach (var validationAttribute in propertyValidationAttributes)
                {
                    var propertyValue = property.GetValue(bindingModel);
                    if (!validationAttribute.IsValid(propertyValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private object ProcessBindingModelParameters(
                ParameterInfo param, 
                IHttpRequest request)
        {
            Type bindingModelType = param.ParameterType;

            var bindingModelInstance = Activator.CreateInstance(bindingModelType);
            var bindingModelProperties = bindingModelType.GetProperties();

            foreach (var property in bindingModelProperties)
            {
                try
                {
                    object value = GetParameterFromRequestData(request, property.Name);
                    property.SetValue(bindingModelInstance, Convert.ChangeType(value, property.PropertyType));
                }
                catch 
                {
                    Console.WriteLine($"The {property.Name} field could not be mapped.");
                }
            }

            return Convert.ChangeType(bindingModelInstance, bindingModelType);
        }

        private object ProcessPrimitiveParameter(
                ParameterInfo param, 
                IHttpRequest request)
        {
            object value = this.GetParameterFromRequestData(request, param.Name);
            return Convert.ChangeType(value, param.ParameterType);
        }

        private object GetParameterFromRequestData(IHttpRequest request, string paramName)
        {
            if (request.QueryData.ContainsKey(paramName))
            {
                return request.QueryData[paramName];
            }

            if (request.FormData.ContainsKey(paramName))
            {
                return request.FormData[paramName];
            }

            return null;
        }

        public MethodInfo GetAction(string requestMethod, Controller controller, string actionName)
        {
            var actions = this
                    .GetSuitableMethods(controller, actionName)
                    .ToList();

            if (!actions.Any())
            {
                return null;
            }

            foreach (var action in actions)
            {
                var httpMethodAttributes = action
                    .GetCustomAttributes()
                    .Where(ca => ca is HttpMethodAttribute)
                    .Cast<HttpMethodAttribute>()
                    .ToList();

                if (!httpMethodAttributes.Any() &&
                    requestMethod.ToUpper() == "GET")
                {
                    return action;
                }

                foreach (var httpMethodAttribute in httpMethodAttributes)
                {
                    if (httpMethodAttribute.IsValid(requestMethod))
                    {
                        return action;
                    }
                }
            }

            return null;
        }

        private IEnumerable<MethodInfo> GetSuitableMethods(Controller controller, string actionName)
        {
            if (controller == null)
            {
                return new MethodInfo[0];
            }

            return controller
                .GetType()
                .GetMethods()
                .Where(m => m.Name.ToLower() == actionName.ToLower());
        }

        public static Controller GetController(string controllerName, IHttpRequest request)
        {
            if (controllerName != null)
            {
                var controllerTypeName = string.Format("{0}.{1}.{2}{3}, {0}",
                MvcContext.Get.AssemblyName,
                MvcContext.Get.ControllersFolder,
                controllerName,
                MvcContext.Get.ControllerSuffix);

                var controllerType = Type.GetType(controllerTypeName);
                var controller = (Controller)Activator.CreateInstance(controllerType);

                if (controller != null)
                {
                    controller.Request = request;
                }

                return controller;
            }

            return null;
        }

        private IActionResult InvokeAction(Controller controller, MethodInfo action, object[] actionParameters)
        {
            return (IActionResult)action.Invoke(controller, actionParameters);
        }
    }
}
