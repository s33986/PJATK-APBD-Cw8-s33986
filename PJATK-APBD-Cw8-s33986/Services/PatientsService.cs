using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Cw8_s33986.DTOs;
using PJATK_APBD_Cw8_s33986.Exceptions;
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

    public async Task AssignBedAsync(string pesel, BedAssignmentRequestDto request,
        CancellationToken cancellationToken)
    {
        var patientExists = await context.Patients.AnyAsync(p => p.Pesel == pesel, cancellationToken);
        if (!patientExists)
        {
            throw new NotFoundException($"Pacjent o peselu {pesel} nie istnieje");
        }
        
        var bedExists = await context.Beds.AnyAsync(b => b.Room.Ward.Name == request.ward && b.BedType.Name == request.bedType,cancellationToken);
        if (!bedExists)
        {
            throw new NotFoundException($"Na oddziale {request.ward} nie znaleziono łóżka typu {request.bedType}");
        }

        var freeBed = await context.Beds
            .Where(b => b.Room.Ward.Name == request.ward && b.BedType.Name == request.bedType)
            .Where(b => !b.BedAssignments.Any(ba =>
                (request.to == null || ba.From < request.to) && (ba.To == null || request.from < ba.To)))
            .FirstOrDefaultAsync(cancellationToken);

        if (freeBed == null)
        {
            throw new ConflictException(
                $"Łóżko: '{request.bedType}' na oddziale '{request.ward}' jest niedostępne w wybranym terminie");
        }

        var newAssignment = new BedAssignment
        {
            PatientPesel = pesel,
            BedId = freeBed.Id,
            From = request.from,
            To = request.to,
        };

        context.BedAssignments.Add(newAssignment);
        await context.SaveChangesAsync(cancellationToken);
        
    }
}