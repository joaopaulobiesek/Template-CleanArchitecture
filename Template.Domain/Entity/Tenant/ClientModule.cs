using Template.Domain.Entity;
using Template.Domain.Entity.Tenant;

public sealed class ClientModule : Entity
{//Regra: Sempre que o modulo for desativado ele nao podera ser reativado. assim ficara um historico do mesmo
    public Guid ClientId { get; private set; }
    public string Module { get; private set; }
    public DateTime ActivatedAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }
    public bool IsActive => DeactivatedAt == null;

    public Client Client { get; private set; }

    public void Activate(Guid clientId, string module)
    {
        ClientId = clientId;
        Module = module;
        ActivatedAt = DateTime.UtcNow; 
    }
   
    public void Deactivate()
    {
        DeactivatedAt = DateTime.UtcNow;
    }
}