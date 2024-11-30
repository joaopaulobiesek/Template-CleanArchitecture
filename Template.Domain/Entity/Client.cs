using Template.Domain.Interfaces;
using Template.Domain.Validation;

namespace Template.Domain.Entity;

public sealed class Client : Entity
{
    public string FullName { get; private set; }
    public string DocumentNumber { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public string? ZipCode { get; private set; }

    public void CreateClient(ICreateClient c)
    {
        this.ValidateFullName(c.FullName);
        this.ValidateDocumentNumber(c.DocumentNumber);
        this.ValidatePhone(c.Phone);
        this.ValidateZipCode(c.ZipCode);
        this.ValidateEmail(c.Email);

        this.FullName = c.FullName;
        this.Email = c.Email;
        this.DocumentNumber = StringFormatter.FormatCpfOrCnpj(c.DocumentNumber);
        this.Phone = string.IsNullOrEmpty(c.Phone) ? null : StringFormatter.FormatPhoneNumber(c.Phone);
        this.ZipCode = string.IsNullOrEmpty(c.ZipCode) ? null : StringFormatter.FormatZipCode(c.ZipCode);
    }

    public void UpdateClient(IUpdateClient c)
    {
        DomainExceptionValidation.When(!this.Active, "Cannot update a deleted client.");
        this.ValidateFullName(c.FullName);
        this.ValidatePhone(c.Phone);
        this.ValidateZipCode(c.ZipCode);
        this.ValidateEmail(c.Email);

        this.FullName = c.FullName;
        this.Email = c.Email;
        this.Phone = string.IsNullOrEmpty(c.Phone) ? null : StringFormatter.FormatPhoneNumber(c.Phone);
        this.ZipCode = string.IsNullOrEmpty(c.ZipCode) ? null : StringFormatter.FormatZipCode(c.ZipCode);
    }

    public void DeleteClient()
        => Inactivate();

    private void ValidateFullName(string fullName)
    {
        DomainExceptionValidation.ValidateRequiredString(fullName, "Full name is required.");
        DomainExceptionValidation.ValidateMaxLength(fullName, 100, "Full name can have a maximum of 100 characters.");
    }

    private void ValidateDocumentNumber(string documentNumber)
    {
        DomainExceptionValidation.ValidateRequiredString(documentNumber, "Document number is required.");
        DomainExceptionValidation.ValidateMaxLength(documentNumber, 20, "Document number can have a maximum of 20 characters.");
        DomainExceptionValidation.ValidateFormat(StringFormatter.IsValidCpfOrCnpj, documentNumber, "Invalid CPF or CNPJ.");
    }

    private void ValidatePhone(string? phone)
    {
        DomainExceptionValidation.ValidateMaxLength(phone, 20, "Phone number can have a maximum of 20 characters.");
        DomainExceptionValidation.ValidateFormat(StringFormatter.IsValidPhoneNumber, phone, "Invalid phone number.");
    }

    private void ValidateEmail(string? email)
    {
        DomainExceptionValidation.ValidateMaxLength(email, 254, "Email can have a maximum of 254 characters.");
        DomainExceptionValidation.ValidateEmailFormat(email, "Invalid email format.");
    }

    private void ValidateZipCode(string? zipCode)
        => DomainExceptionValidation.ValidateMaxLength(zipCode, 10, "Zip code can have a maximum of 10 characters.");
}