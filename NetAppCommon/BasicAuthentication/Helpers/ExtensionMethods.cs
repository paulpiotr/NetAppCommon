#region using

using System.Collections.Generic;
using System.Linq;
using NetAppCommon.BasicAuthentication.Models;

#endregion

namespace NetAppCommon.BasicAuthentication.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPasswords(this IEnumerable<User> users) =>
            users.Select(x => x.WithoutPassword());

        public static User WithoutPassword(this User user)
        {
            user.Password = null;
            return user;
        }
    }
}
