namespace Template.Domain.Entity;

public abstract class Entity
{
    public Guid Id { get; init; }
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeleteAt { get; private set; }

    public Entity()
    {
        this.Id = Guid.NewGuid();
        this.Active = true;
        this.CreatedAt = DateTime.Now;
    }

    public void Updated() 
        => this.UpdatedAt = DateTime.Now;

    public void Delete() 
        => this.DeleteAt = DateTime.Now;

    public void Inactivate() 
        => this.Active = false;

    public void Activate() 
        => this.Active = true;
}