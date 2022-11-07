using Minimal_Apis.auth;

namespace Minimal_Apis.Repository
{
    public interface IUserRepository
    {
        public UserDto GetUser(UserModel user);
    }
}
