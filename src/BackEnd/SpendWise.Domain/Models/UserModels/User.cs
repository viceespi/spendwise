namespace SpendWise.Domain.Models.UserModels;

public class User
{
    public User(string name, Guid id)
    {
        Name = name;
        Id = id;
    }
    
    public string Name { get; set; }
    
    public Guid Id { get; set; }
}