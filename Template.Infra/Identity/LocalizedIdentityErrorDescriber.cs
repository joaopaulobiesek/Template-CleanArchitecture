namespace Template.Infra.Identity;

public class IdentityPortugueseMessages : IdentityErrorDescriber
{
    public override IdentityError DefaultError() => new() { Code = "Erro desconhecido", Description = $"Um erro desconhecido ocorreu." }; 
    public override IdentityError ConcurrencyFailure() => new() { Code = "Erro de concorrência", Description = "Falha de concorrência otimista, o objeto foi modificado." }; 
    public override IdentityError PasswordMismatch() => new() { Code = "Senha inválida", Description = "Senha incorreta." }; 
    public override IdentityError InvalidToken() => new() { Code = "Token inválido", Description = "Token inválido." }; 
    public override IdentityError LoginAlreadyAssociated() => new() { Code = "Login inválido", Description = "Já existe um usuário com este login." }; 
    public override IdentityError InvalidUserName(string? userName) => new() { Code = "Usuário inválido", Description = $"Login '{userName}' é inválido, pode conter apenas letras ou dígitos." }; 
    public override IdentityError InvalidEmail(string? email) => new() { Code = "Email inválido", Description = $"E-mail '{email}' é inválido." }; 
    public override IdentityError DuplicateUserName(string userName) => new() { Code = "Usuário duplicado", Description = $"Login '{userName}' já está sendo utilizado." }; 
    public override IdentityError DuplicateEmail(string email) => new() { Code = "Email duplicado", Description = $"E-mail '{email}' já está sendo utilizado." }; 
    public override IdentityError InvalidRoleName(string? role) => new() { Code = "Permissão inválida", Description = $"A permissão '{role}' é inválida." }; 
    public override IdentityError DuplicateRoleName(string role) => new() { Code = "Permissão inválida", Description = $"A permissão '{role}' já está sendo utilizada." }; 
    public override IdentityError UserAlreadyHasPassword() => new() { Code = "Senha já cadastrada", Description = "Usuário já possui uma senha definida." }; 
    public override IdentityError UserLockoutNotEnabled() => new() { Code = nameof(UserLockoutNotEnabled), Description = "Lockout não está habilitado para este usuário." }; 
    public override IdentityError UserAlreadyInRole(string role) => new() { Code = "Usuário já possui essa permissão", Description = $"Usuário já possui a permissão '{role}'." }; 
    public override IdentityError UserNotInRole(string role) => new() { Code = "Sem permissão", Description = $"Usuário não tem a permissão '{role}'." }; 
    public override IdentityError PasswordTooShort(int length) => new() { Code = "Senha curta", Description = $"Senhas devem conter ao menos {length} caracteres." }; 
    public override IdentityError PasswordRequiresNonAlphanumeric() => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Senhas devem conter ao menos um caracter não alfanumérico." }; 
    public override IdentityError PasswordRequiresDigit() => new() { Code = nameof(PasswordRequiresDigit), Description = "Senhas devem conter ao menos um digito ('0'-'9')." }; 
    public override IdentityError PasswordRequiresLower() => new() { Code = nameof(PasswordRequiresLower), Description = "Senhas devem conter ao menos um caracter em caixa baixa ('a'-'z')." }; 
    public override IdentityError PasswordRequiresUpper() => new() { Code = nameof(PasswordRequiresUpper), Description = "Senhas devem conter ao menos um caracter em caixa alta ('A'-'Z')." }; 
}