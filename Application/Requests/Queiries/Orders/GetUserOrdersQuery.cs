using Application.Common.PaginationOptions;
using Store.Domain.Enums;

namespace Application.Requests.Queiries.Orders;

public record GetUserOrdersQuery(PaginationOptions options, OrderStatuses? status);