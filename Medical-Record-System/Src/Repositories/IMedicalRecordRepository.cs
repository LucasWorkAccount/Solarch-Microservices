using Medical_Record_System.Entities;

namespace Medical_Record_System.Repositories;

public interface IMedicalRecordRepository
{
    public Task<MedicalRecord> GetMedicalRecordByUuid(Guid uuid);
}