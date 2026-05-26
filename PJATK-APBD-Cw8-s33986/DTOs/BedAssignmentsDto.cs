namespace PJATK_APBD_Cw8_s33986.DTOs;

public record BedAssignmentsDto(
    int id,
    DateTime from,
    DateTime? to,
    BedDto bed
);