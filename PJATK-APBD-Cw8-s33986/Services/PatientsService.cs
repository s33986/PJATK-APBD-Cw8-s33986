using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Cw8_s33986.DTOs;
using PJATK_APBD_Cw8_s33986.Infrastructure;
using PJATK_APBD_Cw8_s33986.Models;

namespace PJATK_APBD_Cw8_s33986.Services;

public class PatientsService(HospitalDbContext context) : IPatientsService
{
    public async Task<IEnumerable<PatientDto>> GetPatientsAsync(string? search, CancellationToken cancellationToken)
    {
        var query = context.Patients.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var searchTerm = $"%{search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.FirstName, searchTerm) || EF.Functions.Like(p.LastName, searchTerm));
        }

        return await query.Select(p => new PatientDto(
            p.Pesel,
            p.FirstName,
            p.LastName,
            p.Age,
            p.Sex ? "Male" : "Female",
            p.Admissions.Select(a => new AdmissionDto(
                a.Id,
                a.AdmissionDate,
                a.DischargeDate,
                new WardDto(
                    a.Ward.Id, a.Ward.Name, a.Ward.Description)
            )),
            p.BedAssignments.Select(ba => new BedAssignmentsDto(
                ba.Id,
                ba.From,
                ba.To,
                new BedDto(
                    ba.BedId,
                    new BedTypeDto(
                        ba.Bed.BedType.Id,
                        ba.Bed.BedType.Name,
                        ba.Bed.BedType.Description
                    ),
                    new RoomDto(
                        ba.Bed.Room.Id,
                        ba.Bed.Room.HasTv,
                        new WardDto(
                            ba.Bed.Room.Ward.Id,
                            ba.Bed.Room.Ward.Name,
                            ba.Bed.Room.Ward.Description
                        )
                    )
                )
            )))).ToListAsync(cancellationToken);
    }
}