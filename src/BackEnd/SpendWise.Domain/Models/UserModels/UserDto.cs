namespace SpendWise.Domain.Models.UserModels;

public class UserDto
{
    public UserDto(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; }
}