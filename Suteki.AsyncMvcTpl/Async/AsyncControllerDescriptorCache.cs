using System;
using System.Web.Mvc;
using Microsoft.Web.Mvc;

namespace Suteki.AsyncMvcTpl.Async
{
    public class AsyncControllerDescriptorCache : ReaderWriterCache<Type, ControllerDescriptor>, IControllerDescriptorCache
    {
        public virtual ControllerDescriptor GetDescriptor(Type controllerType)
        {
            return FetchOrCreateItem(controllerType, () => new BetterAsyncControllerDescriptor());
        }
    }
}