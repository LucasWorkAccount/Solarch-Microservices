using Patient_Transferral_System.Services;

namespace Patient_Transferral_System.Entities;

public class PatientDataReceiver
{
    public List<Patient> GetPatientDataFromLines()
    {
        List<Patient> patients = new List<Patient>();
        
        CsvDataReader csvDataReader = new CsvDataReader();
        List<String[]> CsvLines = csvDataReader.GetDataFromCsv();
        
        foreach (var line in CsvLines)
        {
            Patient newPatient = new Patient(line[1], line[2], line[3], line[4]);
            patients.Add(newPatient);
        }

        return patients;
    }
}