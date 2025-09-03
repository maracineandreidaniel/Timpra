using Microsoft.AspNetCore.Mvc;
using Timpra.BusinessLogic.DTOs.Orders;
using System.Collections.Generic;
using Timpra.BusinessLogic.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Timpra.BusinessLogic.DTOs;
using Timpra.API.Filters;
using System.Net;

namespace Timpra.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [TokenAuthenticationFilter]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService testService)
        {
            _orderService = testService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAll();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] OrderDto newOrder)
        {
            var order = await _orderService.AddAsync(newOrder);
            return Created(nameof(GetById), order); ;
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] OrderDto newOrder)
        {
            if (id != newOrder.Id)
            {
                return BadRequest("Invalid request");
            }

            var order = await _orderService.UpdateAsync(newOrder, id);
            return Ok(order);
        }

        [HttpDelete("{orderId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(int orderId)
        {
            var dbOrder = await _orderService.RemoveAsync(orderId);
            if (dbOrder != null)
            {
                return NoContent();
            }

            return BadRequest("Something went wrong...");
        }

        [HttpPatch]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Archive([FromBody] OrderDto order)
        {
            var dbOrder = await _orderService.ArchiveAsync(order.Id);
            if (dbOrder != null)
            {
                return NoContent();
            }

            return BadRequest("Something went wrong...");
        }

        [Route("paginated")]
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedListResponseDto<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersPaginated([FromQuery] int pageIndex, [FromQuery] int itemsNumber, [FromQuery] string sortField, [FromQuery] string? sortDirection, [FromBody] OrderListFilterDto filter)
        {
            var paginatedOrders = await _orderService.GetOrdersPaginatedAsync(pageIndex, itemsNumber, sortField, sortDirection, filter);
            return Ok(paginatedOrders);
        }
    }
}
