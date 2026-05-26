namespace PJATK_APBD_Cw8_s33986.DTOs;

public record AdmissionDto(
    int id,
    DateTime admissionDate,
    DateTime? dischargeDate,
    WardDto ward
);