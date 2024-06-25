using Patient_Transferral_System.Services;

namespace Patient_Transferral_System.Entities;

public class PatientDataReceiver
{
    public List<TransferralPatient> GetPatientDataFromLines()
    {
        List<TransferralPatient> patients = new List<TransferralPatient>();
        
        CsvDataReader csvDataReader = new CsvDataReader();
        List<String[]> CsvLines = csvDataReader.GetDataFromCsv();
        
        foreach (var line in CsvLines)
        {
            TransferralPatient newTransferralPatient = new TransferralPatient(line[1], line[2], line[3], line[4]);
            patients.Add(newTransferralPatient);
        }

        return patients;
    }
}