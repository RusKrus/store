using Store.Domain.Enums;

namespace Store.Domain.Models;

public class User
{
  public int Id { get; set; }
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string Email { get; set; } = null!;
  public UserRole Role { get; set; }
  public Cart? Cart { get; set; } = null;
  public List<Order>? Orders { get; set; } = null;
}
