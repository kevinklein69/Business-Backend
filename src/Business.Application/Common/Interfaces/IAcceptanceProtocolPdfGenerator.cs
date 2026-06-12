using Business.Domain.Entities;

namespace Business.Application.Common.Interfaces;

public interface IAcceptanceProtocolPdfGenerator
{
    byte[] Generate(Order order, OrderAcceptance acceptance);
}
