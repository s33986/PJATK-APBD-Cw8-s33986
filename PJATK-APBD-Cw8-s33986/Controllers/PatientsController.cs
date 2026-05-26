using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_Cw8_s33986.Models;
using PJATK_APBD_Cw8_s33986.Services;

namespace PJATK_APBD_Cw8_s33986.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(PatientsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Patient>>> GetAllPatientsAsync(string? search,
        CancellationToken cancellationToken)
    {
        return  Ok(await service.GetPatientsAsync(search, cancellationToken));
    }
}