using System.Threading;
using System.Threading.Tasks;
using Suteki.AsyncMvcTpl.Models;

namespace Suteki.AsyncMvcTpl.Services
{
    public class UserService
    {
        readonly UserRepository userRepository = new UserRepository();
        readonly EmailService emailService = new EmailService();

        public Task<User> GetCurrentUser()
        {
            const int currentUserId = 10;
            return userRepository.GetUserById(currentUserId);
        }

        public Task<int> SendUserAMessage(User user, string message)
        {
            //simulate sending a message for this example
            return Task<int>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return 1;
            });
            //return emailService.SendEmail(user.Email, message).ContinueWith(_ => 0);
        }
    }
}