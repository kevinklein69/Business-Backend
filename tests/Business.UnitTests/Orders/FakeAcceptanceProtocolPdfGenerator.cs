using Business.Application.Common.Interfaces;
using Business.Domain.Entities;

namespace Business.UnitTests.Orders;

public class FakeAcceptanceProtocolPdfGenerator : IAcceptanceProtocolPdfGenerator
{
    public Order? LastOrder { get; private set; }
    public OrderAcceptance? LastAcceptance { get; private set; }

    public byte[] Generate(Order order, OrderAcceptance acceptance)
    {
        LastOrder = order;
        LastAcceptance = acceptance;
        return [1, 2, 3, 4];
    }
}
