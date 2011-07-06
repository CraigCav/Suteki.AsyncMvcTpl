using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Microsoft.Web.Mvc;

namespace Suteki.AsyncMvcTpl.Async
{
    public class ReflectedTaskPatternActionDescriptor : AsyncActionDescriptor
    {
        readonly MethodInfo actionMethod;
        readonly ControllerDescriptor controllerDescriptor;
        static readonly MethodDispatcherCache DispatcherCache = new MethodDispatcherCache();

        public ReflectedTaskPatternActionDescriptor(MethodInfo actionMethod, ControllerDescriptor controllerDescriptor)
        {
            this.actionMethod = actionMethod;
            this.controllerDescriptor = controllerDescriptor;
        }

        public override ParameterDescriptor[] GetParameters()
        {
            return actionMethod.GetParameters().Select(pi => new ReflectedParameterDescriptor(pi, this)).ToArray();
        }

        public override string ActionName
        {
            get { throw new NotImplementedException(); }
        }

        public override ControllerDescriptor ControllerDescriptor
        {
            get { return controllerDescriptor; }
        }

        //slightly altered from ReflectedDelegatePatternActionDescriptor in Asp.net mvc futures.
        public override IAsyncResult BeginExecute(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state)
        {
            var task = Task<object>.Factory.StartNew(x =>
            {
                var setupParametersInfos = actionMethod.GetParameters();
                var rawSetupParameterValues = from parameterInfo in setupParametersInfos
                                              select ExtractParameterFromDictionary(parameterInfo, parameters, actionMethod);
                var setupParametersArray = rawSetupParameterValues.ToArray();

                var setupDispatcher = DispatcherCache.GetDispatcher(actionMethod);

                var returnedDelegate = setupDispatcher.Execute(controllerContext.Controller, setupParametersArray);
                
                if(returnedDelegate != null && 
                    returnedDelegate.GetType().IsGenericType &&
                    typeof(Task<>).IsAssignableFrom(returnedDelegate.GetType().GetGenericTypeDefinition()))
                {
                    dynamic taskResult = returnedDelegate;
                    return taskResult.Result;
                }

                return null;
            }, state);

            if (callback != null) task.ContinueWith(res => callback(task));

            return task;
        }

        public override object EndExecute(IAsyncResult asyncResult)
        {
            return ((Task<object>)asyncResult).Result;
        }

        //copied from internal AsyncActionDescriptor implementation in Asp.net mvc futures.
        private static object ExtractParameterFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters, MethodInfo methodInfo)
        {
            object value;

            if (!parameters.TryGetValue(parameterInfo.Name, out value))
            {
                // the key should always be present, even if the parameter value is null
                string message = String.Format(CultureInfo.CurrentUICulture, "The parameters dictionary does not contain an entry for parameter '{0}' of type '{1}' for method '{2}' in '{3}'. The dictionary must contain an entry for each parameter, even parameters with null values.",
                                               parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }

            if (value == null && !TypeHelpers.TypeAllowsNullValue(parameterInfo.ParameterType))
            {
                // tried to pass a null value for a non-nullable parameter type
                string message = String.Format(CultureInfo.CurrentUICulture, "The parameters dictionary contains a null entry for parameter '{0}' of non-nullable type '{1}' for method '{2}' in '{3}'. To make a parameter optional its type should be either a reference type or a Nullable type.",
                                               parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);
                throw new ArgumentException(message, "parameters");
            }

            if (value != null && !parameterInfo.ParameterType.IsInstanceOfType(value))
            {
                // value was supplied but is not of the proper type
                string message = String.Format(CultureInfo.CurrentUICulture, "The parameters dictionary contains an invalid entry for parameter '{0}' for method '{1}' in '{2}'. The dictionary contains a value of type '{3}', but the parameter requires a value of type '{4}'.",
                                               parameterInfo.Name, methodInfo, methodInfo.DeclaringType, value.GetType(), parameterInfo.ParameterType);
                throw new ArgumentException(message, "parameters");
            }

            return value;
        }
    }
}