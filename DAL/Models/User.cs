using System;
using System.Collections.Generic;

public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string password { get; set; }
    public string role { get; set; }
}

public class UserResponse
{
    public string Status { get; set; }
    public List<User> Data { get; set; }
}

