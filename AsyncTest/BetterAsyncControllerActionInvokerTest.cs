using System.Web.Mvc;
using System.Web.Mvc.Async;
using NUnit.Framework;
using Suteki.AsyncMvcTpl.Async;

namespace AsyncTest
{
    [TestFixture]
    public class BetterAsyncControllerActionInvokerTest
    {
        [Test]
        public void ShouldImplement_AsyncControllerActionInvoker()
        {
            var subjectUnderTest = CreateSut();

            Assert.IsInstanceOf<AsyncControllerActionInvoker>(subjectUnderTest);
        }

        [Test]
        public void ShouldOverrideGetControllerDescriptorToHookInToActionSelectorPipeline()
        {
            // Arrange
            var controllerContext = new ControllerContext
            {
                Controller = new EmptyController()
            };

            var subjectUnderTest = CreateSut();

            // Act
            var descriptor = subjectUnderTest.GetControllerDescriptor(controllerContext);

            // Assert
            Assert.IsInstanceOf<BetterAsyncControllerDescriptor>(descriptor);
        }

        private static BetterAsyncControllerActionInvokerTestWrapper CreateSut()
        {
            return new BetterAsyncControllerActionInvokerTestWrapper(new AsyncControllerDescriptorCache());
        }

        private class BetterAsyncControllerActionInvokerTestWrapper : BetterAsyncControllerActionInvoker
        {
            public BetterAsyncControllerActionInvokerTestWrapper(IControllerDescriptorCache descriptorCache)
                : base(descriptorCache)
            {
            }

            public new ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext)
            {
                return base.GetControllerDescriptor(controllerContext);
            }
        }

        private class EmptyController : AsyncController
        {
        }
    }
}