using Patient_Transferral_System.Services;

namespace Patient_Transferral_System.Entities;

public class PatientDataReceiver
{
    public async Task<List<TransferralPatient>> GetPatientDataFromLines()
    {
        List<TransferralPatient> patients = new List<TransferralPatient>();
        
        CsvDataReader csvDataReader = new CsvDataReader();
        List<string> CsvLines = await csvDataReader.GetDataFromCsv();
        
        foreach (var line in CsvLines)
        {
            if (line != CsvLines[0])
            {
                List<string> fields = line.Split(',').ToList();
                try
                {
                    TransferralPatient newTransferralPatient = new TransferralPatient(fields[1], fields[2], fields[3], fields[4]);
                    patients.Add(newTransferralPatient);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        return patients;
    }
}