using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace Suteki.AsyncMvcTpl.Async
{
    public class BetterAsyncControllerActionInvoker : AsyncControllerActionInvoker
    {
        private readonly IControllerDescriptorCache descriptorCache;

        public BetterAsyncControllerActionInvoker(IControllerDescriptorCache descriptorCache)
        {
            this.descriptorCache = descriptorCache;
        }

        protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
        {
            var controllerType = controllerContext.Controller.GetType();
            var controllerDescriptor = descriptorCache.GetDescriptor(controllerType);
            return controllerDescriptor;
        }
    }
}