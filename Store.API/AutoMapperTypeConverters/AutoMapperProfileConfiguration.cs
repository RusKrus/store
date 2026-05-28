using Application.Commands;
using Application.Commands.Auth;
using Application.Commands.Users;
using Application.Common.PaginationResult;
using AutoMapper;
using Store.API.Contracts.Requests.Auth;
using Store.API.Contracts.Requests.Product;
using Store.API.Contracts.Requests.User;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Users;
using Store.Domain.Models;

namespace Store.AutoMapperTypeConverters;

public class AutoMapperProfileConfiguration : Profile
{
  public AutoMapperProfileConfiguration()
  {
    CreateMap<CreateProductRequest, CreateProductCommand>();
    CreateMap<Product, ProductResponse>();
    CreateMap<UpdateProductCommand, UpdateProductRequest>();

    CreateMap(typeof(PaginationResult<>), typeof (PaginationResponse<>));

    CreateMap<RegisterRequest, RegisterCommand>();
    CreateMap<LoginRequest, LoginCommand>();

    CreateMap<User, UserResponse>()
      .ForCtorParam(nameof(UserResponse.FullName), opt =>
        opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
    CreateMap<CreateUserRequest, CreateUserCommand>();
    CreateMap<UpdateUserRequest, UpdateUserCommand>();
    CreateMap<UpdateProductRequest, UpdateProductCommand>();
    CreateMap<UpdatePasswordRequest, UpdatePasswordCommand>();
  }
}
