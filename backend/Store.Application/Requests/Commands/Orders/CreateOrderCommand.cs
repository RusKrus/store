namespace Application.Requests.Commands.Orders;

public record CreateOrderCommand(
    string ReceiverFirstName,
    string ReceiverLastName,
    string ReceiverEmail,
    string ReceiverPhoneNumber,
    string City,
    string ReceiverAddress,
    string? OrderComment
    );