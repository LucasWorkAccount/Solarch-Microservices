using Medical_Record_System.Entities;

namespace Medical_Record_System.Repositories;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly IEventRepository _eventRepository;

    public MedicalRecordRepository(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    
    public async Task<MedicalRecord> GetMedicalRecordByUuid(Guid uuid)
    {
        var medicalRecordEvents = await _eventRepository.GetEventsByUuid(uuid);
        return new MedicalRecordCollection(uuid, medicalRecordEvents).MedicalRecords[0];
    }
}