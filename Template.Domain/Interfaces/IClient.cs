namespace Template.Domain.Interfaces;

public interface ICreateClient
{
    string FullName { get; set; }
    string DocumentNumber { get; set; }
    string Email { get; set; }
    string? Phone { get; set; }
    string? ZipCode { get; set; }
}

public interface IUpdateClient
{
    string FullName { get; set; }
    string Email { get; set; }
    string? Phone { get; set; }
    string? ZipCode { get; set; }
}