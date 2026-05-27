using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_Cw8_s33986.DTOs;
using PJATK_APBD_Cw8_s33986.Exceptions;
using PJATK_APBD_Cw8_s33986.Models;
using PJATK_APBD_Cw8_s33986.Services;

namespace PJATK_APBD_Cw8_s33986.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Patient>>> GetAllPatientsAsync(string? search,
        CancellationToken cancellationToken)
    {
        return  Ok(await service.GetPatientsAsync(search, cancellationToken));
    }

    [HttpPost("{pesel}/bedassignments")]
    public async Task<IActionResult> AssignBedAsync(string pesel, [FromBody] BedAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            await service.AssignBedAsync(pesel, request, cancellationToken);
            return Created();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return NotFound(e.Message);
        }
    }
}