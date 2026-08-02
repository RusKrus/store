using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Store.Domain.Enums;

namespace Store.Domain.Models;

public class User : BaseEntity
{
  private User() {}

  public User(string firstName, string lastName, string email, string passwordHash, UserRole? role)
  {
    SetValue(firstName, lastName, email, passwordHash, role);
    CreatedAt = DateTime.UtcNow;
  }
  public string FirstName { get; private set; } = null!;
  public string LastName { get; private set; } = null!;
  public string FullName => $"{FirstName} {LastName}";
  public string Email { get; private set; } = null!;
  public string PasswordHash { get; private set; } = null!;
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; } = null;
  public UserRole Role { get; private set; }
  public Cart? Cart { get; private set; } = null;
  public List<Order>? Orders { get; private set; } = null;

  public void Update(string firstName, string lastName, string email, string passwordHash, UserRole? role)
  {
    SetValue(firstName, lastName, email, passwordHash, role);
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateProfile(string firstName, string lastName, string email)
  {
    if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
    {
      throw new ValidationException("User data entered incorrectly");
    }
    FirstName = firstName;
    LastName = lastName;
    Email = email;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdatePassword(string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ValidationException("Password entered incorrectly");
    }
    PasswordHash = passwordHash;
    UpdatedAt = DateTime.UtcNow;
  }

  public CartsMergeStatuses AssignCart(Cart cart)
  {
    if (Cart is null)
    {
      Cart = cart;
      return CartsMergeStatuses.CartAssigned;
    }
    else
    {
      Cart.MergeCarts(cart);
      return CartsMergeStatuses.CartsMerged;
    }
  }

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
    Role = role ?? UserRole.Customer;
  }
}
