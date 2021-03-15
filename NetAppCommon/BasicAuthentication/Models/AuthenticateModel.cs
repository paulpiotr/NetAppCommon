#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace NetAppCommon.BasicAuthentication.Models
{
    public class AuthenticateModel
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}