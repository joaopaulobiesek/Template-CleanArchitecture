namespace Template.Infra.Identity;

public interface ITokenService
{
    string GenerateJwt(ContextUser user, List<string> roles);
    string ValidateTokenGetUserId(string token);
}
