using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.AsyncMvcTpl.Async;

namespace AsyncTest
{
    [TestFixture]
    public class ReflectedTaskPatternActionDescriptorTest
    {
        [Test]
        public void ShouldImplement_AsyncActionDescriptor()
        {
            var actionMethod = typeof(TaskController).GetMethod("MethodReturningTask");

            var subjectUnderTest = new ReflectedTaskPatternActionDescriptor(actionMethod, null);

            Assert.IsInstanceOf<AsyncActionDescriptor>(subjectUnderTest);
        }

        [Test]
        public void ShouldReturnTaskResultWhenExecutedAsynchronously()
        {
            var controllerContext = MockRepository.GenerateStub<ControllerContext>();
            controllerContext.Controller = new TaskController();

            var actionMethod = typeof(TaskController).GetMethod("MethodReturningTask");

            var subjectUnderTest = new ReflectedTaskPatternActionDescriptor(actionMethod, null);
            
            AsyncCallback callback = ar => subjectUnderTest.EndExecute(ar);

            var task = Task<object>.Factory.FromAsync(
                subjectUnderTest.BeginExecute(controllerContext, null, callback, null),
                subjectUnderTest.EndExecute);

            Assert.IsInstanceOf<ViewResult>(task.Result);
        }

        [Test]
        public void ShouldHandleTasksWithNoReturnValue()
        {
            var controllerContext = MockRepository.GenerateStub<ControllerContext>();
            controllerContext.Controller = new TaskController();

            var actionMethod = typeof(TaskController).GetMethod("MethodReturningTaskWithNoResult");

            var subjectUnderTest = new ReflectedTaskPatternActionDescriptor(actionMethod, null);

            AsyncCallback callback = ar => subjectUnderTest.EndExecute(ar);

            var task = Task<object>.Factory.FromAsync(
                subjectUnderTest.BeginExecute(controllerContext, null, callback, null),
                subjectUnderTest.EndExecute);

            Assert.IsNull(task.Result);
        }

// ReSharper disable Asp.NotResolved
        private class TaskController : Controller, IAsyncManagerContainer {
            private readonly AsyncManager _asyncHelper = new AsyncManager();

            public AsyncManager AsyncManager {
                get {
                    return _asyncHelper;
                }
            }
            [HttpGet]
            public Task<ViewResult> MethodReturningTask()
            {
                return Task<ViewResult>.Factory.StartNew(View);
            }

            [HttpGet]
            public Task MethodReturningTaskWithNoResult()
            {
                return Task.Factory.StartNew(() => { });
                
            }
        }
// ReSharper restore Asp.NotResolved
    }
}

