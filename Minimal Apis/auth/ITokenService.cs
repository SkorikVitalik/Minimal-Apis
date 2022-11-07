namespace Minimal_Apis.auth
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDto userDto);
    }
}
