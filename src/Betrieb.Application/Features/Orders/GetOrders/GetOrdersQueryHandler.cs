using Betrieb.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Betrieb.Application.Features.Orders.GetOrders;

public class GetOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id,
                o.Title,
                o.Description,
                o.Customer,
                o.Status,
                o.CreatedAt,
                o.Assignees
                    .Select(a => new AssigneeDto(a.Id, a.FirstName + " " + a.LastName))
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}
