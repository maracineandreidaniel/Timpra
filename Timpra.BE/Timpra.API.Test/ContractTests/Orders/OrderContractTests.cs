using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.API.Test.ContractTests.Common.Models;
using Timpra.API.Test.Utilities;
using System.Net.Http.Json;
using Timpra.BusinessLogic.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Timpra.API.Test.ContractTests.Orders;

public class OrderContractTests(CustomEndpointContractTestFactory<Program> factory) : IClassFixture<CustomEndpointContractTestFactory<Program>>
{
    private const string Route = "Orders";
    private readonly IOrderService orderService = factory.Services.GetRequiredService<IOrderService>();
    private readonly CustomEndpointContractTestFactory<Program> factory = factory;

    [Theory]
    [AutoDomainData]
    public async Task Get_ShouldReturnOkWithContent(OrderDto orderDto)
    {
        // Arrange
        orderService.GetByIdAsync(orderDto.Id).Returns(orderDto);

        // Act
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var loginResponse = await client.PostAsJsonAsync("Authenticate/login", new LoginDTO { Username = "string", Password = "string" });
        var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<LoginResponseDTO>(loginResponseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
        var response = await client.GetAsync($"{Route}/{orderDto.Id}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseModel = JsonSerializer.Deserialize<OrderDtoTestModel>(responseContent);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        responseModel.Should().BeOfType<OrderDtoTestModel>();
    }
}
