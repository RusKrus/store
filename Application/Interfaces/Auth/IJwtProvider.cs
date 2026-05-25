using Store.Domain.Models;

namespace Store.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}