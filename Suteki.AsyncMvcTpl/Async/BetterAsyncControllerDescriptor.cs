using System;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace Suteki.AsyncMvcTpl.Async
{
    public class BetterAsyncControllerDescriptor : ControllerDescriptor
    {
        public override ActionDescriptor FindAction(ControllerContext controllerContext, string actionName)
        {
            var selector = new AsyncActionMethodSelector(controllerContext.Controller.GetType());
            var creator = selector.FindActionMethod(controllerContext, actionName);
            return creator == null ? null : creator(actionName, this);
        }

        //doesnt seem to even be used by the mvc framework
        public override ActionDescriptor[] GetCanonicalActions()
        {
            throw new NotImplementedException();
        }

        //only appears to be consumed within the Controller descriptor
        public override Type ControllerType
        {
            get { throw new NotImplementedException(); }
        }
    }
}