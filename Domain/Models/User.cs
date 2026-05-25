using System.ComponentModel.DataAnnotations;
using Store.Domain.Enums;

namespace Store.Domain.Models;

public class User
{
  private User() {}

  public User(string firstName, string lastName, string email, string passwordHash, UserRole? role)
  {
    SetValue(firstName, lastName, email, passwordHash, role);
  }
  public int Id { get; init; }
  public string FirstName { get; private set; } = null!;
  public string LastName { get; private set; } = null!;
  public string Email { get; private set; } = null!;
  public string PasswordHash { get; private set; } = null!;
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; } = null;
  public UserRole Role { get; private set; }
  public Cart? Cart { get; private set; } = null;
  public List<Order>? Orders { get; private set; } = null;

  private void SetValue(string firstName, string lastName, string email, string passwordHash, UserRole? role)
  {
    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ValidationException("User data entered incorrectly");
    }
    FirstName = firstName;
    LastName = lastName;
    Email = email;
    PasswordHash = passwordHash;
    CreatedAt = DateTime.UtcNow;
    Role = role ?? UserRole.Customer;
  }
}
