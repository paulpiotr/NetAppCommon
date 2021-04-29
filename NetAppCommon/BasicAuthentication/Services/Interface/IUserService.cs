#region using

using System.Collections.Generic;
using System.Threading.Tasks;
using NetAppCommon.BasicAuthentication.Models;

#endregion

namespace NetAppCommon.BasicAuthentication.Services.Interface
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<IEnumerable<User>> GetAll();
    }
}
