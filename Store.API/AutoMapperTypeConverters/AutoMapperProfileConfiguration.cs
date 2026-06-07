using Application.Commands;
using Application.Commands.Auth;
using Application.Commands.Cart;
using Application.Commands.Users;
using Application.Common.PaginationResult;
using Application.Requests.Commands.Orders;
using AutoMapper;
using Store.API.Contracts.Requests.Auth;
using Store.API.Contracts.Requests.Cart;
using Store.API.Contracts.Requests.Order;
using Store.API.Contracts.Requests.Product;
using Store.API.Contracts.Requests.User;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Cart;
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
    CreateMap<UpdateProfileRequest, UpdateProfileCommand>();
    CreateMap<UpdatePasswordRequest, UpdatePasswordCommand>();

    CreateMap<AddProductToCartRequest, AddProductToCartCommand>();
    CreateMap<CartItem, CartItemInCartResponse>()
      .ForCtorParam(nameof(CartItemInCartResponse.ProductName), opt =>
        opt.MapFrom(src => src.Product.Name));
    CreateMap<Cart, CartResponse>();

    CreateMap<CreateOrderRequest, CreateOrderCommand>();
  }
}
