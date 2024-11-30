namespace Template.Application.ViewModels.Clients;

public class ClientVM
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string DocumentNumber { get; set; }
    public string? Phone { get; set; }
    public string? ZipCode { get; set; }

    public ClientVM() { }

    public ClientVM(Guid id, string fullName, string documentNumber, string? phone, string? zipCode)
    {
        Id = id;
        FullName = fullName;
        DocumentNumber = documentNumber;
        Phone = phone;
        ZipCode = zipCode;
    }
}