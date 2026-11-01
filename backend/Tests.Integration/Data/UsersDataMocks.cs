using Store.Domain.Enums;

namespace Tests.Integration.Data;

public sealed class UsersData
{
    public sealed class AddUserData
    {
        public AddUserData(string firstName, string lastName, string email, string password, UserRole role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Role = role;
        }
            
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

    
        public string FullName => $"{FirstName} {LastName}";
    }
}