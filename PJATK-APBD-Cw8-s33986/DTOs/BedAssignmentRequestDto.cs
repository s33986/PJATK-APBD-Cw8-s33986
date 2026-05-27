namespace PJATK_APBD_Cw8_s33986.DTOs;

public record BedAssignmentRequestDto(
    DateTime from,
    DateTime? to,
    string bedType,
    string ward
    );