using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
}

public class UserResponse
{
    public string Status { get; set; }
    public List<User> Data { get; set; }
}
