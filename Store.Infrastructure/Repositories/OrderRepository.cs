using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class OrderRepository(StoreContext context) : BaseRepository<Order>(context), IOrderRepository
{

}