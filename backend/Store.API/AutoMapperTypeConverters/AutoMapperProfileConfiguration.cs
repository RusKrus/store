using System.Text.Json;
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
using Store.API.Contracts.Response.Common;
using Store.API.Contracts.Response.OrderItem;
using Store.API.Contracts.Response.Orders;
using Store.API.Contracts.Response.Product;
using Store.API.Contracts.Response.Users;
using Store.Application.Requests.Commands.Product;
using Store.Domain.Models;

namespace Store.API.AutoMapperTypeConverters;

public class AutoMapperProfileConfiguration : Profile
{
  public AutoMapperProfileConfiguration()
  {
    CreateMap<CreateProductRequest, CreateProductCommand>();
    CreateMap<Product, ProductResponse>()
      .ForMember(destination => destination.Images, opt => opt.MapFrom(product =>
        product.Images
          .Select(imageDescription => new ImageResponse(
            imageDescription.Id,
            $"api/product/{imageDescription.Id}/image",
            imageDescription.Description))))
      .ForMember(destination => destination.Specs, opt => opt.MapFrom(product => 
        product.Specs.Select(specs =>  JsonSerializer.Deserialize<ProductSpecGroup>(specs))));
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

    CreateMap<OrderItem, OrderItemResponse>();
    CreateMap<OrderReceiverInfo, ReceiverInfoResponse>()
      .ForCtorParam(nameof(ReceiverInfoResponse.ReceiverFullName), opt => opt.MapFrom(src => $"{src.ReceiverFirstName} {src.ReceiverLastname}"));
    CreateMap<Order, OrderResponse>();
    CreateMap<CreateOrderRequest, CreateOrderCommand>();
  }
}
