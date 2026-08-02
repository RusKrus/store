using Application.Common.PaginationOptions;

namespace Application.Queiries.Users;

public record GetAllUsersQuery(PaginationOptions Options, string? SearchString);