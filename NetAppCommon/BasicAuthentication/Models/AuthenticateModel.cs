using System.ComponentModel.DataAnnotations;

namespace NetAppCommon.BasicAuthentication.Models
{
    public partial class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
