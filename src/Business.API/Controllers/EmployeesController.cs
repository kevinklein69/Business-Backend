using Business.Application.Common.Exceptions;
using Business.Application.Features.Employees.CreateEmployee;
using Business.Application.Features.Employees.DeleteEmployee;
using Business.Application.Features.Employees.GetEmployees;
using Business.Application.Features.Employees.UpdateEmployee;
using Business.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Business.API.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeesController(ISender sender) : ControllerBase
{
    public record CreateEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        Role Role,
        string? Department);

    public record UpdateEmployeeRequest(
        string FirstName,
        string LastName,
        string Email,
        Role Role,
        string? Department,
        string? Password);

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEmployeesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateEmployeeCommand(request.FirstName, request.LastName, request.Email, request.Password, request.Role, request.Department),
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
                new UpdateEmployeeCommand(id, request.FirstName, request.LastName, request.Email, request.Role, request.Department, request.Password),
                cancellationToken);

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
