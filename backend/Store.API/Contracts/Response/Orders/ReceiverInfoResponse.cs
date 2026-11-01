namespace Store.API.Contracts.Response.Orders;

public record ReceiverInfoResponse(
    string ReceiverFullName,
    string ReceiverAddress,
    string ReceiverEmail,
    string ReceiverPhoneNumber,
    string City,
    string? OrderComment
    );