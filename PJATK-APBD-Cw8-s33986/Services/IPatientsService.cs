using PJATK_APBD_Cw8_s33986.DTOs;

namespace PJATK_APBD_Cw8_s33986.Services;

public interface IPatientsService
{
    Task<IEnumerable<PatientDto>> GetPatientsAsync(string? search, CancellationToken cancellationToken);
    
}