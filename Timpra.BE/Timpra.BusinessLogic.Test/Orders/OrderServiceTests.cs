using Xunit;
using System.Threading.Tasks;
using Timpra.BusinessLogic.Test.Utilities;
using NSubstitute;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.BusinessLogic.DTOs.Orders;
using FluentAssertions;
using Timpra.DataAccess.Repository.Abstraction;
using Timpra.DataAccess.Entities;
using Timpra.BusinessLogic.Services;

namespace Timpra.BusinessLogic.Test.Orders;

public class OrderServiceTests
{
    private readonly IRepository<Order> orderRepositoryMock = Substitute.For<IRepository<Order>>();
    private readonly IOrderService orderService;

    public OrderServiceTests()
    {
        orderService = new OrderService(orderRepositoryMock);
    }

    [Theory]
    [AutoDomainData]
    public async Task AddOrder_Should_Call_AddAsync_Once_When_OrderDto_Is_Provided(Order order, OrderDto orderDto)
    {
        // Act
        var result = await orderService.AddAsync(orderDto);

        // Assert
        await orderRepositoryMock
            .Received(1)
            .AddAsync(Arg.Is<Order>(o => o.Id == orderDto.Id));

        result.Should().BeEquivalentTo(orderDto);
    }

    [Theory]
    [AutoDomainData]
    public async Task RemoveOrder_Should_Call_RemoveAsync_Once_When_OrderDto_Id_Is_Provided(Order order, OrderDto orderDto)
    {
        // Arrange
        orderRepositoryMock.GetByIdAsync(orderDto.Id).Returns(order);

        // Act
        var result = await orderService.RemoveAsync(orderDto.Id);

        // Assert
        await orderRepositoryMock
            .Received(1)
            .UpdateAsync(order, orderDto.Id);

        result.IsDeleted.Should().BeTrue();
    }
}
