using Template.Domain.Validations;
using System.Text.RegularExpressions;

namespace Template.Domain;

public static class StringFormatter
{
    public static string RemoveNonNumericCharacters(string input)
        => Regex.Replace(input, @"[^\d]", "");

    public static string FormatZipCode(string zipCode) =>
        Convert.ToUInt64(RemoveNonNumericCharacters(zipCode)).ToString(@"00000-000");

    public static string FormatCnpj(string cnpj) =>
        Convert.ToUInt64(RemoveNonNumericCharacters(cnpj)).ToString(@"00\.000\.000\/0000\-00");

    public static string FormatCpf(string cpf) =>
        Convert.ToUInt64(RemoveNonNumericCharacters(cpf)).ToString(@"000\.000\.000\-00");

    public static string FormatCpfOrCnpj(string cpfOrCnpj) =>
        RemoveNonNumericCharacters(cpfOrCnpj).Length switch
        {
            11 => Convert.ToUInt64(RemoveNonNumericCharacters(cpfOrCnpj)).ToString(@"000\.000\.000\-00"),
            14 => Convert.ToUInt64(RemoveNonNumericCharacters(cpfOrCnpj)).ToString(@"00\.000\.000\/0000\-00"),
            _ => cpfOrCnpj
        };

    public static string FormatPhoneNumber(string phoneNumber) =>
        RemoveNonNumericCharacters(phoneNumber).Length switch
        {
            10 => Convert.ToUInt64(RemoveNonNumericCharacters(phoneNumber)).ToString(@"(00)0000-0000"),
            11 => Convert.ToUInt64(RemoveNonNumericCharacters(phoneNumber)).ToString(@"(00)0 0000-0000"),
            _ => phoneNumber
        };

    public static bool IsValidCpfOrCnpj(string cpfOrCnpj) =>
        cpfOrCnpj.Length == 11 ? CPFValidationAttribute.IsValid(cpfOrCnpj)
        : (cpfOrCnpj.Length == 14 ? CNPJValidationAttribute.IsValid(cpfOrCnpj)
        : false);
    
    public static bool IsValidPhoneNumber(string phoneNumber) =>
        phoneNumber.Length == 10 || phoneNumber.Length == 11;

    public static bool IsValidCnpj(string cnpj) =>
        CNPJValidationAttribute.IsValid(cnpj);

    //CNPJ null no valida CPF é para saber se o CNPJ é Estrangeiro se for aceita CPF 000.000.000-00
    public static bool IsValidCpf(string cpf, string? cnpj = null) =>
        CPFValidationAttribute.IsValid(cpf, cnpj);
}