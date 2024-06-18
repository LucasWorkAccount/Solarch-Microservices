using Microsoft.EntityFrameworkCore;

namespace Medical_Record_System;

public class StateEventHandler : IEventHandler
{
    private readonly MedicalRecordEventStoreContext _medicalRecordEventStoreContext;

    public StateEventHandler(MedicalRecordEventStoreContext medicalRecordEventStoreContext)
    {
        _medicalRecordEventStoreContext = medicalRecordEventStoreContext;
    }

    public async Task Apply(Event @event)
    {
        var medicalRecord = await _medicalRecordEventStoreContext.MedicalRecords
            .FirstOrDefaultAsync(medicalRecord => medicalRecord.Uuid == @event.Uuid);

        if (medicalRecord == null)
        {
            return;
        }
            
        medicalRecord.Apply(@event);
        await _medicalRecordEventStoreContext.SaveChangesAsync();
    }
}