using System;
using System.Collections.Generic;

public class User
{
    public int user_id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string role { get; set; }
}

public class UserResponse
{
    public string Status { get; set; }
    public List<User> Data { get; set; }
}

public class LocalDataResponse
{
    public string UsersJson { get; set; }
    public string SubjectsJson { get; set; }
    public string ClassJson { get; set; }
    public string ClassSubjectsJson { get; set; }
    public string ClassStudentsJson { get; set; }
    public string StudentsJson { get; set; }
    public DateTime LastTimeUpdateUser { get; set; }
    public DateTime LastTimeUpdateSubject { get; set; }
    public DateTime LastTimeUpdateClass { get; set; }
}