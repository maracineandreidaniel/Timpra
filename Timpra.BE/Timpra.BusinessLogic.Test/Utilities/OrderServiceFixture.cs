using Timpra.BusinessLogic.Services;
using Timpra.DataAccess.Entities;
using Timpra.DataAccess.Repository.Abstraction;
using NSubstitute;

namespace Timpra.BusinessLogic.Test.Utilities;

public class OrderServiceFixture
{
    public OrderService OrderService { get; }
    public IRepository<Order> OrderRepositoryMock { get; }

    public OrderServiceFixture()
    {
        OrderRepositoryMock = Substitute.For<IRepository<Order>>();
        OrderService = new OrderService(OrderRepositoryMock);
    }
}
