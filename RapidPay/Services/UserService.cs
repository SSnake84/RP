using RapidPay.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
    }

    public class UserService : IUserService
    {
        private List<User> Users = new List<User> {
            new User { Id = 1, Username = "Jose", Password = "Enser" },
            new User { Id = 2, Username = "Christos", Password = "Christodoulou" }
        };

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(
                () => Users.SingleOrDefault(
                    x => x.Username == username && x.Password == password
                )
            );
            if (user == null)
                return null;

            user.Password = null;

            return user;
        }
    }
}
