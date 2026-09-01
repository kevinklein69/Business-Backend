using Business.Application.Common.Exceptions;
using Business.Application.Common.Interfaces;
using Business.Application.Features.Employees.CreateEmployee;
using Business.Application.Features.Employees.DeleteEmployee;
using Business.Application.Features.Employees.GetEmployeeById;
using Business.Application.Features.Employees.GetEmployees;
using Business.Application.Features.Employees.UpdateEmployee;
using Business.Application.Features.Employees.UpdateEmployeeRole;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeesController(ISender sender, ICurrentUserService currentUser) : ControllerBase
{
    public record CreateEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        Role Role,
        string? Department,
        string Street,
        string HouseNumber,
        string Zip,
        string City,
        string? Phone,
        DateOnly EntryDate,
        int? ProbationMonths,
        DateOnly? ProbationEndDate,
        int? VacationDaysEntitlement,
        int? InitialBalanceMinutes);

    public record UpdateEmployeeRoleRequest(Role Role);

    public record UpdateEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        Role Role,
        string? Department,
        string? Password,
        string Street,
        string HouseNumber,
        string Zip,
        string City,
        string? Phone,
        DateOnly EntryDate,
        int? ProbationMonths,
        DateOnly? ProbationEndDate,
        int? VacationDaysEntitlement,
        int? InitialBalanceMinutes);

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmployeesQuery(), cancellationToken);
        return Ok(result);
    }

    /// The current user's own profile — visible to every authenticated user, unlike GetById below.
    [HttpGet("me")]
    public async Task<ActionResult<EmployeeDetailDto>> GetMe(CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var result = await sender.Send(new GetEmployeeByIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new GetEmployeeByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateEmployeeCommand(
                request.FirstName, request.LastName, request.Email, request.Password, request.Role, request.Department,
                request.Street, request.HouseNumber, request.Zip, request.City, request.Phone,
                request.EntryDate, request.ProbationMonths, request.ProbationEndDate, request.VacationDaysEntitlement,
                request.InitialBalanceMinutes),
            cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new UpdateEmployeeCommand(
                    id, request.FirstName, request.LastName, request.Email, request.Role, request.Department, request.Password,
                    request.Street, request.HouseNumber, request.Zip, request.City, request.Phone,
                    request.EntryDate, request.ProbationMonths, request.ProbationEndDate, request.VacationDaysEntitlement,
                    request.InitialBalanceMinutes),
                cancellationToken);

            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> UpdateRole(Guid id, UpdateEmployeeRoleRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new UpdateEmployeeRoleCommand(id, request.Role), cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteEmployeeCommand(id), cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
