using System.ComponentModel.DataAnnotations;

namespace Store.API.Contracts.Requests.Order;

public record CreateOrderRequest
{
    [Length(1, 50)]
    public string ReceiverFirstName { get; init; }
    [Length(1, 50)]
    public string ReceiverLastName { get; init;  }
    [EmailAddress]
    public string ReceiverEmail { get; init; }
    [Phone]
    public string ReceiverPhoneNumber { get; init; }
    [Length(1, 50)]
    public string City { get; init; }
    [Length(1, 50)]
    public string Address { get; init; }
    [Length(0, 300)]
    public string? OrderComment { get; init; }
}