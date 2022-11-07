using Minimal_Apis.auth;

namespace Minimal_Apis.Repository
{
    
    public class UserRepository:IUserRepository
    {
        private List<UserDto> users = new List<UserDto>()
        {
            new UserDto("John", "12345"),
            new UserDto("Milana", "V545rat545_"),
            new UserDto("Vitalik", "545rat545"),
        };

        public UserDto GetUser(UserModel user)
        {
            return users.FirstOrDefault(u => string.Equals(user.UserName, u.UserName
                ) && string.Equals(user.Password, u.Password)) ?? throw new Exception();
        }
    }
}
