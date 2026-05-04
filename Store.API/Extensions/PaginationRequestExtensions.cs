using Application.Common.PaginationOptions;
using Store.API.Contracts.Requests.Common;

namespace Store.API.Extensions;

public static class PaginationRequestExtensions
{
    public static PaginationOptions ToOptions(this PaginationRequest paginationRequest)
    {
        return PaginationOptions.Create(
            paginationRequest.PageNumber,
            paginationRequest.PageSize,
            paginationRequest.ShowAll
        );
    }
}