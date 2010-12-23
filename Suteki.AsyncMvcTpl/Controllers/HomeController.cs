using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Suteki.AsyncMvcTpl.Async;
using Suteki.AsyncMvcTpl.Services;

namespace Suteki.AsyncMvcTpl.Controllers
{
    public class HomeController : Controller
    {
        readonly UserService userService = new UserService();

        public HomeController()
        {
            IControllerDescriptorCache descriptorCache = new AsyncControllerDescriptorCache();
            ActionInvoker = new BetterAsyncControllerActionInvoker(descriptorCache);
        }
        
        [HttpGet]
        public Task<ViewResult> Index()
        {
            return from user in userService.GetCurrentUser()
                   from _ in userService.SendUserAMessage(user, "Hi From the MVC TPL experiment")
                   select View(user);
        }

        [HttpGet]
        public Task DoStuff()
        {
            var doStuff = from user in userService.GetCurrentUser()
                          from _ in userService.SendUserAMessage(user, "Hi From the MVC TPL experiment")
                          select View("index", user);

            return doStuff.ContinueWith(x => { });
        }

        //[HttpGet]
        //public void IndexAsync()
        //{
        //    AsyncManager.OutstandingOperations.Increment();
        //    userService.GetCurrentUser().ContinueWith(t1 =>
        //    {
        //        var user = t1.Result;
        //        userService.SendUserAMessage(user, "Hi From the MVC TPL experiment").ContinueWith(_ =>
        //        {
        //            AsyncManager.Parameters["user"] = user;
        //            AsyncManager.OutstandingOperations.Decrement();
        //        });
        //    });
            
        //}

        //public ViewResult IndexCompleted(User user)
        //{
        //    return View(user);
        //}
    }
}