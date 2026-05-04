using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Store.API.Contracts.Requests.Common;

public record PaginationRequest
{
    /// <summary>
    /// Items per page. Default: 20
    /// </summary>
    [FromQuery]
    [Range(0, 100)]
    public int PageSize { get; init; } = 20;


    /// <summary>
    /// Current page. Default: 1
    /// </summary>
    [FromQuery]
    [Range(0, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Optional flag, if it is required to ignore pagination and return all items. Default: false
    /// </summary>
    [FromQuery]
    public bool ShowAll { get; init; } = false;
}