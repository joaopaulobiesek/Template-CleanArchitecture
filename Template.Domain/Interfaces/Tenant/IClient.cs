namespace Template.Domain.Interfaces.Tenant;

public interface ICreateClient
{
    string FullName { get; set; }
    string DocumentNumber { get; set; }
    string Email { get; set; }
    string? Phone { get; set; }
    string? ZipCode { get; set; }
    bool Paid { get; set; }
}

public interface IUpdateClient
{
    string FullName { get; set; }
    string Email { get; set; }
    string? Phone { get; set; }
    string? ZipCode { get; set; }
    bool Paid { get; set; }
}