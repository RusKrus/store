namespace Application.Requests.Commands.Orders;

public record CreateOrderCommand(
    string ReceiverName,
    string ReceiverLastName,
    string ReceiverAddress,
    string ReceiverEmail,
    string ReceiverPhoneNumber,
    string City,
    string Address,
    string? OrderComment
    );