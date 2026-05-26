namespace PJATK_APBD_Cw8_s33986.DTOs;

public record PatientDto (
    string pesel,
    string firstName,
    string lastName,
    int age,
    string sex,
    IEnumerable<AdmissionDto> admissions,
    IEnumerable<BedAssignmentsDto> bedAssignments
);