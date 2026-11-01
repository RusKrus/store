using Store.Domain.Enums;

namespace Tests.Integration.Data;

public sealed class UsersData
{
    public sealed class AddUserData
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required UserRole Role { get; set; }

    
        public string FullName => $"{FirstName} {LastName}";
    }
}