using System;
using System.Web.Mvc;

namespace Suteki.AsyncMvcTpl.Async
{
    public interface IControllerDescriptorCache
    {
        ControllerDescriptor GetDescriptor(Type controllerType);
    }
}