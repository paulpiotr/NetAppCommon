#region using

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetAppCommon.BasicAuthentication.Helpers;
using NetAppCommon.BasicAuthentication.Models;
using NetAppCommon.BasicAuthentication.Services.Interface;

#endregion

namespace NetAppCommon.BasicAuthentication.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new()
        {
            new()
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Username = "test",
                Password = "test"
            }
        };

        public async Task<User> Authenticate(string username, string password)
        {
            User user = await Task.Run(() =>
                _users.SingleOrDefault(x => x.Username == username && x.Password == password));
            // return null if user not found
            if (user == null)
            {
                return null;
            }

            // authentication successful so return user details without password
            return user.WithoutPassword();
        }

        public async Task<IEnumerable<User>> GetAll() => await Task.Run(() => _users.WithoutPasswords());
    }
}
