using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PedidoGestao.Domain.Entities;
using PedidoGestao.Infrastructure.Persistence;
using Azure.Messaging.ServiceBus;
using System.Text.Json;



namespace PedidoGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public OrdersController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        //Console.WriteLine(_context.Orders.ToListAsync() + " <- São esses os dados dos pedidos");
        var orders = await _context.Orders.ToListAsync();
        return Ok(orders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        order.Status = "Pendente";
        order.DataCriacao = DateTime.UtcNow;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var connectionString = _configuration["ServiceBus:ConnectionString"];
        var queueName = _configuration["ServiceBus:QueueName"];
        var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(queueName);

        var messageBody = JsonSerializer.Serialize(new { order.Id });
        var message = new ServiceBusMessage(messageBody);
        await sender.SendMessageAsync(message);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
