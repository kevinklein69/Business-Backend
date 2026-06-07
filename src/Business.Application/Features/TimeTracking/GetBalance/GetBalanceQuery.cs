using MediatR;

namespace Business.Application.Features.TimeTracking.GetBalance;

public record GetBalanceQuery : IRequest<BalanceDto>;

public record BalanceDto(
    int WeekMinutes,
    int WeekTargetMinutes,
    int MonthMinutes,
    int MonthTargetMinutes,
    int TotalBalanceMinutes);
