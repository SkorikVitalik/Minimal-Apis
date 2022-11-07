using System.ComponentModel.DataAnnotations;

namespace Minimal_Apis.auth
{
    public record class UserModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty ;
    }
    public record class UserDto(string UserName, string Password);
}