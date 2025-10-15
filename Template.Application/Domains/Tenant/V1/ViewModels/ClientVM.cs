namespace Template.Application.Domains.Tenant.V1.ViewModels;

public class ClientVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string DocumentNumber { get; set; }
    public string? Phone { get; set; }
    public string? ZipCode { get; set; }
    public bool Paid { get; set; }

    public ClientVM() { }

    public ClientVM(Guid id, string fullName, string documentNumber, string? phone, string? zipCode, bool paid)
    {
        Id = id;
        FullName = fullName;
        DocumentNumber = documentNumber;
        Phone = phone;
        ZipCode = zipCode;
        Paid = paid;
    }
}